using AutoMapper;
using BookstoreInventory.DTOs;
using BookstoreInventory.Models;

namespace BookstoreInventory.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            //Author -> AuthorDTO
            CreateMap<Author, AuthorDto>()
                .ForMember(dest => dest.BookIds, opt => opt.MapFrom(src => src.Books.Select(b => b.Id).ToList()));

            //Book -> BookDTO
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => new AuthorDto
                {
                    Id = src.Author.Id,
                    Name = src.Author.Name,
                    BookIds = src.Author.Books.Select(b => b.Id).ToList()
                }));
        }
    }
}
