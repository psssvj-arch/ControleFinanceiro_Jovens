namespace MoneyTeen.Models;

public enum TipoTransacao
{
    Receita,
    Despesa
}

public class Transacao
{
    private string _descricao = "";
    private decimal _valor;
    private DateTime _data = DateTime.Today;
    private TipoTransacao _tipo;

    public int Id { get; set; }

    public string Descricao
    {
        get { return _descricao; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("A descricao e obrigatoria.");

            string descricao = value.Trim();

            if (descricao.Length > 150)
                throw new ArgumentException("A descricao deve ter no maximo 150 caracteres.");

            _descricao = descricao;
        }
    }

    public decimal Valor
    {
        get { return _valor; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("O valor deve ser maior que zero.");

            _valor = value;
        }
    }

    public DateTime Data
    {
        get { return _data; }
        set
        {
            if (value.Date > DateTime.Today)
                throw new ArgumentException("A data nao pode ser no futuro.");

            if (value.Year < 2000)
                throw new ArgumentException("Data invalida.");

            _data = value.Date;
        }
    }

    public TipoTransacao Tipo
    {
        get { return _tipo; }
        set
        {
            if (!Enum.IsDefined(typeof(TipoTransacao), value))
                throw new ArgumentException("Tipo de transacao invalido.");

            _tipo = value;
        }
    }

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public bool EhReceita()
    {
        return Tipo == TipoTransacao.Receita;
    }

    public void Editar(string descricao, decimal valor, DateTime data, TipoTransacao tipo, int categoriaId)
    {
        Transacao temporaria = new Transacao();

        temporaria.Descricao = descricao;
        temporaria.Valor = valor;
        temporaria.Data = data;
        temporaria.Tipo = tipo;

        Descricao = temporaria.Descricao;
        Valor = temporaria.Valor;
        Data = temporaria.Data;
        Tipo = temporaria.Tipo;
        CategoriaId = categoriaId;
    }
}