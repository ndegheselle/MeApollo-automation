﻿using Automation.App.Shared.ViewModels.Work;
using Automation.Shared.Clients;
using RestSharp;

namespace Automation.App.Shared.ApiClients
{
    public class TaskInstancesClient : BaseCrudClient<TaskInstance>
    {
        public TaskInstancesClient(RestClient restClient) : base(restClient, "instances")
        { }
    }
}
