using BookstoreInventory.DTOs;
using FluentValidation;

namespace BookstoreInventory.Validators
{
    public class CreateAuthorValidator : AbstractValidator<CreateAuthorDto>
    {
        public CreateAuthorValidator() {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        }
    }
}
