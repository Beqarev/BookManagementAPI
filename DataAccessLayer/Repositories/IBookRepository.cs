using BookManagementAPI.DAL.Domain;

namespace BookManagementAPI.DAL.Repositories;

public interface IBookRepository
{
    Task<Book?> GetBookById(int id);
    Task<List<Book>> GetBooksByIdsAsync(List<int> ids);
    IQueryable<Book> GetBooksQuery();
    Task CreateBook(Book book);
    Task CreateBooksInBulk(List<Book> books);
    void UpdateBook(Book book);
    void UpdateBooks(List<Book> books);
    Task<int> SaveAsync();
}