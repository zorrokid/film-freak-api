using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace FilmFreakApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase 
{
    private readonly TodoContext _context;

    public TodoController(TodoContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItem todoItem)
    {
        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();
        // Returns 201 with location header and todo-item in response body 
        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, ItemToDTO(todoItem));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
    {
        return await _context.TodoItems
            .Select(i => ItemToDTO(i))
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null) return NotFound();
        return ItemToDTO(todoItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
    {
        if (id != todoItem.Id)
        {
            return BadRequest();
        }
        if (!_context.TodoItems.Any(i => i.Id == id))
        {
            return NotFound();
        }
    
        _context.Entry(todoItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }
        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private TodoItemDTO ItemToDTO(TodoItem todoItem) => new TodoItemDTO
    {
        Id = todoItem.Id,
        IsComplete = todoItem.IsComplete,
        Name = todoItem.Name
    };
}