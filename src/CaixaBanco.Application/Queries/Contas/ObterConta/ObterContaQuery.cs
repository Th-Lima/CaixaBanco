using CaixaBanco.Application.Responses.Contas;
using MediatR;

namespace CaixaBanco.Application.Queries.Contas.ObterConta
{
    public class ObterContaQuery : IRequest<ContaResponse?>
    {
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
    }
}
