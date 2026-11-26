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
            {
                throw new ArgumentException("O nome da meta é obrigatório.");
            }
            if (value.Length > 100)
            {
                throw new ArgumentException("O nome deve ter no máximo 100 letras.");
            }
            _nome = value.Trim();
        }
    }

    public decimal ValorAlvo
    {
        get { return _valorAlvo; }
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("O valor alvo deve ser maior que zero.");
            }
            _valorAlvo = value;
        }
    }

    public decimal ValorAtual
    {
        get { return _valorAtual; }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("O valor atual não pode ser negativo.");
            }
            _valorAtual = value;
        }
    }

    public DateTime Prazo
    {
        get { return _prazo; }
        set
        {
            if (value.Date <= DateTime.Today)
            {
                throw new ArgumentException("O prazo deve ser uma data futura.");
            }
            _prazo = value.Date;
        }
    }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }


    public void Depositar(decimal valor)
    {
        if (valor <= 0)
        {
            throw new ArgumentException("O depósito deve ser maior que zero.");
        }
        _valorAtual = _valorAtual + valor;
    }


    public decimal CalcularPercentual()
    {
        if (_valorAlvo == 0)
        {
            return 0;
        }
        decimal percentual = _valorAtual / _valorAlvo * 100;
        if (percentual > 100)
        {
            percentual = 100;
        }
        return Math.Round(percentual, 2);
    }


    public bool EstaConcluida()
    {
        return _valorAtual >= _valorAlvo;
    }
}
