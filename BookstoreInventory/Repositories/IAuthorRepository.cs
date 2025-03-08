using BookstoreInventory.DTOs;
using BookstoreInventory.Models;

namespace BookstoreInventory.Repositories
{
    public interface IAuthorRepository
    {
        Task<PagedResultDto<Author>> GetAllAsync(int page, int pageSize);
        Task<Author> GetByIdAsync(Guid id);
        Task AddAsync(Author author);
        Task UpdateAsync(Author author);
        Task DeleteAsync(Author author);
    }
}
