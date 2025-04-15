using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Handlers;
using Questao5.Domain;
using Questao5.Domain.Dtos;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Interfaces;
using Questao5.Domain.Repositories;
using Questao5.Infrastructure.Database.UnityOfWork;
using Xunit;

namespace Questao5.Tests
{
    public class AdicionarMovimentoHandlerTests
    {
        private readonly IValidator<AdicionarMovimentoRequest> _validator;
        private readonly IContaRepository _contaRepository;
        private readonly IIdempontenciaRepository _idempontenciaRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IUnityOfWork _uow;
        private readonly AdicionarMovimentoHandler _handler;

        public AdicionarMovimentoHandlerTests()
        {
            _validator = Substitute.For<IValidator<AdicionarMovimentoRequest>>();
            _contaRepository = Substitute.For<IContaRepository>();
            _idempontenciaRepository = Substitute.For<IIdempontenciaRepository>();
            _movimentoRepository = Substitute.For<IMovimentoRepository>();
            _uow = Substitute.For<IUnityOfWork>();

            _handler = new AdicionarMovimentoHandler(
                _validator,
                _contaRepository,
                _idempontenciaRepository,
                _movimentoRepository,
                _uow
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnErrors_WhenValidationFails()
        {
            // Arrange
            var request = new AdicionarMovimentoRequest();
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
            await _contaRepository.Received(0).ObterPorId(Arg.Any<Guid>());
            await _idempontenciaRepository.Received(0).ObterPorId(Arg.Any<Guid>());
            await _movimentoRepository.Received(0).Inserir(Arg.Any<Movimento>());
            await _idempontenciaRepository.Received(0).Inserir(Arg.Any<Idempotencia>());
            await _uow.Received(0).BeginTransactionAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnCachedResponse_WhenIdempotencyExists()
        {
            // Arrange
            var request = ObterRequestValida();
            var cachedResponse = new AdicionarMovimentoResponse { Id = Guid.NewGuid() };
            _validator.Validate(request).Returns(new ValidationResult());

            _idempontenciaRepository.ObterPorId(request.IdRequest)
                .Returns(new IdempotenciaDto { Resultado = System.Text.Json.JsonSerializer.Serialize(cachedResponse) });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Id.Should().Be(cachedResponse.Id);

            await _contaRepository.Received(0).ObterPorId(Arg.Any<Guid>());
            await _idempontenciaRepository.Received(1).ObterPorId(Arg.Is<Guid>(x => x == request.IdRequest));
            await _movimentoRepository.Received(0).Inserir(Arg.Any<Movimento>());
            await _idempontenciaRepository.Received(0).Inserir(Arg.Any<Idempotencia>());
            await _uow.Received(0).BeginTransactionAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenAccountIsInvalid()
        {
            // Arrange
            var request = new AdicionarMovimentoRequest { IdContaCorrente = Guid.NewGuid() };
            _validator.Validate(request).Returns(new ValidationResult());
            ContaDto? contaDto = null;
            _contaRepository.ObterPorId(request.IdContaCorrente).Returns(contaDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Errors.Should().ContainSingle(e => e.Message == ErrorMessages.INVALID_ACCOUNT);

            await _contaRepository.Received(1).ObterPorId(Arg.Is<Guid>(x => x == request.IdContaCorrente));
            await _idempontenciaRepository.Received(1).ObterPorId(Arg.Any<Guid>());
            await _movimentoRepository.Received(0).Inserir(Arg.Any<Movimento>());
            await _idempontenciaRepository.Received(0).Inserir(Arg.Any<Idempotencia>());
            await _uow.Received(0).BeginTransactionAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenAccountIsInactive()
        {
            // Arrange
            var request = new AdicionarMovimentoRequest { IdContaCorrente = Guid.NewGuid() };
            _validator.Validate(request).Returns(new ValidationResult());
            ContaDto? contaDto = new ContaDto { Ativo = false};
            _contaRepository.ObterPorId(request.IdContaCorrente).Returns(contaDto);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Errors.Should().ContainSingle(e => e.Message == ErrorMessages.INACTIVE_ACCOUNT);

            await _contaRepository.Received(1).ObterPorId(Arg.Is<Guid>(x => x == request.IdContaCorrente));
            await _idempontenciaRepository.Received(1).ObterPorId(Arg.Any<Guid>());
            await _movimentoRepository.Received(0).Inserir(Arg.Any<Movimento>());
            await _idempontenciaRepository.Received(0).Inserir(Arg.Any<Idempotencia>());
            await _uow.Received(0).BeginTransactionAsync(Arg.Any<CancellationToken>());
        }
        [Fact]
        public async Task Handle_ShouldCommitTransaction_WhenOperationIsSuccessful()
        {
            // Arrange
            var request = ObterRequestValida();
            var movimentoId = Guid.NewGuid();
            _validator.Validate(request).Returns(new ValidationResult());
            _contaRepository.ObterPorId(request.IdContaCorrente).Returns(new ContaDto { Ativo = true });
            var transaction = Substitute.For<IUnitOfWorkTransaction>();
            _uow.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(transaction);
            _movimentoRepository.Inserir(Arg.Any<Movimento>()).Returns(movimentoId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Id.Should().Be(movimentoId);
            await _contaRepository.Received(1).ObterPorId(Arg.Any<Guid>());
            await _idempontenciaRepository.Received(1).ObterPorId(Arg.Any<Guid>());
            await _movimentoRepository.Received(1).Inserir(Arg.Any<Movimento>());
            await _idempontenciaRepository.Received(1).Inserir(Arg.Any<Idempotencia>());
            await _uow.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
            transaction.Received(1).Commit();
            transaction.Received(0).RollBack();
        }

        [Fact]
        public async Task Handle_ShouldRollbackTransaction_WhenExceptionOccurs()
        {
            // Arrange
            var request = new AdicionarMovimentoRequest
            {
                IdRequest = Guid.NewGuid(),
                IdContaCorrente = Guid.NewGuid(),
                MovimentoDto = new AdicionarMovimentoDto { Valor = 100, TipoMovimento = (char)ETipoMovimento.Credito }
            };
            _validator.Validate(request).Returns(new ValidationResult());
            _contaRepository.ObterPorId(request.IdContaCorrente).Returns(new ContaDto { Ativo = true });

            var transaction = Substitute.For<IUnitOfWorkTransaction>();
            _uow.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(transaction);
            _movimentoRepository.When(x => x.Inserir(Arg.Any<Movimento>())).Do(x => throw new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
            transaction.Received(1).RollBack();
        }

        private AdicionarMovimentoRequest ObterRequestValida()
        {
            return new AdicionarMovimentoRequest
            {
                IdRequest = Guid.NewGuid(),
                IdContaCorrente = Guid.NewGuid(),
                MovimentoDto = new AdicionarMovimentoDto
                {
                    TipoMovimento = (char)ETipoMovimento.Credito,
                    Valor = 10
                }
            };
        }
    }
}