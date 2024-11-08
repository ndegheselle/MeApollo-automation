using Automation.Api.Supervisor.Business;
using Automation.Dal.Models;
using Automation.Dal.Repositories;
using Automation.Plugins.Shared;
using Automation.Realtime;
using Automation.Shared.Base;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Automation.Api.Supervisor.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TasksController : Controller
    {
        private readonly TasksRepository _taskRepo;
        private readonly TaskIntancesRepository _taskInstanceRepo;
        private readonly IMongoDatabase _database;
        private readonly WorkerAssignator _assignator;

        public TasksController(IMongoDatabase database, RedisConnectionManager redis)
        {
            _database = database;
            _taskRepo = new TasksRepository(database);
            _taskInstanceRepo = new TaskIntancesRepository(database);
            _assignator = new WorkerAssignator(database, redis);
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Guid>> CreateAsync(TaskNode element)
        {
            if (element.ParentId == null)
            {
                return BadRequest(new Dictionary<string, string[]>()
                {
                    {nameof(TaskNode.Name), [$"A task cannot be created without a parent."] }
                });
            }

            var scopeRepository = new ScopesRepository(_database);
            var existingChild = await scopeRepository.GetDirectChildByNameAsync(element.ParentId, element.Name);
            
            if (existingChild != null)
            {
                return BadRequest(new Dictionary<string, string[]>()
                {
                    {nameof(TaskNode.Name), [$"The name {element.Name} is already used in this scope."] }
                });
            }

            var scope = await scopeRepository.GetByIdAsync(element.ParentId.Value);
            if (scope == null)
            {
                return BadRequest(new Dictionary<string, string[]>()
                {
                    {nameof(TaskNode.Name), [$"The parent id {element.ParentId} is invalid."] }
                });
            }

            element.ParentTree = [.. scope.ParentTree, scope.Id];
            return await _taskRepo.CreateAsync(element);
        }

        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync([FromRoute] Guid id)
        {
            return _taskRepo.DeleteAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<TaskNode?> GetByIdAsync([FromRoute] Guid id)
        {
            return await _taskRepo.GetByIdAsync(id);
        }

        [HttpPut]
        [Route("{id}")]
        public Task UpdateAsync([FromRoute] Guid id, [FromBody]TaskNode element)
        {
            return _taskRepo.UpdateAsync(id, element);
        }

        [HttpPost]
        [Route("{id}/execute")]
        public async Task<TaskInstance> ExecuteAsync([FromRoute] Guid id)
        {
            TaskNode? task = await _taskRepo.GetByIdAsync(id);

            if (task == null)
                throw new InvalidOperationException($"No task node found for the id '{id}'.");

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            // TODO : get the full task context from the body
            TaskContext context = new TaskContext()
            {
                Parameters = []
            };

            return await _assignator.AssignAsync(task, context);
        }

        [HttpGet]
        [Route("{id}/instances")]
        public async Task<ListPageWrapper<TaskInstance>> GetInstancesAsync([FromRoute] Guid id, [FromQuery] int page, [FromQuery] int pageSize)
        {
            return await _taskInstanceRepo.GetByTaskAsync(id, page, pageSize);
        }
    }
}
