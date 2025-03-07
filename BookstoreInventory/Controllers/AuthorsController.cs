using BookstoreInventory.DTOs;
using BookstoreInventory.Models;
using BookstoreInventory.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreInventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _authorRepository.GetAllAsync();

            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if(author == null) return NotFound();
            return Ok(author);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAuthorDto authorDto)
        {
            var author = new Author
            {
                Name = authorDto.Name
            };

            await _authorRepository.AddAsync(author);
            return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CreateAuthorDto authorDto)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) return NotFound();

            author.Name = authorDto.Name;
            await _authorRepository.UpdateAsync(author);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) return NotFound();

            await _authorRepository.DeleteAsync(author);
            return NoContent();
        }
    }
}
