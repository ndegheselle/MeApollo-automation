﻿using Automation.Dal.Repositories;
using Automation.Realtime;
using Automation.Realtime.Clients;
using Automation.Realtime.Models;
using MongoDB.Driver;

namespace Automation.Supervisor.Api.Business
{
    /// <summary>
    /// Clean the dead workers once in a while, ensure that a task is never hanged if a server crashed.
    /// </summary>
    public class WorkerCleaner : BackgroundService
    {
        private readonly TimeSpan _cleaningInterval = TimeSpan.FromSeconds(30);
        private readonly TaskIntancesRepository _repository;
        private readonly WorkersRealtimeClient _workersClient;
        private readonly WorkerAssignator _assignator;

        public WorkerCleaner(IMongoDatabase database, RedisConnectionManager redis)
        {
            _repository = new TaskIntancesRepository(database);
            _workersClient = new WorkersRealtimeClient(redis.Connection);
            _assignator = new WorkerAssignator(database, redis);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_cleaningInterval, stoppingToken);
                await _workersClient.CleanDeadWorkers();
                await CleanUnhandledTasks();
            }
        }

        /// <summary>
        /// Assign the dead workers tasks that are not finished to some other workers.
        /// </summary>
        /// <returns></returns>
        private async Task CleanUnhandledTasks()
        {
            IEnumerable<WorkerInstance> activeWorkers = await _workersClient.GetWorkersAsync();
            foreach (var task in await _repository.GetUnhandledAsync(activeWorkers.Select(x => x.Id)))
            {
                await _assignator.ReassignAsync(task);
            }
        }
    }
}
