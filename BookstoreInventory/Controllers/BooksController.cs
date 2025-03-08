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
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IValidator<CreateBookDto> _bookValidator;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, IValidator<CreateBookDto> bookValidator, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _bookValidator = bookValidator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookRepository.GetAllAsync();

            var bookDtos = _mapper.Map<List<BookDto>>(books);
            return Ok(bookDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) new NotFoundException($"Buku dengan Id {id} tidak ditemukan.");
            
            var bookDtos = _mapper.Map<BookDto>(book);
            return Ok(bookDtos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookDto bookDto)
        {
            //Checking Validation
            var bookValidator = await _bookValidator.ValidateAsync(bookDto);
            if ((!bookValidator.IsValid))
            {
                throw new ValidationException(bookValidator.Errors);
            }

            var book = new Book
            {
                Title = bookDto.Title,
                AuthorId = bookDto.AuthorId
            };

            await _bookRepository.AddAsync(book);
            return CreatedAtAction(nameof(GetById), new {id = book.Id}, book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CreateBookDto bookDto)
        {
            //Checking Validation
            var bookValidator = await _bookValidator.ValidateAsync(bookDto);
            if ((!bookValidator.IsValid))
            {
                throw new ValidationException(bookValidator.Errors);
            }

            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) new NotFoundException($"Buku dengan Id {id} tidak ditemukan.");

            book.Title = bookDto.Title;
            await _bookRepository.UpdateAsync(book);
            
            return Ok(book);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) new NotFoundException($"Buku dengan Id {id} tidak ditemukan.");

            await _bookRepository.DeleteAsync(book);
            return Ok(id);
        }

    }
}
