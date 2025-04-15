using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using Questao5.Domain;
using Questao5.Domain.Dtos;
using Questao5.Domain.Interfaces;
using Xunit;

namespace Questao5.Tests
{
    public class ObterSaldoHandlerTests
    {
        private readonly IValidator<ObterSaldoRequest> _validator;
        private readonly IContaRepository _contaRepository;
        private readonly ISaldoRepository _saldoRepository;
        private readonly ObterSaldoHandler _handler;

        public ObterSaldoHandlerTests()
        {
            _validator = Substitute.For<IValidator<ObterSaldoRequest>>();
            _contaRepository = Substitute.For<IContaRepository>();
            _saldoRepository = Substitute.For<ISaldoRepository>();

            _handler = new ObterSaldoHandler(_validator, _contaRepository, _saldoRepository);
        }

        [Fact]
        public async Task Handle_ShouldReturnErrors_WhenValidationFails()
        {
            // Arrange
            var request = new ObterSaldoRequest { IdContaCorrente = Guid.NewGuid() };
            var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("IdContaCorrente", "IdContaCorrente is required")
        };
            _validator.Validate(request).Returns(new ValidationResult(validationFailures));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().ContainSingle(e => e.Message == "IdContaCorrente is required");
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenAccountDoesNotExist()
        {
            // Arrange
            var request = new ObterSaldoRequest { IdContaCorrente = Guid.NewGuid() };
            _validator.Validate(request).Returns(new ValidationResult());
            _contaRepository.ObterPorId(request.IdContaCorrente).Returns((ContaDto?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Errors.Should().ContainSingle(e => e.Message == ErrorMessages.INVALID_ACCOUNT);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenAccountIsInactive()
        {
            // Arrange
            var request = new ObterSaldoRequest { IdContaCorrente = Guid.NewGuid() };
            _validator.Validate(request).Returns(new ValidationResult());
            _contaRepository.ObterPorId(request.IdContaCorrente).Returns(new ContaDto
            {
                Id = Guid.NewGuid().ToString(),
                Nome = "Test Account",
                Numero = 12345,
                Ativo = false
            });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Errors.Should().ContainSingle(e => e.Message == ErrorMessages.INACTIVE_ACCOUNT);
        }

        [Fact]
        public async Task Handle_ShouldReturnSaldo_WhenAccountIsValid()
        {
            // Arrange
            var request = new ObterSaldoRequest { IdContaCorrente = Guid.NewGuid() };
            _validator.Validate(request).Returns(new ValidationResult());
            _contaRepository.ObterPorId(request.IdContaCorrente).Returns(new ContaDto
            {
                Id = Guid.NewGuid().ToString(),
                Nome = "Test Account",
                Numero = 12345,
                Ativo = true
            });
            _saldoRepository.ObterSaldoConta(request.IdContaCorrente).Returns(1000m);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.SaldoConta.Should().NotBeNull();
            result.SaldoConta.Saldo.Should().Be(1000m);
            result.SaldoConta.NomeTitular.Should().Be("Test Account");
            result.SaldoConta.NumeroConta.Should().Be(12345);
            result.SaldoConta.DataConsulta.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }
    }
}