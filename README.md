# 📚 Bookstore Inventory API

Bookstore Inventory adalah proyek API sederhana menggunakan **ASP.NET Core 8**, yang mengelola data penulis (Authors) dan buku (Books) dengan konsep best practice seperti:
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

## ⚙️ Instalasi

```bash
git clone https://github.com/yourusername/BookstoreInventory.git
cd BookstoreInventory
dotnet restore
dotnet ef database update
dotnet run

---
## 📌 Arsitektur Project
📁 BookstoreInventory
│
├── Controllers/
├── DTOs/
├── Models/
├── Repositories/
├── Validators/
├── Utils/
├── Caching/
├── Filters/
└── Tests/
