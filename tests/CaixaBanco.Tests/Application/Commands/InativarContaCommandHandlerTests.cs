using CaixaBanco.Application.Commands.Contas.InativarConta;
using CaixaBanco.Domain.Entities;
using CaixaBanco.Domain.Enums;
using CaixaBanco.Domain.Notification;
using CaixaBanco.Domain.Repositories;
using Moq;

namespace CaixaBanco.Tests.Application.Commands
{
    public class InativarContaCommandHandlerTests
    {
        private readonly Mock<IContaRepository> _contaRepositoryMock;
        private readonly Mock<INotificador> _notificadorMock;
        private readonly InativarContaCommandHandler _handler;

        public InativarContaCommandHandlerTests()
        {
            _contaRepositoryMock = new Mock<IContaRepository>();
            _notificadorMock = new Mock<INotificador>();
            _handler = new InativarContaCommandHandler(
                _contaRepositoryMock.Object,
                _notificadorMock.Object
            );
        }

        [Fact(DisplayName = "Inativação de Conta bem-sucedida")]
        public async Task Handle_DeveRetornarTrueEInativarConta_QuandoContaAtivaExiste()
        {
            // Arrange
            const string documento = "12345678900";
            const string nome = "José da Silva";
            var contaAtiva = new Conta(nome, documento);
            var command = new InativarContaCommand { Documento = documento, UsuarioResponsavel = "João" };

            _contaRepositoryMock
                .Setup(r => r.ObterContaAsync(documento))
                .ReturnsAsync(contaAtiva);

            _contaRepositoryMock
                .Setup(r => r.InativarContaAsync(It.IsAny<InativacaoConta>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            Assert.Equal(StatusConta.Inativa, contaAtiva.Status);

            _contaRepositoryMock.Verify(r => r.ObterContaAsync(documento), Times.Once);
            _contaRepositoryMock.Verify(r => r.InativarContaAsync(It.Is<InativacaoConta>(
                reg => reg.Documento == documento && reg.UsuarioResponsavel == command.UsuarioResponsavel)), Times.Once);
            _notificadorMock.Verify(n => n.Disparar(It.IsAny<Notificacao>()), Times.Never);
        }

        [Theory(DisplayName = "Inativação falha se a conta não for encontrada")]
        [InlineData("11122233344", " 11122233344 ")]
        public async Task Handle_DeveRetornarFalseENotificar_QuandoContaNaoForEncontrada(
            string documentoEsperado, string documentoNoComando)
        {
            // Arrange
            var command = new InativarContaCommand { Documento = documentoNoComando, UsuarioResponsavel = "João" };

            _contaRepositoryMock
                .Setup(r => r.ObterContaAsync(documentoEsperado))
                .ReturnsAsync((Conta)null!); 

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(resultado);

            _contaRepositoryMock.Verify(r => r.ObterContaAsync(documentoEsperado), Times.Once);
            _contaRepositoryMock.Verify(r => r.InativarContaAsync(It.IsAny<InativacaoConta>()), Times.Never);
            _notificadorMock.Verify(n => n.Disparar(It.Is<Notificacao>(
                not => not.Mensagem == "Conta não encontrada.")), Times.Once);
        }

        // 3. Cenário: Conta já inativa
        [Theory(DisplayName = "Inativação falha se a conta já estiver inativa")]
        [InlineData(StatusConta.Inativa)]
        public async Task Handle_DeveRetornarFalseENotificar_QuandoContaJaEstiverInativa(StatusConta statusInicial)
        {
            // Arrange
            const string documento = "99988877766";
            const string nome = "Luca Silva";
            var conta = new Conta(documento, nome);
            var command = new InativarContaCommand { Documento = documento, UsuarioResponsavel = "João" };

            conta.Inativar();

            _contaRepositoryMock
                .Setup(r => r.ObterContaAsync(documento))
                .ReturnsAsync(conta);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(resultado);
            Assert.Equal(statusInicial, conta.Status); 

            _contaRepositoryMock.Verify(r => r.ObterContaAsync(documento), Times.Once);
            _contaRepositoryMock.Verify(r => r.InativarContaAsync(It.IsAny<InativacaoConta>()), Times.Never);
            _notificadorMock.Verify(n => n.Disparar(It.Is<Notificacao>(
                not => not.Mensagem == "Conta já está inativa.")), Times.Once);
        }

        [Fact(DisplayName = "Inativação retorna false se o repositório falhar ao salvar")]
        public async Task Handle_DeveRetornarFalse_QuandoInativarContaAsyncFalha()
        {
            // Arrange
            const string documento = "44455566677";
            const string nome = "Luca Silva";
            var contaAtiva = new Conta(documento, nome);
            var command = new InativarContaCommand { Documento = documento, UsuarioResponsavel = "admin" };

            _contaRepositoryMock
                .Setup(r => r.ObterContaAsync(documento))
                .ReturnsAsync(contaAtiva);

            _contaRepositoryMock
                .Setup(r => r.InativarContaAsync(It.IsAny<InativacaoConta>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(resultado);
            Assert.Equal(StatusConta.Inativa, contaAtiva.Status); 

            _contaRepositoryMock.Verify(r => r.InativarContaAsync(It.IsAny<InativacaoConta>()), Times.Once);
            _notificadorMock.Verify(n => n.Disparar(It.IsAny<Notificacao>()), Times.Never);
        }

        [Theory(DisplayName = "Deve usar 'system' como usuário se o responsável for nulo/vazio/whitespace")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        public async Task Handle_DeveUsarSystemComoUsuarioResponsavel_QuandoComandoNaoInformaUsuario(string usuarioInvalido)
        {
            // Arrange
            const string documento = "66677788899";
            const string nome = "Luca Silva";
            var contaAtiva = new Conta(documento, nome);
            var command = new InativarContaCommand { Documento = documento, UsuarioResponsavel = usuarioInvalido };
            const string usuarioEsperado = "system";

            _contaRepositoryMock
                .Setup(r => r.ObterContaAsync(documento))
                .ReturnsAsync(contaAtiva);

            _contaRepositoryMock
                .Setup(r => r.InativarContaAsync(It.IsAny<InativacaoConta>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            _contaRepositoryMock.Verify(r => r.InativarContaAsync(It.Is<InativacaoConta>(
                reg => reg.UsuarioResponsavel == usuarioEsperado)), Times.Once);
        }
    }
}
