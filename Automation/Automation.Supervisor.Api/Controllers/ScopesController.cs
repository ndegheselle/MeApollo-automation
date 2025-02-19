using Automation.Dal.Models;
using Automation.Dal.Repositories;
using Automation.Shared.Base;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Automation.Supervisor.Api.Controllers
{
    [ApiController]
    [Route("scopes")]
    public class ScopesController : BaseCrudController<Scope>
    {
        private ScopesRepository _repository => (ScopesRepository)_repository;
        private readonly TaskIntancesRepository _taskInstanceRepo;

        public ScopesController(IMongoDatabase database) : base(new ScopesRepository(database))
        {
            _taskInstanceRepo = new TaskIntancesRepository(database);
        }

        [HttpPost]
        [Route("")]
        public override async Task<ActionResult<Guid>> CreateAsync(Scope element)
        {
            if (element.ParentId == null)
            {
                return BadRequest(new Dictionary<string, string[]>()
                {
                    {nameof(AutomationTask.Name), [$"A scope cannot be created without a parent."] }
                });
            }

            var existingChild = await _repository.GetDirectChildByNameAsync(element.ParentId, element.Name);
            if (existingChild != null)
            {
                // XXX : if need more info can also use return ValidationProblem(new ValidationProblemDetails());
                return BadRequest(new Dictionary<string, string[]>()
                {
                    {nameof(Scope.Name), [$"The name {element.Name} is already used in this scope."] }
                });
            }

            var scope = await _repository.GetByIdAsync(element.ParentId.Value);
            if (scope == null)
            {
                return BadRequest(new Dictionary<string, string[]>()
                {
                    {nameof(AutomationTask.Name), [$"The parent id {element.ParentId} is invalid."] }
                });
            }

            element.ParentTree = [..scope.ParentTree, scope.Id];
            return await _repository.CreateAsync(element);
        }

        [HttpGet]
        [Route("root")]
        public async Task<Scope> GetRootAsync()
        {
            return await _repository.GetRootAsync();
        }

        [HttpGet]
        [Route("{id}/instances")]
        public async Task<ListPageWrapper<TaskInstance>> GetInstancesAsync([FromRoute] Guid id, [FromQuery] int page, [FromQuery] int pageSize)
        {
            return await _taskInstanceRepo.GetByScopeAsync(id, page, pageSize);
        }
    }
}
