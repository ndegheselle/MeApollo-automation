﻿using Automation.Dal;
using Automation.Dal.Models;
using Automation.Dal.Repositories;
using Automation.Plugins.Shared;
using Automation.Worker.Control.Flow;
using MongoDB.Driver;

namespace Automation.Worker.Executor
{
    public class WorkflowExecutor
    {
        private readonly TasksRepository _tasksRepo;

        private readonly ITaskExecutor _executor;

        public WorkflowExecutor(DatabaseConnection connection, ITaskExecutor executor)
        {
            _tasksRepo = new TasksRepository(connection);
            _executor = executor;
        }

        public async Task<TaskInstance> ExecuteWorkflowAsync(TaskInstance instance, IProgress<TaskInstanceNotification>? progress = null)
        {
            BaseAutomationTask baseTask = await _tasksRepo.GetByIdAsync(instance.TaskId);
            if (baseTask is not AutomationWorkflow workflow)
                throw new Exception("Task is not a valid automation task.");

            workflow.Graph.Refresh();
            var startNode = workflow.Graph.Nodes.OfType<GraphControl>().Single(x => x.TaskId == StartTask.AutomationTask.Id);

            instance.StartedAt = DateTime.UtcNow;
            // We don't need to execute the start node since it doesn't have any logic
            await ExecuteNextAsync(startNode, workflow.Graph, instance);
            instance.FinishedAt = DateTime.UtcNow;
            instance.State = EnumTaskState.Completed;
            return instance;
        }

        public async Task ExecuteNodeAsync(GraphTask node, Graph graph, TaskInstance workflowInstance)
        {
            SubTaskInstance instance = new SubTaskInstance(workflowInstance.Id, node, new TaskParameters("", ""));

            switch (node)
            {
                case GraphWorkflow:
                    await ExecuteWorkflowAsync(instance, null);
                    break;
                case GraphTask:
                    await _executor.ExecuteAsync(instance, null);
                    break;
            }

            if (instance.State == EnumTaskState.Completed)
                await ExecuteNextAsync(node, graph, workflowInstance);
        }

        public async Task ExecuteNextAsync(GraphTask node, Graph graph, TaskInstance workflowInstance)
        {
            var nextConnections = graph.Connections.Where(x => x.Source?.Parent == node);
            foreach (var connection in nextConnections)
            {
                if (connection.Target?.Parent == null)
                    throw new Exception($"Could not resolve connection target node ['{connection.SourceId}' <-> '{connection.TargetId}'].");

                // TODO : pass next instance to the next task
                // TODO : handle task state and errors (if any) to stop the workflow execution if needed
                // TODO : handle task outputs and pass them to the next task if needed

                await ExecuteNodeAsync(connection.Target.Parent, graph, workflowInstance);
            }
        }
    }
}
