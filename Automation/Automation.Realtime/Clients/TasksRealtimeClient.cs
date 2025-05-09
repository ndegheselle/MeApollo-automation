﻿using Automation.Plugins.Shared;
using Automation.Realtime.Base;
using Automation.Shared.Data;
using StackExchange.Redis;

namespace Automation.Realtime.Clients
{
    public class TasksRealtimeClient
    {
        public RedisPublisher<TaskProgress> Progress { get; private set; }
        public RedisPublisher<EnumTaskState> Lifecycle { get; private set; }

        public TasksRealtimeClient(ConnectionMultiplexer connection, Guid instanceId) { 
            Progress = new RedisPublisher<TaskProgress>(connection, $"tasks:instance:{instanceId}:progress");
            Lifecycle = new RedisPublisher<EnumTaskState>(connection, $"tasks:instance:{instanceId}:lifecycle");
        }
    }
}