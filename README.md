# 📚 Bookstore Inventory API

Bookstore Inventory adalah proyek API sederhana menggunakan **ASP.NET Core 8** dengan database **SQL Server**, yang mengelola data penulis (Authors) dan buku (Books) dengan konsep best practice seperti:
- ✅ Clean Architecture
- ✅ AutoMapper
- ✅ FluentValidation
- ✅ Error Handling Middleware
- ✅ Caching (Decorator Pattern)
- ✅ Pagination
- ✅ Serilog Logging
- ✅ Swagger Documentation

---

## 🚀 Fitur Utama
- CRUD **Authors** & **Books**.
- Validasi input dengan **FluentValidation**.
- **Error Handling** terpusat.
- **Global Response Wrapper** menggunakan Filter.
- **Caching** untuk optimasi performa.
- DTO Mapping via **AutoMapper** untuk mencegah infinite loop.
- Pagination pada data list.
- Logging dengan **Serilog**.


---

## 🛠️ Tech Stack
| Tech | Description |
|------|-------------|
| ASP.NET Core 8 | Backend Framework |
| Entity Framework Core | ORM |
| SQL Server | Database |
| AutoMapper | Mapping Entity → DTO |
| FluentValidation | Input Validation |
| Serilog | Logging |
| Swagger | API Documentation |
| IMemoryCache | Caching |
| xUnit + Moq | Unit Testing |

---

## 📌 Arsitektur Project
BookstoreInventory/
│
├── BookstoreInventory/           // Project utama
│   └── Caching/
│   └── Controllers/
│   └── Data/
│   └── DTOs/
│   └── Mappings/
│   └── Migrations/
│   └── Models/
│   └── Repositories/
│   └── Utils/
│   └── Validators/
│
├── BookstoreInventory.Tests/     // Project testing
│   └── Controllers/
│       └── AuthorsControllerTests.cs
│   └── ErrorHandlerMiddlewareTests.cs
│
├── BookstoreInventory.sln

---

## 🔧 Instalasi dan Konfigurasi
### 1️⃣ Clone Repository
```bash
git clone https://github.com/SuciN-26/BookstoreAPI.git
cd BookstoreInventory
```

### 2️⃣ Konfigurasi Database
- **Buka** file `appsettings.json`
- **Sesuaikan** connection string dengan database lokalmu:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=DESKTOP-32TTCH0\\SQLEXPRESS;Database=BookstoreDB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
}
```
- **Jalankan perintah `dotnet restore` pada project BookstoreInventory**
```bash

dotnet restore
```
- **Jalankan perintah `dotnet ef database update` pada project BookstoreInventory**
```bash 
dotnet ef database update
```
(Pastikan dotnet-ef menggunakan versi 9.^ )


### 3️⃣ Jalankan Aplikasi
- **Untuk menjalankan aplikasi (pastikan berada pada project BookstoreInventory)**
```bash 
dotnet run
```
Aplikasi akan berjalan di http://localhost:5240 🚀

---
### 📌 Endpoints API
📘 Books
| Method | Endpoint | Deskripsi |
|------|-------------|-------------|
|GET	|/api/Books	|Get semua buku |
|GET	|/api/Books/{id} |	Get detail buku|
|POST	|/api/Books	 | Tambah buku baru|
|PUT	|/api/Books/{id} |	Update buku|
|DELETE	|/api/Books/{id} |	Hapus buku|


📝 Authors
| Method |	Endpoint | Deskripsi |
|------|-------------|-------------|
|GET	|/api/Authors?page=1&pageSize=10 |	Get semua penulis (dengan pagination)|
|GET	|/api/Authors/{id}|	Get detail penulis|
|POST	|/api/Authors|	Tambah penulis baru|
|PUT	|/api/Authors/{id}|	Update penulis|
|DELETE	|/api/Authors/{id}|	Hapus penulis|

---

## ⚡Fitur Tambahan

### ✅ Pagination
Gunakan query ?page=1&size=10 untuk membatasi hasil.

Contoh:
```sh
GET /api/Authors?page=1&size=10
```

Format response:

```json
{
  "status": 200,
  "data": 
    {
        "data": [
            { "id": "1", "title": "Book Title", "author": { "name": "Author Name" } }
        ],
        "currentPage": 1,
        "pageSize": 10,
        "totalItems": 7,
        "totalPages": 1
    }
}
```

### ✅ AutoMapper DTO
Entity Model (Book.cs)

```csharp
public class Book {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid AuthorId { get; set; }
    public virtual Author Author { get; set; }
}
```

DTO (BookDto.cs)

```csharp
public class BookDto {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string AuthorName { get; set; }
}

```
Mapping Profile

```csharp
public class MappingProfile : Profile {
    public MappingProfile() {
        CreateMap<Book, BookDto>()
        .ForMember(dest => dest.Author, opt => opt.MapFrom(src => new AuthorDto
        {
            Id = src.Author.Id,
            Name = src.Author.Name,
            BookIds = src.Author.Books.Select(b => b.Id).ToList()
        }));
    }
}
```

### ✅ Caching
Books akan di-cache selama 5 menit untuk mempercepat response.

```csharp

public class CachedBookService : IBookRepository {
    private readonly IBookRepository _inner;
    private readonly IMemoryCache _cache;

    public CachedBookService(IBookRepository inner, IMemoryCache cache) {
        _inner = inner;
        _cache = cache;
    }

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
}
```

### ✅ Error Handling dengan Middleware
Semua error ditangani dengan middleware custom.

``` csharp

namespace BookstoreInventory.Utils
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Terjadi error saat memproses request {Method} {Path}. Error: {Message}",
                    context.Request.Method,
                    context.Request.Path,
                    ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                // Jika response sudah berjalan, kita tidak bisa menulis response lagi
                return;
            }

            var response = context.Response;
            response.ContentType = "application/json";

            int statusCode;
            object errorResponse;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new { status = statusCode, error = exception.Message };
                    break;

                case ValidationException validationEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    var errors = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                    errorResponse = new { status = statusCode, error = "Validation failed.", errors };
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new { status = statusCode, error = "Terjadi kesalahan pada server." };
                    break;
            }

            response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(errorResponse);

            await response.WriteAsync(result);
        }
    }
}

```

---
## 🧪 Menjalankan Unit Test
---

- **Jalakan perintah `dotnet restore` pada project BookstoreInventoryTest**
```bash
cd BookstoreInventory.Tests
dotnet restore
```
- **Untuk menjalankan unit testing**
```
dotnet test
```
Contoh Output Jika Berhasil
```
Total tests: 10
Passed: 10
```

---
## 📌 Contoh Unit Test untuk Error Handling

```
public class ErrorHandlerMiddlewareTests {
    [Fact]
    public async Task Invoke_ShouldHandleException_AndReturn500() {
        var middleware = new ErrorHandlerMiddleware(async (innerHttpContext) => {
            throw new Exception("Test Exception");
        }, Mock.Of<ILogger<ErrorHandlerMiddleware>>());

        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        await middleware.Invoke(context);

        Assert.Equal(500, context.Response.StatusCode);
    }
}

```

---
### 💡 Kontributor
👨‍💻 Suci Nopikirana - Developer 🚀
