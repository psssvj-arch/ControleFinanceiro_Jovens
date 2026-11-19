using Microsoft.EntityFrameworkCore;
using MoneyTeen.Models;

namespace MoneyTeen.Data;

public class AppDbContext : DbContext
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }
    public DbSet<Meta> Metas { get; set; }
    public DbSet<Orcamento> Orcamentos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conexao = Environment.GetEnvironmentVariable("MONEYTEEN_DB")
            ?? "server=localhost;database=moneyteen;user=root;password=SUA_SENHA;";

        optionsBuilder.UseMySql(conexao, ServerVersion.AutoDetect(conexao));
    }
} 
