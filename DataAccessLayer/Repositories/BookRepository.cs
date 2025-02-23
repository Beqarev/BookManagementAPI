using BookManagementAPI.DAL.Domain;
using BookManagementAPI.DAL.Repositories;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories;

public class BookRepository : IBookRepository
{
    private readonly DataContext _context;

    public BookRepository(DataContext context)
    {
        _context = context;
    }
    
    public async Task<Book?> GetBookById(int id)
    {
        var book =  await _context.Books.FirstOrDefaultAsync(x => x.Id == id && x.DeleteDate == null);
        
        return book;
    }
    public async Task<List<Book>> GetBooksByIdsAsync(List<int> ids)
    {
        return await _context.Books
            .Where(b => ids.Contains(b.Id) && b.DeleteDate == null)
            .ToListAsync();
    }

    public IQueryable<Book> GetBooksQuery()
    {
        return _context.Books.AsQueryable();
    }
    
    public async Task CreateBook(Book book)
    {
        await _context.Books.AddAsync(book);
    }
    
    public async Task CreateBooksInBulk(List<Book> books)
    {
        await _context.Books.AddRangeAsync(books);
    }

    public void UpdateBook(Book book)
    {
        _context.Books.Update(book);
    }
    
    public void UpdateBooks(List<Book> books)
    {
        _context.Books.UpdateRange(books);
    }
    
    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
}