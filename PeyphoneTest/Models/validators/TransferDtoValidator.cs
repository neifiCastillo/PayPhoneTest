using FluentValidation;
using PeyphoneTest.Models.Dtos;

namespace PeyphoneTest.Validators
{
    public class TransferDtoValidator : AbstractValidator<TransferDto>
    {
        public TransferDtoValidator()
        {
            RuleFor(x => x.FromWalletId)
                .GreaterThan(0).WithMessage("Los IDs de las billeteras deben ser válidos.");

            RuleFor(x => x.ToWalletId)
                .GreaterThan(0).WithMessage("Los IDs de las billeteras deben ser válidos.");

            RuleFor(x => x)
                .Must(x => x.FromWalletId != x.ToWalletId)
                .WithMessage("No se puede transferir a la misma billetera.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");

            RuleFor(x => x.ToWalletName)
                .NotEmpty().WithMessage("Debe proporcionar el nombre de la cuenta de destino.")
                .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Debe proporcionar el nombre de la cuenta de destino.");
        }
    }
}
