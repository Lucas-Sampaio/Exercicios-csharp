using FluentValidation;
using Questao5.Application.Queries.Requests;

namespace Questao5.Application.Validators
{
    public class ObterSaldoValidator : AbstractValidator<ObterSaldoRequest>
    {
        public ObterSaldoValidator()
        {
            RuleFor(x => x.IdContaCorrente)
               .NotEqual(Guid.Empty)
               .WithMessage("O {PropertyName} não pode ser vazio.");
        }
    }
}