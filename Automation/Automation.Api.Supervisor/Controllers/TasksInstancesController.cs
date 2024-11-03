using Automation.Api.Supervisor.Business;
using Automation.Dal.Models;
using Automation.Dal.Repositories;
using Automation.Plugins.Shared;
using Automation.Realtime;
using Automation.Shared.Base;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

namespace Automation.Api.Supervisor.Controllers
{
    [ApiController]
    [Route("instances")]
    public class TasksInstancesController : Controller
    {
        private readonly TaskIntanceRepository _taskInstanceRepo;

        public TasksInstancesController(IMongoDatabase database)
        {
            _taskInstanceRepo = new TaskIntanceRepository(database);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<TaskInstance?> GetByIdAsync([FromRoute] Guid id)
        {
            return await _taskInstanceRepo.GetByIdAsync(id);
        }
    }
}