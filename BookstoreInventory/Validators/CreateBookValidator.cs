using BookstoreInventory.DTOs;
using FluentValidation;

namespace BookstoreInventory.Validators
{
    public class CreateBookValidator : AbstractValidator<CreateBookDto>
    {
        public CreateBookValidator() 
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.AuthorId).NotEmpty().WithMessage("AuthorId is required");
        }
    }
}
