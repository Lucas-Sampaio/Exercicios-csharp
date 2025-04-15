namespace Questao5.Domain.Entities
{
    public class Idempotencia(Guid id, string requisicao, string resultado) : Entity(id)
    {
        public string Requisicao { get; set; } = requisicao;
        public string Resultado { get; set; } = resultado;
    }
}