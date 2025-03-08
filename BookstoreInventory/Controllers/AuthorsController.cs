using AutoMapper;
using BookstoreInventory.DTOs;
using BookstoreInventory.Models;
using BookstoreInventory.Repositories;
using BookstoreInventory.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreInventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IValidator<CreateAuthorDto> _validator;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, IValidator<CreateAuthorDto> validator, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _validator = validator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var pagedAuthors = await _authorRepository.GetAllAsync(page, pageSize);

            var authorDtos = _mapper.Map<List<AuthorDto>>(pagedAuthors.Data);

            var result = new PagedResultDto<AuthorDto>
            {
                Data = authorDtos,
                CurrentPage = pagedAuthors.CurrentPage,
                PageSize = pagedAuthors.PageSize,
                TotalItems = pagedAuthors.TotalItems,
                TotalPages = pagedAuthors.TotalPages
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null)  new NotFoundException($"Author dengan Id {id} tidak ditemukan.");

            var authorDto = _mapper.Map<AuthorDto>(author);
            return Ok(authorDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAuthorDto authorDto)
        {
            //Checking Validation
            var authorValidator = await _validator.ValidateAsync(authorDto);
            if (!authorValidator.IsValid)
            { 
                throw new ValidationException(authorValidator.Errors);
            }

            var author = new Author
            {
                Id = Guid.NewGuid(),
                Name = authorDto.Name
            };

            await _authorRepository.AddAsync(author);
            return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CreateAuthorDto authorDto)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) throw new NotFoundException($"Author dengan Id {id} tidak ditemukan.");

            //Checking Validation
            var authorValidator = await _validator.ValidateAsync(authorDto);
            if (!authorValidator.IsValid)
            {
                throw new ValidationException(authorValidator.Errors);
            }

            author.Name = authorDto.Name;
            await _authorRepository.UpdateAsync(author);
            return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) throw new NotFoundException($"Author dengan Id {id} tidak ditemukan.");

            await _authorRepository.DeleteAsync(author);
            return Ok(id);
        }
    }
}
