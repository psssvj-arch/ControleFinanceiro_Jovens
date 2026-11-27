namespace MoneyTeen.Models;

public class Orcamento
{
    private decimal _valorLimite;
    private int _mes = 1;
    private int _ano = DateTime.Now.Year;

    public int Id { get; set; }

    public decimal ValorLimite
    {
        get { return _valorLimite; }
        set
        {
            if (value <= 0)
                throw new ArgumentException("O valor limite deve ser maior que zero.");

            _valorLimite = value;
        }
    }

    public int Mes
    {
        get { return _mes; }
        set
        {
            if (value < 1 || value > 12)
                throw new ArgumentException("O mes deve estar entre 1 e 12.");

            _mes = value;
        }
    }

    public int Ano
    {
        get { return _ano; }
        set
        {
            if (value < 2000 || value > 2100)
                throw new ArgumentException("Ano invalido.");

            _ano = value;
        }
    }

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public decimal CalcularPercentualGasto(decimal totalGasto)
    {
        if (_valorLimite == 0)
            return 0;

        return Math.Round((totalGasto / _valorLimite) * 100, 2);
    }

    public bool Estourou(decimal totalGasto)
    {
        return totalGasto > _valorLimite;
    }

    public void Editar(decimal valorLimite, int mes, int ano, int categoriaId)
    {
        Orcamento temporario = new Orcamento();

        temporario.ValorLimite = valorLimite;
        temporario.Mes = mes;
        temporario.Ano = ano;

        ValorLimite = temporario.ValorLimite;
        Mes = temporario.Mes;
        Ano = temporario.Ano;
        CategoriaId = categoriaId;
    }
}