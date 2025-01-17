using System.Linq;
using AutoMapper;
using dorotec_backend_test.Classes.DTOs;
using dorotec_backend_test.Classes.Exceptions;
using dorotec_backend_test.Classes.Pagination;
using dorotec_backend_test.Interfaces;
using dorotec_backend_test.Models;
using dorotec_backend_test.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace dorotec_backend_test.Services;

public class BookService : IBookService
{
    private readonly BookstoreDbContext _context;
    private readonly IMapper _mapper;
    public BookService(BookstoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BookDTO> Create(BookDTO dto)
    {
        Book book = _mapper.Map<Book>(dto);

        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        return _mapper.Map<BookDTO>(book);
    }

    public async Task DeleteOne(int id)
    {
        Book? book = await _context.Books.FindAsync(id);

        if (book is null) throw new ResourceNotFoundException();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }

    public async Task<BookDTO> GetOne(int id)
    {
        Book? book = await _context.Books.FindAsync(id);

        if (book is null) throw new ResourceNotFoundException();

        return _mapper.Map<BookDTO>(book);
    }

    public async Task<PageResult<BookDTO>> GetPage(int index, byte size)
    {
        PageFilter filter = new PageFilter(index, size);

        IQueryable<Book> query = _context.Books
            .IgnoreAutoIncludes();

        long count = await query.CountAsync();

        if (count < 1) throw new ResourceNotFoundException();

        List<BookDTO> books = await query
            .OrderBy(book => book.Name)
            .Skip(filter.Skip)
            .Take(filter.Take)
            .Select(book => _mapper.Map<BookDTO>(book))
            .ToListAsync();

        return new PageResult<BookDTO>(books, filter, count);
    }

    public async Task<PageResult<BookDTO>> GetPage(BookFilterDTO bookFilter)
    {
        PageFilter filter = new PageFilter(bookFilter.Index, bookFilter.Size);

        IQueryable<Book> query = _context.Books
            .IgnoreAutoIncludes();

        query = query.WhereFilterMatches(bookFilter);

        long count = await query.CountAsync();

        if (count < 1) throw new ResourceNotFoundException();

        List<BookDTO> books = await query
            .OrderBy(book => book.Name)
            .Skip(filter.Skip)
            .Take(filter.Take)
            .Select(book => _mapper.Map<BookDTO>(book))
            .ToListAsync();

        return new PageResult<BookDTO>(books, filter, count);
    }

    public async Task<BookDTO> UpdateOne(int id, BookDTO dto)
    {
        Book? book = await _context.Books.FindAsync(id);

        if (book is null) throw new ResourceNotFoundException();

        _mapper.Map(dto, book);

        _context.Books.Update(book);
        await _context.SaveChangesAsync();

        return _mapper.Map<BookDTO>(book);
    }
}
