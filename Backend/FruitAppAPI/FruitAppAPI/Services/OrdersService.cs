using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

using FruitAppAPI.Utils.ConfigModels;
using FruitAppAPI.Services.Interfaces;
using FruitAppAPI.ViewModels.Orders;
using FruitAppAPI.Models;

namespace FruitAppAPI.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IFruitGraphService _fruitsService;
        private readonly ISmsService _smsService;
        private readonly IProviderService _providerService;
        private readonly CloudQueueClient _cloudQueues;
        CloudTable orderTable;
        CloudTable messageSent;

        public OrdersService(
            IOptions<TableConfig> options,
            IProviderService providerService,
            IFruitGraphService fruitGraphService,
            ISmsService smsService)
        {
            _providerService = providerService;
            _smsService = smsService;
            _fruitsService = fruitGraphService;

            var account = CloudStorageAccount.Parse(options.Value.ConnectionString);
            var cloudTable = account.CreateCloudTableClient();
            _cloudQueues = account.CreateCloudQueueClient();

            orderTable = cloudTable.GetTableReference("orders");

            messageSent = cloudTable.GetTableReference("messages");
        }

        public async Task<Guid> Create(OrderCreateVM orderCreateVM)
        {
            var orderID = Guid.NewGuid();

            if(await CreateQueue(orderID) && await CreateOrder(orderCreateVM, orderID))
            {
                await SaveTransaction(orderID, new QueueModel
                {
                    Action = ACTIONS.CREATED,
                    PrevQuantity = 0f,
                    ChangeQuantity = orderCreateVM.Quantity,
                    PostQuantity = orderCreateVM.Quantity
                });

                var providers = await _providerService.FindProviders(orderCreateVM.Fruit, orderCreateVM.Certificates);
                IEnumerable<(Provider prov, double distance)> optimized = OptimizeDistance(providers, 127.34, 132.3).Take(4);

                var quantityPerProvider = Math.Ceiling(orderCreateVM.Quantity / optimized.Count());

                var tasks = new List<Task<string>>(optimized.Count());
                foreach (var provider in optimized)
                {
                    tasks.Add(_smsService.SendMessage($"Necesitamos {quantityPerProvider} de {orderCreateVM.Fruit}, ¿puedes?", provider.prov.PhoneNumber));
                }

                var responses = await Task.WhenAll(tasks);

                var phones = optimized.Select(provider => provider.prov.PhoneNumber);

                await SaveMessageRequested(phones, quantityPerProvider, orderID);

                await SaveIntoQueue(phones, orderID);

                return orderID;
            }

            return Guid.Empty;
        }

        Task SaveIntoQueue(IEnumerable<string> phones, Guid orderID)
        {
            var queue = _cloudQueues.GetQueueReference(orderID.ToString());
            var tasks = new List<Task>(phones.Count());
            foreach (var phone in phones)
            {
                var message = new QueueModel
                {
                    Message = $"Sent message to {phone}",
                    Action = ACTIONS.UPDATED,
                    TransactionID = Guid.NewGuid()
                };

                var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(message));
                tasks.Add(queue.AddMessageAsync(queueMessage));
            }

            return Task.WhenAll(tasks);
        }

        async Task<bool> SaveMessageRequested(IEnumerable<string> phones, double quantityRequested, Guid orderID)
        {
            var operation = new TableBatchOperation();
            foreach (var phone in phones)
            {
                var entity = new DynamicTableEntity()
                {
                    RowKey = phone,
                    PartitionKey = orderID.ToString(),
                    Properties = new Dictionary<string, EntityProperty>
                    {
                        { "Quantity", new EntityProperty(quantityRequested) }
                    }
                };

                operation.Insert(entity);
            }

            var results = await messageSent.ExecuteBatchAsync(operation);

            return results.Where(code => code.HttpStatusCode != 204).Count() == 0;
        }

        async Task<bool> CreateOrder(OrderCreateVM orderCreateVM, Guid orderID)
        {
            var dynamicTable = new DynamicTableEntity(orderCreateVM.Fruit, orderID.ToString())
            {
                Properties = new Dictionary<string, EntityProperty>()
                {
                    { "Quantity", new EntityProperty(orderCreateVM.Quantity) },
                    { "PendingQuantity", new EntityProperty(orderCreateVM.Quantity) }
                }
            };

            var op = TableOperation.Insert(dynamicTable);
            var result = await orderTable.ExecuteAsync(op);
            return result.HttpStatusCode == 204;
        }

        Task SaveTransaction(Guid queueId, QueueModel transaction)
        {
            transaction.TransactionID = Guid.NewGuid();
            var queue = _cloudQueues.GetQueueReference(queueId.ToString());
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(transaction));
            return queue.AddMessageAsync(message);
        }

        Task<bool> CreateQueue(Guid orderID)
        {
            var queue = _cloudQueues.GetQueueReference(orderID.ToString());
            
            return queue.CreateIfNotExistsAsync();
        }

        public Task<bool> Delete() => throw new NotImplementedException();

        public async Task<List<GetOrderVM>> Get()
        {
            var fruits = await _fruitsService.GetFruits();
            var queries = new List<Task<List<DynamicTableEntity>>>();
            foreach (var fruit in fruits)
            {
                queries.Add(ExecuteQuery(CreateQuery(fruit)));
            }

            return await AwaitResults(queries);
        }

        async Task<List<GetOrderVM>> AwaitResults(List<Task<List<DynamicTableEntity>>> queries)
        {
            var queriesResult = await Task.WhenAll(queries);
            var superList = new List<GetOrderVM>();
            foreach (var list in queriesResult)
            {
                superList.AddRange(list.Select(entity =>
                {
                    var vm = new GetOrderVM
                    {
                        Fruit = entity.PartitionKey,
                        OrderId = entity.RowKey,
                        Quantity = entity.Properties["Quantity"].DoubleValue ?? 0,
                        PendingQuantity = entity.Properties["PendingQuantity"].DoubleValue ?? 0
                    };

                    return vm;
                }));
            }

            return superList;
        }

        async Task<List<DynamicTableEntity>> ExecuteQuery(TableQuery<DynamicTableEntity> query)
        {
            var internalList = new List<DynamicTableEntity>();
            TableContinuationToken token = null;
            do
            {
                var result = await orderTable.ExecuteQuerySegmentedAsync(query, token);
                internalList.AddRange(result.Results);
                token = result.ContinuationToken;
            } while (token != null);

            return internalList;
        }

        TableQuery<DynamicTableEntity> CreateQuery(string fruitName)
        {
            var query = new TableQuery<DynamicTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, fruitName));

            return query;
        }

        public Task<bool> Update() => throw new NotImplementedException();

        private string SanitizeGuid(Guid guid) => guid.ToString().Replace("-", "");

        private IEnumerable<(Provider, double)> OptimizeDistance(IEnumerable<Provider> providers, double orderLat, double orderLon)
            => providers
            .Select(provider => (provider, distance: HavesineDistance(provider.Latitude, provider.Longitude, orderLat, orderLon)))
            .OrderBy(tuple => tuple.distance);
            
        private static double R = 6371;

        private double HavesineDistance(double x1, double y1, double x2, double y2)
        {
            double ToRadians(double val) => ((Math.PI / 180) * val);

            double dLon = ToRadians(x2 - x1);
            double dLat = ToRadians(y2 - y1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(ToRadians(y1)) * Math.Cos(ToRadians(y2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

            double d = R * c;

            return d;
        }

        public async Task<FullOrderVM> Get(Guid id)
        {
            var query = new TableQuery<DynamicTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString()));

            var result = await orderTable.ExecuteQuerySegmentedAsync(query, null);
            var entity = result.Results.First();

            var response = new FullOrderVM
            {
                Events = (await GetMessages(_cloudQueues.GetQueueReference(id.ToString()))).ToList(),
                Fruit = entity.PartitionKey,
                OrderId = entity.RowKey,
                Quantity = entity.Properties["Quantity"].DoubleValue ?? 0,
                PendingQuantity = entity.Properties["PendingQuantity"].DoubleValue ?? 0
            };

            return response;
            
        }

        public async Task<IEnumerable<object>> GetMessages(CloudQueue queue)
        {
            var messages = await queue.PeekMessagesAsync(10);
            return messages.Select(message => JsonConvert.DeserializeObject(message.AsString));
        }

        public async Task UpdateOrderWithPhone(string phone)
        {
            var orders = await FindOrderIdWithPhone(phone);
            var queue = _cloudQueues.GetQueueReference(orders.orderID.ToString());
            var transaction = new QueueModel
            {
                Action = ACTIONS.UPDATED,
                TransactionID = Guid.NewGuid(),
                ChangeQuantity = Convert.ToSingle(orders.quantity)
            };

            var order = await GetOrder(orders.orderID);
            var pendingValue = Convert.ToSingle(order.Properties["PendingQuantity"].DoubleValue);
            transaction.PrevQuantity = pendingValue;

            var calculation = pendingValue - orders.quantity;

            transaction.PostQuantity = Convert.ToSingle(calculation);

            order.Properties["PendingQuantity"] = new EntityProperty(calculation);

            if(await Update(order))
            {
                await RemoveMessage(orders.orderID, phone);
                await SaveTransaction(orders.orderID, transaction);
            }
        }

        private async Task RemoveMessage(Guid orderID, string phone)
        {
            var op = TableOperation.Retrieve<DynamicTableEntity>(orderID.ToString(), phone);
            var result = await messageSent.ExecuteAsync(op);

            var entity = result.Result as DynamicTableEntity;

            var deleteOp = TableOperation.Delete(entity);
            await messageSent.ExecuteAsync(deleteOp);
        }

        private async Task<bool> Update(DynamicTableEntity entity)
        {
            entity.ETag = "*";
            var op = TableOperation.InsertOrReplace(entity);
            var result = await orderTable.ExecuteAsync(op);

            return result.HttpStatusCode == 204;
        }

        private async Task<DynamicTableEntity> GetOrder(Guid orderID)
        {
            var query = new TableQuery<DynamicTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, orderID.ToString()));

            var results = await orderTable.ExecuteQuerySegmentedAsync(query, null);
            return results.Results.First();
        }

        private async Task<(Guid orderID, double quantity)> FindOrderIdWithPhone(string phone)
        {
            var query = new TableQuery<DynamicTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, phone));

            var results = await messageSent.ExecuteQuerySegmentedAsync(query, null);

            return (Guid.Parse(results.Results.First().PartitionKey), results.Results.First().Properties["Quantity"].DoubleValue ?? 0);

            throw new NotImplementedException();
        }
    }
}
