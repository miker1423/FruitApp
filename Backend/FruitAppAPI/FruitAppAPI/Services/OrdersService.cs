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
        private readonly IProviderService _providerService;
        private readonly CloudTableClient _cloudTables;
        private readonly CloudQueueClient _cloudQueues;

        public OrdersService(
            IOptions<TableConfig> options,
            IProviderService providerService)
        {
            var account = CloudStorageAccount.Parse(options.Value.ConnectionString);
            _cloudTables = account.CreateCloudTableClient();
            _cloudQueues = account.CreateCloudQueueClient();
        }

        public async Task<bool> Create(OrderCreateVM orderCreateVM)
        {
            var orderID = Guid.NewGuid();
            var queue = _cloudQueues.GetQueueReference(orderID.ToString());

            if(await queue.CreateIfNotExistsAsync())
            {
                var transactionID = Guid.NewGuid();

                var createdAction = new QueueModel
                {
                    Action = ACTIONS.CREATED,
                    PrevQuantity = 0f,
                    ChangeQuantity = orderCreateVM.Quantity,
                    PostQuantity = orderCreateVM.Quantity,
                    TransactionID = transactionID
                };

                var message = new CloudQueueMessage(JsonConvert.SerializeObject(createdAction));
                await queue.AddMessageAsync(message, TimeSpan.MaxValue, TimeSpan.Zero, null, null);
            }
        }

        public Task<bool> Delete() => throw new NotImplementedException();
        public Task<bool> Get() => throw new NotImplementedException();
        public Task<bool> Update() => throw new NotImplementedException();

        private string SanitizeGuid(Guid guid) => guid.ToString().Replace("-", "");
    }
}
