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
                throw new ArgumentException("O nome e obrigatorio.");

            string nome = value.Trim();

            if (nome.Any(char.IsDigit))
                throw new ArgumentException("O nome nao pode conter numeros.");

            if (nome.Length < 2 || nome.Length > 100)
                throw new ArgumentException("O nome deve ter entre 2 e 100 caracteres.");

            _nome = nome;
        }
    }

    public string Email
    {
        get { return _email; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("O e-mail e obrigatorio.");

            string email = value.Trim();

            int arroba = email.IndexOf('@');

            if (arroba <= 0 ||
                arroba != email.LastIndexOf('@') ||
                email.Contains(' ') ||
                email.EndsWith('.') ||
                email.Substring(arroba + 1).IndexOf('.') <= 0 ||
                email.Substring(arroba + 1).EndsWith('.'))
            {
                throw new ArgumentException("E-mail invalido.");
            }

            _email = email;
        }
    }

    public List<Transacao> Transacoes { get; set; } = new List<Transacao>();
    public List<Meta> Metas { get; set; } = new List<Meta>();
    public List<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();

    public void Editar(string nome, string email)
    {
        Usuario temporario = new Usuario();

        temporario.Nome = nome;
        temporario.Email = email;

        Nome = temporario.Nome;
        Email = temporario.Email;
    }
}