using FluentValidation;
using PeyphoneTest.Models.Dtos;

namespace PeyphoneTest.Validators
{
    public class WalletDtoValidator : AbstractValidator<WalletDto>
    {
        public WalletDtoValidator()
        {
            RuleFor(w => w.Name)
                .NotEmpty().WithMessage("El nombre de la billetera es obligatorio.")
                .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("El nombre de la billetera es obligatorio.");

            RuleFor(w => w.DocumentId)
                .NotEmpty().WithMessage("El documento es obligatorio.")
                .Must(doc => !string.IsNullOrWhiteSpace(doc)).WithMessage("El documento es obligatorio.")
                .Length(10).WithMessage("El documento debe tener exactamente 10 caracteres.");

            RuleFor(w => w.Balance)
                .GreaterThanOrEqualTo(0).WithMessage("El saldo inicial no puede ser negativo.");
        }
    }
}
