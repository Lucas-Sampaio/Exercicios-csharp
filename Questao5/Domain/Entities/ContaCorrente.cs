namespace Questao5.Domain.Entities
{
    public class ContaCorrente : Entity
    {
        public int Numero { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
    }
}
