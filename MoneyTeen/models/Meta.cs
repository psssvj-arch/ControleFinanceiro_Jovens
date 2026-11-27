namespace MoneyTeen.Models;

public class Meta
{
    private string _nome = "";
    private decimal _valorAlvo;
    private decimal _valorAtual;
    private DateTime _prazo = DateTime.Today.AddDays(1);

    public int Id { get; set; }

    public string Nome
    {
        get { return _nome; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("O nome da meta e obrigatorio.");

            string nome = value.Trim();

            if (nome.Length > 100)
                throw new ArgumentException("O nome deve ter no maximo 100 caracteres.");

            _nome = nome;
        }
    }

    public decimal ValorAlvo
    {
        get { return _valorAlvo; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("O valor alvo deve ser maior que zero.");

            _valorAlvo = value;
        }
    }

    public decimal ValorAtual
    {
        get { return _valorAtual; }
        set
        {
            if (value < 0)
                throw new ArgumentException("O valor atual nao pode ser negativo.");

            _valorAtual = value;
        }
    }

    public DateTime Prazo
    {
        get { return _prazo; }
        set
        {
            if (value.Date <= DateTime.Today)
                throw new ArgumentException("O prazo deve ser uma data futura.");

            _prazo = value.Date;
        }
    }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public void Depositar(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("O deposito deve ser maior que zero.");

        _valorAtual += valor;
    }

    public decimal CalcularPercentual()
    {
        if (_valorAlvo == 0)
            return 0;

        decimal percentual = (_valorAtual / _valorAlvo) * 100;

        if (percentual > 100)
            percentual = 100;

        return Math.Round(percentual, 2);
    }

    public bool EstaConcluida()
    {
        return _valorAtual >= _valorAlvo;
    }

    public void Editar(string nome, decimal valorAlvo, decimal valorAtual, DateTime prazo)
    {
        Meta temporaria = new Meta();

        temporaria.Nome = nome;
        temporaria.ValorAlvo = valorAlvo;
        temporaria.ValorAtual = valorAtual;
        temporaria.Prazo = prazo;

        Nome = temporaria.Nome;
        ValorAlvo = temporaria.ValorAlvo;
        ValorAtual = temporaria.ValorAtual;
        Prazo = temporaria.Prazo;
    }
}