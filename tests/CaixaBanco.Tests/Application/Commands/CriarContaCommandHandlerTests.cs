using CaixaBanco.Application.Commands.Contas.CriarConta;
using CaixaBanco.Domain.Entities;
using CaixaBanco.Domain.Notification;
using CaixaBanco.Domain.Repositories;
using Moq;

namespace CaixaBanco.Tests.Application.Commands
{
    public class CriarContaCommandHandlerTests
    {
        private readonly Mock<IContaRepository> _contaRepositoryMock;
        private readonly Mock<INotificador> _notificadorMock;
        private readonly CriarContaCommandHandler _handler;

        public CriarContaCommandHandlerTests()
        {
            _contaRepositoryMock = new Mock<IContaRepository>();
            _notificadorMock = new Mock<INotificador>();
            _handler = new CriarContaCommandHandler(
                _contaRepositoryMock.Object,
                _notificadorMock.Object
            );
        }

        [Fact(DisplayName = "Criação de Conta bem-sucedida")]
        public async Task Handle_DeveRetornarTrueESalvarConta_QuandoDocumentoNaoExiste()
        {
            // Arrange
            var command = new CriarContaCommand { Nome = "João da Silva", Documento = "12345678900" };

            _contaRepositoryMock
                .Setup(r => r.ObterContaAsync("12345678900", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Conta)null!);

            _contaRepositoryMock
                .Setup(r => r.CriarContaAsync(It.IsAny<Conta>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            _contaRepositoryMock.Verify(r => r.ObterContaAsync("12345678900", It.IsAny<CancellationToken>()), Times.Once);
            _contaRepositoryMock.Verify(r => r.CriarContaAsync(It.Is<Conta>(c =>
                c.Nome == "João da Silva" && c.Documento == "12345678900"), It.IsAny<CancellationToken>()), Times.Once);
            _notificadorMock.Verify(n => n.Disparar(It.IsAny<Notificacao>()), Times.Never);
        }

        [Fact(DisplayName = "Criação de Conta falha ao salvar no repositório")]
        public async Task Handle_DeveRetornarFalse_QuandoFalhaAoSalvarNoRepositorio()
        {
            // Arrange
            var command = new CriarContaCommand { Nome = "Maria Souza", Documento = "11122233344" };

            _contaRepositoryMock
                .Setup(r => r.ObterContaAsync("11122233344", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Conta)null!);

            _contaRepositoryMock
                .Setup(r => r.CriarContaAsync(It.IsAny<Conta>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(resultado);
            _contaRepositoryMock.Verify(r => r.ObterContaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _contaRepositoryMock.Verify(r => r.CriarContaAsync(It.IsAny<Conta>(), It.IsAny<CancellationToken>()), Times.Once);
            _notificadorMock.Verify(n => n.Disparar(It.IsAny<Notificacao>()), Times.Never);
        }


        [Theory(DisplayName = "Criação de Conta - Documento já existe")]
        [InlineData("99988877766", "Documento já existe com trim")]
        public async Task Handle_DeveRetornarFalseEDispararNotificacao_QuandoContaJaExiste(string documento, string nome)
        {
            // Arrange
            var command = new CriarContaCommand { Nome = nome, Documento = documento };
            var contaExistente = new Conta(nome, documento);

            _contaRepositoryMock
                .Setup(r => r.ObterContaAsync(documento, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contaExistente);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(resultado);
            _contaRepositoryMock.Verify(r => r.ObterContaAsync(documento, It.IsAny<CancellationToken>()), Times.Once);
            _contaRepositoryMock.Verify(r => r.CriarContaAsync(It.IsAny<Conta>(), It.IsAny<CancellationToken>()), Times.Never);
            _notificadorMock.Verify(n => n.Disparar(It.Is<Notificacao>(
                not => not.Mensagem == "Já existe uma conta cadastrada para este documento.")), Times.Once);
        }

        [Theory(DisplayName = "Criação de Conta - Validação de entrada ausente")]
        [InlineData(null, null, "Documento e Nome precisam ser enviados para criação da conta")]
        [InlineData(null, " ", "Documento e Nome precisam ser enviados para criação da conta")]
        [InlineData(" ", null, "Documento e Nome precisam ser enviados para criação da conta")]
        [InlineData(" ", "\t", "Documento e Nome precisam ser enviados para criação da conta")]
        public async Task Handle_DeveRetornarFalseEDispararNotificacao_QuandoDocumentoENomeEstaoAusentes(
            string documento, string nome, string mensagemEsperada)
        {
            // Arrange
            var command = new CriarContaCommand { Nome = nome, Documento = documento };

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(resultado);
            _contaRepositoryMock.Verify(r => r.ObterContaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _contaRepositoryMock.Verify(r => r.CriarContaAsync(It.IsAny<Conta>(), It.IsAny<CancellationToken>()), Times.Never);
            _notificadorMock.Verify(n => n.Disparar(It.Is<Notificacao>(
                not => not.Mensagem == mensagemEsperada)), Times.Once);
        }

        [Theory(DisplayName = "Criação de Conta bem-sucedida - Documento com espaços")]
        [InlineData(" 12345678900 ", "12345678900")]
        [InlineData("12345-6789/00 ", "12345-6789/00")]
        public async Task Handle_DeveFazerTrimNoDocumentoEUsarParaBuscaECriacao(string documentoComEspacos, string documentoEsperado)
        {
            // Arrange
            var command = new CriarContaCommand { Nome = "Usuário Trim", Documento = documentoComEspacos };

            _contaRepositoryMock
                .Setup(r => r.ObterContaAsync(documentoEsperado, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Conta)null!);

            _contaRepositoryMock
                .Setup(r => r.CriarContaAsync(It.IsAny<Conta>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            _contaRepositoryMock.Verify(r => r.ObterContaAsync(documentoEsperado, It.IsAny<CancellationToken>()), Times.Once);
            _contaRepositoryMock.Verify(r => r.CriarContaAsync(It.Is<Conta>(c =>
                c.Documento == documentoEsperado), It.IsAny<CancellationToken>()), Times.Once);
            _notificadorMock.Verify(n => n.Disparar(It.IsAny<Notificacao>()), Times.Never);
        }
    }
}
