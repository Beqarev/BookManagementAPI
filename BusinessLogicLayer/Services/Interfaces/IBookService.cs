using BookManagementAPI.BLL.Common;
using BookManagementAPI.BLL.Models.Dtos;
using BookManagementAPI.DAL.Domain;

namespace BookManagementAPI.BLL.Services.Interfaces;

public interface IBookService
{
    Task<GetBookDto> GetBookById(int id);
    Task<Pagination<GetBookTitleDto>> GetBooks(int pageIndex = 1, int pageSize = 10);
    Task<List<int>> AddBooksAsync(List<CreateBookRequest> books);
    Task<int> AddBookAsync(CreateBookRequest book);
    Task UpdateAsync(UpdateBookRequest book);
    Task<bool> DeleteBook(int id);
    Task<(int DeletedCount, List<int> NotFoundIds)> DeleteBooksAsync(List<int> ids);
}