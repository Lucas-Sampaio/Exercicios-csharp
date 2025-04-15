namespace Questao5.Domain.Dtos
{
    public record IdempotenciaDto
    {
        public string Id { get; set; }
        public string Resultado { get; set; }
    }
}
