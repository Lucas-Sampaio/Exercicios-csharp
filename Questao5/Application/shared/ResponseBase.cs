using FluentValidation.Results;
using Questao5.Domain.Errors;

namespace Questao5.Application.shared
{
    public class ResponseBase
    {
        public ResponseBase()
        {
            Errors = [];
        }

        public List<Error> Errors { get; set; }

        public void AdicionarErros(List<ValidationFailure> failures)
        {
            var erros = failures.ConvertAll(x => new Error(x.ErrorCode, x.ErrorMessage)).ToArray();
            AdicionarErros(erros);
        }

        public void AdicionarErros(params Error[] erros) => Errors.AddRange(erros);
    }
}