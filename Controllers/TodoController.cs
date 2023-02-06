using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

namespace FilmFreakApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase 
{
    private readonly TodoContext _context;

    public TodoController(TodoContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
    {
        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(PostTodoItem), new { id = todoItem.Id }, todoItem);
    }
}