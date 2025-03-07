using BookstoreInventory.Models;
using BookstoreInventory.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace BookstoreInventory.Caching
{
    public class CachedBookService : IBookRepository
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public CachedBookService(IBookRepository bookRepository, IMemoryCache memoryCache)
        {
            _bookRepository = bookRepository;
            _cache = memoryCache;
        }

        public Task AddAsync(Book book) => _bookRepository.AddAsync(book);

        public Task DeleteAsync(Book book) => _bookRepository.DeleteAsync(book);

        public Task<IEnumerable<Book>> GetAllAsync() => _bookRepository.GetAllAsync();

        public async Task<Book> GetByIdAsync(Guid id)
        {
            var cacheKey = $"book_{id}";

            if (!_cache.TryGetValue(cacheKey, out Book book)) { 
                book = await _bookRepository.GetByIdAsync(id);
                if (book != null) {
                    _cache.Set(cacheKey, book, _cacheDuration);
                }
            }

            return book;
        }

        public Task UpdateAsync(Book book) => _bookRepository.UpdateAsync(book);
    }
}
