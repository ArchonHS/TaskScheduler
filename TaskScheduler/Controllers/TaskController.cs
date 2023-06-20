using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskScheduler.Data;
using TaskScheduler.Data.Interfaces;
using TaskScheduler.Models;
using TaskScheduler.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskScheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[ApiKey]
    //[Authorize]
    public class TaskController : ControllerBase
    {
        private readonly IDapperService<TaskFromApiDTO> _db;
        private readonly IMapper _mapper;
        private readonly ISchedulerService _scheduler;
        public TaskController(IMapper mapper, ISchedulerService scheduler, IDapperService<TaskFromApiDTO> db)
        {
            _mapper = mapper;
            _scheduler = scheduler;
            _db = db;
        }
        // GET: api/<TaskController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet]        
        public async Task<ActionResult<List<TaskFromApiDTO>>> Get()
        {
            var tasks = await _db.GetAll();
            if (tasks == null) return NotFound();
            return Ok(tasks);
        }

        // GET api/<TaskController>/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskFromApiDTO>> Get(Guid id)
        {
            var task = await _db.Get(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        // POST api/<TaskController>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TaskFromApiCreateDTO taskCreate)
        {
            if (taskCreate == null) return BadRequest();
            try
            {
                var id = await _db.Add(_mapper.Map<TaskFromApiDTO>(taskCreate));
                await _scheduler.AddTask(await _db.Get(id));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            return Ok();
        }

        // PUT api/<TaskController>/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] TaskFromApiUpdateDTO taskUpdate)
        {
            if (taskUpdate == null) return BadRequest();
            var updatedTask = _mapper.Map<TaskFromApiDTO>(taskUpdate);
            updatedTask.UidTask = id;
            await _db.Update(updatedTask);
            await _scheduler.ReloadTask(id.ToString());
            return Ok();
        }

        // DELETE api/<TaskController>/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces("application/json")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _scheduler.RemoveTask(id.ToString());
            var deletedTask = await _db.Get(id);
            deletedTask.Deleted = true;
            deletedTask.IsActive = false;
            await _db.Update(deletedTask);
            return NoContent();
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("force/{id}")]
        public async Task<IActionResult> ForceExecute(Guid id)
        {
            var task = await _db.Get(id);
            if (task == null) return NotFound();
            await task.ExecuteTask();
            return Ok();
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [HttpGet("reload")]
        public async Task<IActionResult> Reload()
        {
            await _scheduler.ReloadTasks();
            return Ok();
        }
    }
}
