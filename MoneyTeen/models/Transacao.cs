namespace MoneyTeen.Models;


public enum TipoTransacao { Receita, Despesa }

public class Transacao
{
    private string _descricao = "";
    private decimal _valor;
    private DateTime _data = DateTime.Today;

    public int Id { get; set; }

    public string Descricao
    {
        get { return _descricao; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("A descrição é obrigatória.");
            }
            if (value.Length > 150)
            {
                throw new ArgumentException("A descrição deve ter no máximo 150 letras.");
            }
            _descricao = value.Trim();
        }
    }

    public decimal Valor
    {
        get { return _valor; }
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("O valor deve ser maior que zero.");
            }
            _valor = value;
        }
    }

    public DateTime Data
    {
        get { return _data; }
        set
        {
            if (value.Date > DateTime.Today)
            {
                throw new ArgumentException("A data não pode ser no futuro.");
            }
            if (value.Year < 2000)
            {
                throw new ArgumentException("Data inválida.");
            }
            _data = value.Date;
        }
    }

    public TipoTransacao Tipo { get; set; }


    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }


    public bool EhReceita()
    {
        return Tipo == TipoTransacao.Receita;
    }
}
