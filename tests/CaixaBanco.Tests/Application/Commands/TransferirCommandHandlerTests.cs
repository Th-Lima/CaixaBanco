using CaixaBanco.Application.Commands.Transacoes.Transferir;
using CaixaBanco.Application.Responses.Transacoes;
using CaixaBanco.Domain.Entities;
using CaixaBanco.Domain.Notification;
using CaixaBanco.Domain.Repositories;
using Moq;

namespace CaixaBanco.Tests.Application.Commands
{
    public class TransferirCommandHandlerTests
    {
        private readonly Mock<ITransacaoRepository> _transacaoRepositoryMock;
        private readonly Mock<IContaRepository> _contaRepositoryMock;
        private readonly Mock<INotificador> _notificadorMock;
        private readonly TransferirCommandHandler _handler;

        public TransferirCommandHandlerTests()
        {
            _transacaoRepositoryMock = new Mock<ITransacaoRepository>();
            _contaRepositoryMock = new Mock<IContaRepository>();
            _notificadorMock = new Mock<INotificador>();

            _handler = new TransferirCommandHandler(
                _transacaoRepositoryMock.Object,
                _contaRepositoryMock.Object,
                _notificadorMock.Object
            );
        }

        #region Cenário de Sucesso

        [Fact(DisplayName = "Transferência bem-sucedida deve retornar TransferenciaResponse e atualizar saldos")]
        public async Task Handle_DeveRetornarResponse_QuandoTransacaoForBemSucedida()
        {
            // Arrange
            const string docOrigem = "111";
            const string docDestino = "222";
            const decimal valorTransferencia = 50.00m;
            const decimal saldoInicial = 1000.00m;

            var contaOrigem = new Conta("José", docOrigem);
            var contaDestino = new Conta("João", docDestino);

            var command = new TransferirCommand
            {
                DocumentoOrigem = docOrigem,
                DocumentoDestino = docDestino,
                Valor = valorTransferencia
            };

            var dataTransacao = DateTime.UtcNow.Date;
            var transacaoSalva = new Transacao(contaOrigem.Id, contaDestino.Id, valorTransferencia);

            _contaRepositoryMock.Setup(r => r.ObterContaAsync(docOrigem, It.IsAny<CancellationToken>())).ReturnsAsync(contaOrigem);
            _contaRepositoryMock.Setup(r => r.ObterContaAsync(docDestino, It.IsAny<CancellationToken>())).ReturnsAsync(contaDestino);
            _transacaoRepositoryMock
                .Setup(r => r.ProcessarTransferenciaAsync(
                    It.IsAny<Conta>(), It.IsAny<Conta>(), valorTransferencia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transacaoSalva);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<TransferenciaResponse>(resultado);

            Assert.Equal(saldoInicial - valorTransferencia, contaOrigem.Saldo);
            Assert.Equal(saldoInicial + valorTransferencia, contaDestino.Saldo);

            Assert.Equal(valorTransferencia, resultado.ValorTransacao);
            Assert.Equal(950.00m, resultado.ValorContaOrigemAtualizado);
            Assert.Equal(1050.00m, resultado.ValorContaDestinoAtualizado);
            Assert.Equal(dataTransacao, resultado.CriadoEm);

            _transacaoRepositoryMock.Verify(r => r.ProcessarTransferenciaAsync(
                It.IsAny<Conta>(), It.IsAny<Conta>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()), Times.Once);
            _notificadorMock.Verify(n => n.Disparar(It.IsAny<Notificacao>()), Times.Never);
        }

        #endregion

        #region Cenários de Validação (Documento)

        [Fact(DisplayName = "Validação: Documentos de origem e destino iguais (trim)")]
        public async Task Handle_DeveRetornarNullENotificar_QuandoDocumentosSaoIguais()
        {
            // Arrange
            const string documento = " 111 ";
            var command = new TransferirCommand
            {
                DocumentoOrigem = documento,
                DocumentoDestino = documento,
                Valor = 10.00m
            };

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(resultado);
            _contaRepositoryMock.Verify(r => r.ObterContaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _notificadorMock.Verify(n => n.Disparar(It.Is<Notificacao>(
                not => not.Mensagem == "Conta de origem e destino não podem ser a mesma.")), Times.Once);
        }

        #endregion

        #region Cenários de Validação (Contas Não Encontradas)

        [Fact(DisplayName = "Validação: Conta de origem não encontrada")]
        public async Task Validacao_DeveRetornarNullENotificar_QuandoContaOrigemNaoEncontrada()
        {
            // Arrange
            var command = new TransferirCommand { DocumentoOrigem = "111", DocumentoDestino = "222", Valor = 10.00m };
            _contaRepositoryMock.Setup(r => r.ObterContaAsync("111", It.IsAny<CancellationToken>())).ReturnsAsync((Conta)null!);
            _contaRepositoryMock.Setup(r => r.ObterContaAsync("222", It.IsAny<CancellationToken>())).ReturnsAsync(new Conta("Lucas", "222"));

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(resultado);
            _notificadorMock.Verify(n => n.Disparar(It.Is<Notificacao>(
                not => not.Mensagem == "Conta de origem não encontrada.")), Times.Once);
            _transacaoRepositoryMock.Verify(r => r.ProcessarTransferenciaAsync(
                It.IsAny<Conta>(), It.IsAny<Conta>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "Validação: Conta de destino não encontrada")]
        public async Task Validacao_DeveRetornarNullENotificar_QuandoDestinoNaoEncontrada()
        {
            // Arrange
            var command = new TransferirCommand { DocumentoOrigem = "111", DocumentoDestino = "222", Valor = 10.00m };
            _contaRepositoryMock.Setup(r => r.ObterContaAsync("111", It.IsAny<CancellationToken>())).ReturnsAsync(new Conta("José", "111"));
            _contaRepositoryMock.Setup(r => r.ObterContaAsync("222", It.IsAny<CancellationToken>())).ReturnsAsync((Conta)null!);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(resultado);
            _notificadorMock.Verify(n => n.Disparar(It.Is<Notificacao>(
                not => not.Mensagem == "Conta de destino não encontrada.")), Times.Once);
            _transacaoRepositoryMock.Verify(r => r.ProcessarTransferenciaAsync(
                It.IsAny<Conta>(), It.IsAny<Conta>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region Cenários de Validação (Regras de Entidade)

        [Fact(DisplayName = "Validação: Conta de origem ou destino inativa")]
        public async Task Validacao_DeveRetornarNullENotificar_QuandoContasEstaoInativas()
        {
            // Arrange
            var command = new TransferirCommand { DocumentoOrigem = "111", DocumentoDestino = "222", Valor = 10.00m };
            var origem = new Conta("Lucas", "111");
            var destino = new Conta("João", "222");

            origem.Inativar();

            _contaRepositoryMock.Setup(r => r.ObterContaAsync("111", It.IsAny<CancellationToken>())).ReturnsAsync(origem);
            _contaRepositoryMock.Setup(r => r.ObterContaAsync("222", It.IsAny<CancellationToken>())).ReturnsAsync(destino);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(resultado);
            _notificadorMock.Verify(n => n.Disparar(It.Is<Notificacao>(
                not => not.Mensagem == "Ambas as contas devem estar ativas.")), Times.Once);
            _transacaoRepositoryMock.Verify(r => r.ProcessarTransferenciaAsync(
                It.IsAny<Conta>(), It.IsAny<Conta>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory(DisplayName = "Validação: Valor de transferência não positivo")]
        [InlineData(0.00)]
        [InlineData(-10.00)]
        public async Task Validacao_DeveRetornarNullENotificar_QuandoValorNaoEPositivo(decimal valorInvalido)
        {
            // Arrange
            var command = new TransferirCommand { DocumentoOrigem = "111", DocumentoDestino = "222", Valor = valorInvalido };
            var origem = new Conta("Thales", "111");
            var destino = new Conta("Maria", "222");

            _contaRepositoryMock.Setup(r => r.ObterContaAsync("111", It.IsAny<CancellationToken>())).ReturnsAsync(origem);
            _contaRepositoryMock.Setup(r => r.ObterContaAsync("222", It.IsAny<CancellationToken>())).ReturnsAsync(destino);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(resultado);
            _notificadorMock.Verify(n => n.Disparar(It.Is<Notificacao>(
                not => not.Mensagem == "Valor da transferência deve ser positivo.")), Times.Once);
            _transacaoRepositoryMock.Verify(r => r.ProcessarTransferenciaAsync(
                It.IsAny<Conta>(), It.IsAny<Conta>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "Validação: Saldo insuficiente na conta de origem")]
        public async Task Validacao_DeveRetornarNullENotificar_QuandoSaldoInsuficiente()
        {
            // Arrange
            const decimal valorTransferencia = 2000.00m;
            var command = new TransferirCommand { DocumentoOrigem = "111", DocumentoDestino = "222", Valor = valorTransferencia };
            var origem = new Conta("José", "111");
            var destino = new Conta("Joana", "222");

            _contaRepositoryMock.Setup(r => r.ObterContaAsync("111", It.IsAny<CancellationToken>())).ReturnsAsync(origem);
            _contaRepositoryMock.Setup(r => r.ObterContaAsync("222", It.IsAny<CancellationToken>())).ReturnsAsync(destino);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(resultado);
            _notificadorMock.Verify(n => n.Disparar(It.Is<Notificacao>(
                not => not.Mensagem == "Saldo insuficiente na conta de origem.")), Times.Once);
            _transacaoRepositoryMock.Verify(r => r.ProcessarTransferenciaAsync(
                It.IsAny<Conta>(), It.IsAny<Conta>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region Cenários de Falha na Persistência/Tratamento de Exceção

        [Fact(DisplayName = "Falha: Processamento da transação retorna null")]
        public async Task Handle_DeveRetornarNull_QuandoProcessarTransferenciaAsyncRetornaNull()
        {
            // Arrange
            const decimal valorTransferencia = 10.00m;
            var contaOrigem = new Conta("Lima", "111");
            var contaDestino = new Conta("Ricardo", "222");
            var command = new TransferirCommand { DocumentoOrigem = "111", DocumentoDestino = "222", Valor = valorTransferencia };

            _contaRepositoryMock.Setup(r => r.ObterContaAsync("111", It.IsAny<CancellationToken>())).ReturnsAsync(contaOrigem);
            _contaRepositoryMock.Setup(r => r.ObterContaAsync("222", It.IsAny<CancellationToken>())).ReturnsAsync(contaDestino);

            _transacaoRepositoryMock
                .Setup(r => r.ProcessarTransferenciaAsync(
                    It.IsAny<Conta>(), It.IsAny<Conta>(), valorTransferencia, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Transacao?)null);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(resultado);

            _notificadorMock.Verify(n => n.Disparar(It.IsAny<Notificacao>()), Times.Never);
        }

        [Fact(DisplayName = "Falha: Processamento da transação lança exceção")]
        public async Task Handle_DeveCapturarExcecaoENotificar_QuandoTransacaoFalha()
        {
            // Arrange
            const decimal valorTransferencia = 10.00m;
            var contaOrigem = new Conta("Lima", "111");
            var contaDestino = new Conta("Ricardo", "222");
            var command = new TransferirCommand { DocumentoOrigem = "111", DocumentoDestino = "222", Valor = valorTransferencia };
            var mensagemErro = "Database connection error.";

            _contaRepositoryMock.Setup(r => r.ObterContaAsync("111", It.IsAny<CancellationToken>())).ReturnsAsync(contaOrigem);
            _contaRepositoryMock.Setup(r => r.ObterContaAsync("222", It.IsAny<CancellationToken>())).ReturnsAsync(contaDestino);

            _transacaoRepositoryMock
                .Setup(r => r.ProcessarTransferenciaAsync(
                    It.IsAny<Conta>(), It.IsAny<Conta>(), valorTransferencia, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(resultado);

            _notificadorMock.Verify(n => n.Disparar(It.Is<Notificacao>(
                not => not.Mensagem.Contains($"Erro ao processar transferência: {mensagemErro}"))), Times.Once);
        }
        #endregion
    }
}
