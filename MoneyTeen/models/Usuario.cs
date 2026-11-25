
namespace MoneyTeen.Models;

public class Usuario
{
    private string _nome = "";
    private string _email = "";

    public int Id { get; set; }

    public string Nome
    {
        get { return _nome; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("O nome é obrigatório.");

            _nome = value.Trim();
        }
    }

    public string Email
    {
        get { return _email; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("O e-mail é obrigatório.");

            string email = value.Trim();
            int arroba = email.IndexOf('@');

            if (arroba <= 0 || !email.Substring(arroba + 1).Contains('.') || email.EndsWith('.'))
                throw new ArgumentException("E-mail inválido.");

            _email = email;
        }
    }

    public List<Transacao> Transacoes { get; set; } = new List<Transacao>();
    public List<Meta> Metas { get; set; } = new List<Meta>();
    public List<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();
}
