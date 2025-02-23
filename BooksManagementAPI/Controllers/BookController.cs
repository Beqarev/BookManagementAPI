using BookManagementAPI.BLL.Models.Dtos;
using BookManagementAPI.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksManagementAPI.API.Controllers;

[Authorize]
[Route("api/books")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        if (pageIndex < 1)
            return BadRequest("Page index must be at least 1");
        
        if (pageSize < 1 || pageSize > 50)
            return BadRequest("Page size must be between 1 and 50");

        var result = await _bookService.GetBooks(pageIndex, pageSize);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(int id)
    {
        var result = await _bookService.GetBookById(id);
        if (result == null)
            return NotFound(new { error = $"Book with ID {id} not found" });
        
        return Ok(result);
    }


    [HttpPost("bulk")]
    public async Task<IActionResult> AddBooks([FromBody] List<CreateBookRequest> requests)
    {
        if (!requests.Any())
            return BadRequest(new { error = "No books provided" });
        
        try 
        {
            var ids = await _bookService.AddBooksAsync(requests);
            return Created(string.Empty, ids);
        }
        catch (Exception ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] CreateBookRequest request)
    {
        try
        {
            var id = await _bookService.AddBookAsync(request);
            var book = await _bookService.GetBookById(id);
            return CreatedAtAction(nameof(GetBook), new { id }, book);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while adding", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookRequest book)
    {
        if (id != book.Id)
            return BadRequest(new { error = "Mismatched book ID in request" });

        try
        {
            await _bookService.UpdateAsync(book);
            return Ok(await _bookService.GetBookById(id));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while updating", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try 
        {
            await _bookService.DeleteBook(id);
            return NoContent();
        }
        catch (Exception)
        {
            return NotFound(new { error = $"Book with ID {id} not found" });
        }
    }
    
    [HttpDelete("bulk")]
    public async Task<IActionResult> DeleteBooks([FromBody] List<int> ids)
    {
        if (!ids.Any())
            return BadRequest(new { error = "No IDs provided" });

        var (deletedCount, notFoundIds) = await _bookService.DeleteBooksAsync(ids);
        
        if (deletedCount == 0)
            return NotFound(new { error = "None of the specified books were found" });

        return Ok(new
        {
            deletedCount,
            notFoundIds,
            message = $"Successfully deleted {deletedCount} books" + 
                      (notFoundIds.Any() ? $". {notFoundIds.Count} books were not found." : "")
        });
    }
}