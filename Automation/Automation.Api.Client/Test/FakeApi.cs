﻿using Automation.Shared.Contracts;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading.Tasks;
using Automation.Shared;
using System.ComponentModel.DataAnnotations;

namespace Automation.Api.Client.Test
{
    public class FakeApi
    {
        public string Serialize(object data)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(data, options);
        }

        private TaskNode? LoadTask(Guid id)
        {
            TaskNode? task = TestDataFactory.Data.Tasks.FirstOrDefault(x => x.Id == id);

            if (task == null)
                return task;

            // For each inputs set the parent node
            foreach (TaskConnector connector in TestDataFactory.Data.Connectors
                .Where(x => x.ParentId == task.Id)
                .OrderByDescending(x => x.Type))
            {
                task.Connectors.Add(connector);
            }
            return task;
        }

        public string GetTask(Guid id) { return Serialize(LoadTask(id)); }

        public string GetWorkflow(Guid id)
        {
            TaskNode? task = LoadTask(id);

            if (task is not WorkflowNode workflow)
                return string.Empty;

            // Get the nodes
            workflow.Connections = TestDataFactory.Data.Connections.Where(x => x.ParentId == workflow.Id).ToList();
            foreach (var relation in TestDataFactory.Data.WorkflowRelations.Where(x => x.WorkflowId == workflow.Id))
            {
                if (!workflow.Relations.ContainsKey(relation.TaskId))
                {
                    var workflowTask = LoadTask(relation.TaskId);
                    workflow.Tasks.Add(workflowTask);
                    workflow.Relations.Add(relation.TaskId, relation);
                }
            }

            return Serialize(workflow);
        }

        public string GetRootScope(ScopeLoadOptions options = default)
        {
            return GetScope(Guid.Parse("00000000-0000-0000-0000-000000000001"), options);
        }

        public string GetScope(Guid id, ScopeLoadOptions options = default)
        {
            Scope? scope = TestDataFactory.Data.Scopes.FirstOrDefault(x => x.Id == id);
            if (scope == null)
                return string.Empty;

            if (!options.WithContext)
                scope.Context = new Dictionary<string, string>();

            if (!options.WithChildrens)
                return Serialize(scope);

            foreach (Scope child in TestDataFactory.Data.Scopes.Where(x => x.ParentId == scope.Id).OrderBy(x => x.Name))
            {
                if (!options.WithContext)
                    child.Context = new Dictionary<string, string>();
                scope.SubScope.Add(child);
            }

            foreach (TaskNode child in TestDataFactory.Data.Tasks.Where(x => x.ScopeId == scope.Id))
            {
                scope.Childrens.Add(child);
            }

            return Serialize(scope);
        }

        public string GetTaskHistory(Guid id, int page, int pageSize)
        {
            List<TaskHistory> testList = new List<TaskHistory>();
            for (int i = page * pageSize; i < page * pageSize + pageSize; i++)
            {
                var testInstance = new TaskHistory()
                {
                    Id = Guid.NewGuid(),
                    TaskId = id,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddSeconds(10)
                };
                testList.Add(testInstance);
                testInstance.Status = (EnumInstanceStatus)(i % 5);
            }

            return Serialize(new TaskHistories()
            {
                Data = testList,
                Total = 100000,
                Page = page,
                PageSize = pageSize
            });
        }

        public string GetScopeHistory(Guid id, int page, int pageSize)
        {
            return GetTaskHistory(id, page, pageSize);
        }
    }
}
