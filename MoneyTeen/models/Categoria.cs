namespace MoneyTeen.Models;

public class Categoria
{
    private string _nome = "";

    public int Id { get; set; }

    public string Nome
    {
        get { return _nome; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("O nome da categoria é obrigatório.");
            }
            if (value.Length > 50)
            {
                throw new ArgumentException("O nome deve ter no máximo 50 letras.");
            }
            _nome = value.Trim();
        }
    }


    public List<Transacao> Transacoes { get; set; } = new List<Transacao>();
    public List<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();
}