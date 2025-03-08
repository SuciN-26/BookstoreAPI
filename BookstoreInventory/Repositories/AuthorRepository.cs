using BookstoreInventory.Data;
using BookstoreInventory.DTOs;
using BookstoreInventory.Models;
using BookstoreInventory.Utils;
using Microsoft.EntityFrameworkCore;

namespace BookstoreInventory.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _context;

        public AuthorRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task AddAsync(Author author)
        {
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Author author)
        {
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResultDto<Author>> GetAllAsync(int page, int pageSize)
        {
            return await _context.Authors
                        .Include(a => a.Books)
                        .OrderBy(a => a.Name)
                        .AsQueryable()
                        .ToPagedResultAsync(page, pageSize);
        }

        public async Task<Author> GetByIdAsync(Guid id)
        {
            return await _context.Authors.FindAsync(id);
        }

        public async Task UpdateAsync(Author author)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
        }
    }
}
