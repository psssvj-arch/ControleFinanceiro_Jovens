namespace MoneyTeen.Models;

public class Meta
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public decimal ValorAlvo { get; set; }
    public decimal ValorAtual { get; set; }
    public DateTime Prazo { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}