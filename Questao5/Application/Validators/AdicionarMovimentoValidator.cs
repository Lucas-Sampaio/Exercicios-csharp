using FluentValidation;
using Questao5.Application.Commands.Requests;
using Questao5.Domain;

namespace Questao5.Application.Validators
{
    public class AdicionarMovimentoValidator : AbstractValidator<AdicionarMovimentoRequest>
    {
        public AdicionarMovimentoValidator()
        {
            RuleFor(x => x.IdRequest)
               .NotEqual(Guid.Empty)
               .WithMessage("O {PropertyName} eh obrigatorio.");

            RuleFor(x => x.IdContaCorrente)
                .NotEqual(Guid.Empty)
                .WithMessage("O {PropertyName} não pode ser vazio.");

            RuleFor(x => x.MovimentoDto)
                .NotNull();

            When(x => x.MovimentoDto is not null,() =>
            {
                RuleFor(x => x.MovimentoDto.Valor)
               .GreaterThan(0)
               .WithErrorCode(nameof(ErrorMessages.INVALID_VALUE))
               .WithMessage(ErrorMessages.INVALID_VALUE);

                RuleFor(x => x.MovimentoDto.TipoMovimento)
                    .Must(x => x.Equals('C') || x.Equals('D'))
                    .WithErrorCode(nameof(ErrorMessages.INVALID_TYPE))
                    .WithMessage(nameof(ErrorMessages.INVALID_TYPE));
            });
           
        }
    }
}