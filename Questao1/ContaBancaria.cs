namespace Questao1
{
    internal class ContaBancaria
    {
        public ContaBancaria(long numero, string nome, double saldo = 0)
        {
            Numero = numero;
            Nome = nome;
            Saldo = saldo;
        }

        private readonly double valorTarifaFixa = 3.50;
        public long Numero { get; init; }
        public string Nome { get; set; }
        public double Saldo { get; private set; }

        public void Depositar(double valor) => Saldo += valor;

        public void Sacar(double valor)
        {
            Saldo -= valor;
            Tarifar();
        }

        private void Tarifar()
        {
            Saldo -= valorTarifaFixa;
        }

        public override string ToString()
        {
            return $"conta {Numero}, Titular: {Nome}, Saldo: {Saldo:C}";
        }
    }
}