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
        private readonly ISmsService _smsService;
        private readonly IProviderService _providerService;
        private readonly CloudQueueClient _cloudQueues;
        CloudTable orderTable;
        CloudTable MessagesSent;

        public OrdersService(
            IOptions<TableConfig> options,
            IProviderService providerService,
            ISmsService smsService)
        {
            _smsService = smsService;
            var account = CloudStorageAccount.Parse(options.Value.ConnectionString);
            var cloudTable = account.CreateCloudTableClient();
            _cloudQueues = account.CreateCloudQueueClient();

            orderTable = cloudTable.GetTableReference("orders");
            orderTable.CreateIfNotExistsAsync();

            MessagesSent = cloudTable.GetTableReference("Messages");
            MessagesSent.CreateIfNotExistsAsync();
        }

        public async Task<bool> Create(OrderCreateVM orderCreateVM)
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
                var optimized = OptimizeDistance(providers, orderCreateVM.Latitude, orderCreateVM.Longitude).Take(4);
                

            }

            return false;
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
        public Task<bool> Get() => throw new NotImplementedException();
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
    }
}
