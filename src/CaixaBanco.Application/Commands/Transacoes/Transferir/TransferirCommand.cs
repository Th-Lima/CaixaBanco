using CaixaBanco.Application.Responses.Transacoes;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CaixaBanco.Application.Commands.Transacoes.Transferir
{
    /// <summary>
    /// Comando para transferir valor entre contas
    /// </summary>
    public class TransferirCommand : IRequest<TransferenciaResponse?>
    {
        [Required]
        public string DocumentoOrigem { get; set; } = null!;

        [Required]
        public string DocumentoDestino { get; set; } = null!;

        [Range(0.01, double.MaxValue)]
        public decimal Valor { get; set; }
    }
}
