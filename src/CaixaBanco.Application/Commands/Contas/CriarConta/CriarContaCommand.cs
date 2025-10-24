using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CaixaBanco.Application.Commands.Contas.CriarConta
{
    /// <summary>
    /// Comando para criação de conta
    /// </summary>
    public class CriarContaCommand : IRequest<bool>
    {

        [Required]
        public string Nome { get; set; } = null!;

        [Required]
        public string Documento { get; set; } = null!;
    }
}
