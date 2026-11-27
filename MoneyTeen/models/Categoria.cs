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
                throw new ArgumentException("O nome da categoria e obrigatorio.");

            string nome = value.Trim();

            if (nome.Length > 50)
                throw new ArgumentException("O nome deve ter no maximo 50 caracteres.");

            _nome = nome;
        }
    }

    public List<Transacao> Transacoes { get; set; } = new List<Transacao>();
    public List<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();

    public void Editar(string nome)
    {
        Categoria temporaria = new Categoria();

        temporaria.Nome = nome;

        Nome = temporaria.Nome;
    }
}