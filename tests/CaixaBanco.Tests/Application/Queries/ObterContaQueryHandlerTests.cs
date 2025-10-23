using CaixaBanco.Application.Queries.Contas.ObterConta;
using CaixaBanco.Application.Responses.Contas;
using CaixaBanco.Domain.Entities;
using CaixaBanco.Domain.Enums;
using CaixaBanco.Domain.Notification;
using CaixaBanco.Domain.Repositories;
using Moq;

namespace CaixaBanco.Tests.Application.Queries
{
    public class ObterContaQueryHandlerTests
    {
        private readonly Mock<IContaRepository> _contaRepositoryMock;
        private readonly Mock<INotificador> _notificadorMock;
        private readonly ObterContaQueryHandler _handler;

        public ObterContaQueryHandlerTests()
        {
            _contaRepositoryMock = new Mock<IContaRepository>();
            _notificadorMock = new Mock<INotificador>();
            _handler = new ObterContaQueryHandler(
                _contaRepositoryMock.Object,
                _notificadorMock.Object
            );
        }

        [Fact(DisplayName = "Deve retornar ContaResponse e não notificar quando conta é encontrada")]
        public async Task Handle_DeveRetornarContaResponse_QuandoContaForEncontrada()
        {
            // Arrange
            var dataAbertura = DateTime.UtcNow.Date;
            var contaDb = new Conta("João da Silva", "12345678900");
            var contasEncontradas = new List<Conta> { contaDb };

            var query = new ObterContaQuery { Nome = "João", Documento = "12345678900" };

            _contaRepositoryMock
                .Setup(r => r.ObterContasAsync(query.Nome, query.Documento))
                .ReturnsAsync(contasEncontradas);

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<ContaResponse>(resultado);

            Assert.Equal("João da Silva", resultado.Nome);
            Assert.Equal("12345678900", resultado.Documento);
            Assert.Equal(1000.00m, resultado.Saldo);
            Assert.Equal(dataAbertura, resultado.DataAbertura);
            Assert.Equal(StatusConta.Ativa, resultado.Status);

            _contaRepositoryMock.Verify(r => r.ObterContasAsync(query.Nome, query.Documento), Times.Once);
            _notificadorMock.Verify(n => n.Disparar(It.IsAny<Notificacao>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar null e notificar quando nenhuma conta for encontrada")]
        public async Task Handle_DeveRetornarNullENotificar_QuandoNenhumaContaForEncontrada()
        {
            // Arrange
            var contasVazias = new List<Conta>();
            var query = new ObterContaQuery { Nome = "Inexistente", Documento = "000" };

            _contaRepositoryMock
                .Setup(r => r.ObterContasAsync(query.Nome, query.Documento))
                .ReturnsAsync(contasVazias);

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(resultado);

            _contaRepositoryMock.Verify(r => r.ObterContasAsync(query.Nome, query.Documento), Times.Once);
            _notificadorMock.Verify(n => n.Disparar(It.Is<Notificacao>(
                not => not.Mensagem == "Não foi encontrada nenhuma conta para este documento ou nome")), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar apenas o primeiro resultado quando várias contas são encontradas")]
        public async Task Handle_DeveRetornarPrimeiroResultado_QuandoVariasContasSaoEncontradas()
        {
            // Arrange
            var conta1 = new Conta("Otavio", "111");
            var conta2 = new Conta("Otaviano", "222");
            var contasMultiplas = new List<Conta> { conta1, conta2 };

            var query = new ObterContaQuery { Nome = "Otav" };

            _contaRepositoryMock
                .Setup(r => r.ObterContasAsync(query.Nome, query.Documento))
                .ReturnsAsync(contasMultiplas);

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(conta1.Nome, resultado.Nome);
            Assert.Equal(conta1.Documento, resultado.Documento);
            Assert.Equal(1000m, resultado.Saldo);
            Assert.Equal(StatusConta.Ativa, resultado.Status);

            _notificadorMock.Verify(n => n.Disparar(It.IsAny<Notificacao>()), Times.Never);
        }
    }
}
