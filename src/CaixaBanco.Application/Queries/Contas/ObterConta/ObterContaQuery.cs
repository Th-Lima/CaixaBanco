using CaixaBanco.Application.Responses.Contas;
using MediatR;

namespace CaixaBanco.Application.Queries.Contas.ObterConta
{
    /// <summary>
    /// Query para obter conta bancária por nome ou documento
    /// </summary>
    public class ObterContaQuery : IRequest<IEnumerable<ContaResponse>>
    {
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
    }
}
