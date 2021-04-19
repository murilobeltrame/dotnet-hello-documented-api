using Documented.Api.Data;
using Documented.Api.V2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documented.Api.V2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ApplicationContext context;

        public TodosController(ApplicationContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Lista as tarefas
        /// </summary>
        /// <remarks>
        /// Lista todas as tarefas conforme os parâmetros especificados.<br />
        /// Caso `_offset` não seja definido, o valor **0** usado como padrão.<br />
        /// Caso `_limit` não seja definido, o valor **10** é usado como padrão. O valor máximo para esse parâmetro é *255*.<br />
        /// A ordenação padrão usada é por `DueDate`.<br/>
        /// O número total de registros da pesquisa é retornado no cabeçalho `X-Total-Count`.
        /// </remarks>
        /// <param name="request">Condigurações da pesquisa</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TodoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List([FromQuery] ListRequest request)
        {
            var result = context.Todos.OrderBy(o => o.DueDate);

            var skip = 0;
            var take = (short)10;

            if (request != null)
            {
                if (request.Offset.HasValue) skip = request.Offset.Value;
                if (request.Limit.HasValue) take = request.Limit.Value;
            }
            Response.Headers.Add("X-Total-Count", (await result.CountAsync()).ToString());
            return Ok(await result
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync());
        }


        /// <summary>
        /// Busca uma tarefa
        /// </summary>
        /// <param name="id">Código da tarefa desejada.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await context.Todos.FirstOrDefaultAsync(w => w.Id == id);
            if (result == null) return NotFound(new ProblemDetail
            {
                ErrorCode = -1,
                ErrorMessage = "Cannot found",
                TraceId = Guid.NewGuid()
            });
            return Ok(result);
        }


        /// <summary>
        /// Cria uma tarefa
        /// </summary>
        /// <param name="newTodo">Tarefa a ser criada</param>
        [HttpPost]
        [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(TodoCreateRequest newTodo)
        {
            if (newTodo == null || !ModelState.IsValid) return BadRequest(new ProblemDetail
            {
                ErrorCode = -2,
                ErrorMessage = "Payload is invalid",
                TraceId = Guid.NewGuid()
            });

            var now = DateTime.Now;
            var createdTodoEntity = await context.Todos.AddAsync(new Todo
            {
                CreatedAt = now,
                CurrentStatus = Status.NEW,
                Description = newTodo.Description,
                DueDate = newTodo.DueDate,
                UpdatedAt = now
            });
            await context.SaveChangesAsync();
            return Created($"{HttpContext.Request.Path}/{createdTodoEntity.Entity.Id}", new { id = createdTodoEntity.Entity.Id });
        }

        /// <summary>
        /// Atualiza uma tarefa
        /// </summary>
        /// <remarks>
        /// Esse método deve ser usado também para alteração de estado da tarefa.
        /// </remarks>
        /// <param name="id">Código da tarefa a ser atualizada</param>
        /// <param name="updatedTodo">Dados a serem atualizados na tarefa</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, TodoUpdateRequest updatedTodo)
        {
            if (updatedTodo == null || !ModelState.IsValid) return BadRequest(new ProblemDetail
            {
                ErrorCode = -2,
                ErrorMessage = "Payload is invalid",
                TraceId = Guid.NewGuid()
            });

            var updatingTodo = await context.Todos.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id);

            if (updatingTodo == null) return NotFound(new ProblemDetail
            {
                ErrorCode = -3,
                ErrorMessage = "Trying to write inexisting item",
                TraceId = Guid.NewGuid()
            });

            context.Todos.Update(new Todo
            {
                CreatedAt = updatingTodo.CreatedAt,
                CurrentStatus = updatedTodo.CurrentStatus,
                Description = updatedTodo.Description,
                DueDate = updatedTodo.DueDate,
                Id = updatingTodo.Id,
                UpdatedAt = DateTime.Now
            });
            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Apaga uma tarefa
        /// </summary>
        /// <param name="id">Código da tarefa a ser apagada.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetail), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletingTodo = await context.Todos.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id);

            if (deletingTodo == null) return NotFound(new ProblemDetail
            {
                ErrorCode = -3,
                ErrorMessage = "Trying to write inexisting item",
                TraceId = Guid.NewGuid()
            });

            context.Todos.Remove(deletingTodo);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
