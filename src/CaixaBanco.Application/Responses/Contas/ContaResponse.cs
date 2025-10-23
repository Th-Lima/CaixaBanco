using CaixaBanco.Domain.Enums;

namespace CaixaBanco.Application.Responses.Contas
{
    public class ContaResponse
    {
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
        public DateTime DataAbertura { get; set; }
        public StatusConta Status { get; set; }
    }
}
