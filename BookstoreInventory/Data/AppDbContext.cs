using BookstoreInventory.Models;
using Microsoft.EntityFrameworkCore;
using Faker;

namespace BookstoreInventory.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IHostEnvironment _env;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHostEnvironment env) : base(options) { 
            _env = env;
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
               .Property(b => b.Id)
               .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Author>()
               .Property(a => a.Id)
               .HasDefaultValueSql("NEWID()");

            if (_env.IsDevelopment())
            {
                //Seed fake data
                var authors = new List<Author>() {
                    new Author { Id = Guid.Parse("c9a1b5f2-ef24-4cf6-91d4-9b034b650b3e"), Name = "John Doe" },
                    new Author { Id = Guid.Parse("dc38f913-7f1c-4cf2-a294-2a36c4f47e62"), Name = "Jane Smith" },
                    new Author { Id = Guid.Parse("a2f82c1b-3285-40fa-92f8-5dc6c0aaf9be"), Name = "Alice Johnson" },
                    new Author { Id = Guid.Parse("b76371a8-8792-447b-a1b5-3b2a644d5f9f"), Name = "Michael Brown" },
                    new Author { Id = Guid.Parse("908c0d73-9b13-4d82-8e26-f9cb40930e5f"), Name = "Michael Jordan" }
                };

                modelBuilder.Entity<Author>().HasData(authors);
            }

            modelBuilder
                .Entity<Author>()
                .HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId);
        }
    }
}
