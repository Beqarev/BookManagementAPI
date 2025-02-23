using AutoMapper;
using BookManagementAPI.BLL.Common;
using BookManagementAPI.BLL.Models.Dtos;
using BookManagementAPI.BLL.Services.Interfaces;
using BookManagementAPI.DAL.Domain;
using BookManagementAPI.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookManagementAPI.BLL.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;


    public BookService(IBookRepository bookRepository, IMapper mapper)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
    }
    
    //addingbook bulk
    public async Task<List<int>> AddBooksAsync(List<CreateBookRequest> books)
    {
        var titles = books.Select(b => b.Title).ToList();
        var existingTitles = await _bookRepository.GetBooksQuery()
            .Where(b => titles.Contains(b.Title))
            .Select(b => b.Title)
            .ToListAsync();

        if (existingTitles.Any())
        {
            throw new Exception($"These books already exist: {string.Join(", ", existingTitles)}");
        }
        
        var mappedBooks = _mapper.Map<List<Book>>(books);

        await _bookRepository.CreateBooksInBulk(mappedBooks);
        await _bookRepository.SaveAsync(); 

        return mappedBooks.Select(b => b.Id).ToList();
    }

    //addingbook single
    public async Task<int> AddBookAsync(CreateBookRequest book)
    {
        bool exists = _bookRepository.GetBooksQuery().Any(b => b.Title == book.Title);

        if (exists)
        {
            throw new Exception($"Book {book.Title} already exists");
        }

        var mappedBook = _mapper.Map<Book>(book);

        await _bookRepository.CreateBook(mappedBook);
        await _bookRepository.SaveAsync();

        return mappedBook.Id;
    }
    
    //update
    public async Task UpdateAsync(UpdateBookRequest book)
    {
        var existingBook = await _bookRepository.GetBookById(book.Id);
    
        if (existingBook == null)
        {
            throw new Exception($"Book with ID {book.Id} not found");
        }
    
        _mapper.Map(book, existingBook);
        existingBook.UpdateTime = DateTime.Now;
    
        _bookRepository.UpdateBook(existingBook);
        await _bookRepository.SaveAsync();
    }
    
    //gets book by id
    public async Task<GetBookDto> GetBookById(int id)
    {
        var book = await _bookRepository.GetBookById(id);
        if (book == null)
            return null;
        
        var mappedBook = _mapper.Map<GetBookDto>(book);
        mappedBook.PopularityScore = mappedBook.Views * 0.5f + (DateTime.UtcNow.Year - book.PublicationYear) * 2;
        return mappedBook;
    }
    
    
    //get books
    public async Task<Pagination<GetBookTitleDto>> GetBooks(int pageIndex = 1, int pageSize = 10)
    {
        var query = _bookRepository.GetBooksQuery()
            .OrderByDescending(b => b.Views)
            .Select(b => new GetBookTitleDto() 
            { 
                Title = b.Title,
            });

        return await Pagination<GetBookTitleDto>.CreateAsync(query, pageIndex, pageSize);
    }
    
    
    //softDelete single
    public async Task<bool> DeleteBook(int id)
    {
        var book = await _bookRepository.GetBookById(id);
        if (book == null)
        {
            throw new Exception("Book not found!");
        }
        book.DeleteDate = DateTime.Now;

        _bookRepository.UpdateBook(book);
        await _bookRepository.SaveAsync();
        return true;
    }
    
    //softDelete bulk
    public async Task<(int DeletedCount, List<int> NotFoundIds)> DeleteBooksAsync(List<int> ids)
    {
        if (!ids.Any())
        {
            return (0, new List<int>());
        }

        var books = await _bookRepository.GetBooksByIdsAsync(ids);
        var foundIds = books.Select(b => b.Id).ToList();
        var notFoundIds = ids.Except(foundIds).ToList();

        if (books.Any())
        {
            foreach (var book in books)
            {
                book.DeleteDate = DateTime.Now;
            }

            _bookRepository.UpdateBooks(books);
            await _bookRepository.SaveAsync();
        }

        return (books.Count, notFoundIds);
    }
}