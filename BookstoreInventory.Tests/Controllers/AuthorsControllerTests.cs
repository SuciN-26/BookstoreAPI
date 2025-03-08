using AutoMapper;
using BookstoreInventory.Controllers;
using BookstoreInventory.DTOs;
using BookstoreInventory.Models;
using BookstoreInventory.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreInventory.Tests.Controllers
{
    public class AuthorsControllerTests
    {
        private readonly Mock<IAuthorRepository> _authorRepoMock;
        private readonly Mock<IValidator<CreateAuthorDto>> _validatorMock;
        private readonly AuthorsController _controller;
        private readonly Mock<IMapper> _mapperMock;

        public AuthorsControllerTests()
        {
            _authorRepoMock = new Mock<IAuthorRepository>();
            _validatorMock = new Mock<IValidator<CreateAuthorDto>>();
            _mapperMock = new Mock<IMapper>();
            _controller = new AuthorsController(_authorRepoMock.Object, _validatorMock.Object, _mapperMock.Object);
        }

        // GET /api/authors (Mengambil Semua Penulis)
        [Fact]
        public async Task GetAll_ShouldReturnListOfAuthors()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 2;

            var author = new List<Author> {
                new Author { Id = Guid.NewGuid(), Name = "Author 1", Books = new List<Book>(){ new Book { Id = Guid.NewGuid(), Title ="Harry potter" } } },
                new Author { Id = Guid.NewGuid(), Name = "Author 2", Books = new List<Book>() },
            };

            int totalCount = 7; //total data dari database
            var pageResultDto = new PagedResultDto<Author>() { Data = author, TotalItems = totalCount, CurrentPage = 1, PageSize = 2, TotalPages = 4 };

            _authorRepoMock.Setup(repo => repo.GetAllAsync(pageNumber, pageSize)).ReturnsAsync(pageResultDto);

            var authorDtos = new List<AuthorDto>
            {
                new AuthorDto { Id = author[0].Id, Name = author[0].Name, BookIds = new List<Guid>{ author[0].Books.FirstOrDefault().Id } },
                new AuthorDto { Id = author[1].Id, Name = author[1].Name, BookIds = new List<Guid>{ } },
            };

            _mapperMock.Setup(m => m.Map<List<AuthorDto>>(pageResultDto.Data)).Returns(authorDtos);

            // Act
            var result = await _controller.GetAll(pageNumber, pageSize) as OkObjectResult;
            Console.WriteLine(result);
            //
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var response = result.Value as PagedResultDto<AuthorDto>;
            Assert.NotNull(response);            
            Assert.NotNull(response.Data);
            Assert.Equal(2, ((List<AuthorDto>)response.Data).Count);
            Assert.Equal(totalCount, response.TotalItems);
            Assert.Equal(4, response.TotalPages); // 7 total / 2 per halaman = 4 halaman
        }

    }
}
