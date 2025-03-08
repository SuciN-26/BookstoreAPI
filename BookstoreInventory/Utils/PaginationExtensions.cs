using BookstoreInventory.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BookstoreInventory.Utils
{
    public static class PaginationExtensions
    {
        public static async Task<PagedResultDto<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query, int page, int pageSize)
        {
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<T>
            {
                Data = data,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }

}
