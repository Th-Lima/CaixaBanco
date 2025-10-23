using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CaixaBanco.Application.Commands.Contas.InativarConta
{
    public class InativarContaCommand : IRequest<bool>
    {
        [Required]
        public string? Documento { get; set; }

        [Required]
        public string? UsuarioResponsavel { get; set; }
    }
}
