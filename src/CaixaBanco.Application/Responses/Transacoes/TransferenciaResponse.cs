namespace CaixaBanco.Application.Responses.Transacoes
{
    public class TransferenciaResponse
    {
        public string? DocumentoOrigem { get; set; }
        public string? DocumentoDestino { get; set; }
        public decimal ValorContaDestinoAtualizado { get; set; }
        public decimal ValorContaOrigemAtualizado { get; set; }
        public decimal ValorTransacao { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
