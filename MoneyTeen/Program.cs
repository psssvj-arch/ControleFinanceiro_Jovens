
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MoneyTeen.Data;
using MoneyTeen.Models;

class Program
{
    static void Main(string[] args)
    {
        using AppDbContext banco = new AppDbContext();

        try
        {
            banco.Database.Migrate();
            CriarCategorias(banco);

            Usuario usuario = CadastrarUsuario(banco);

            int opcao = 0;

            while (opcao != 8)
            {
                Console.Clear();

                Console.WriteLine("===== MONEYTEEN =====");
                Console.WriteLine("1 - Inicio");
                Console.WriteLine("2 - Meu dinheiro");
                Console.WriteLine("3 - Receitas");
                Console.WriteLine("4 - Despesas");
                Console.WriteLine("5 - Metas de economia");
                Console.WriteLine("6 - Relatorios");
                Console.WriteLine("7 - Editar cadastros");
                Console.WriteLine("8 - Sair");

                opcao = LerNumero("Escolha uma opcao: ");

                if (opcao == 1)
                    Inicio(usuario);
                else if (opcao == 2)
                    MeuDinheiro(banco, usuario);
                else if (opcao == 3)
                    AdicionarTransacao(banco, usuario, TipoTransacao.Receita);
                else if (opcao == 4)
                    AdicionarTransacao(banco, usuario, TipoTransacao.Despesa);
                else if (opcao == 5)
                    MetasEconomia(banco, usuario);
                else if (opcao == 6)
                    Relatorios(banco, usuario);
                else if (opcao == 7)
                    EditarCadastros(banco, usuario);
                else if (opcao == 8)
                    Console.WriteLine("Saindo do MoneyTeen...");
                else
                {
                    Console.WriteLine("Opcao invalida!");
                    Pausar();
                }
            }
        }
        catch (Exception erro)
        {
            Console.WriteLine("Ocorreu um erro: " + erro.Message);
            Pausar();
        }
    }

    static void CriarCategorias(AppDbContext banco)
    {
        string[] nomes = {
            "Alimentacao", "Transporte", "Lazer", "Estudos", "Outros"
        };

        foreach (string nome in nomes)
        {
            bool existe = banco.Categorias.Any(c => c.Nome == nome);

            if (!existe)
                banco.Categorias.Add(new Categoria { Nome = nome });
        }

        banco.SaveChanges();
    }

    static Usuario CadastrarUsuario(AppDbContext banco)
    {
        Usuario? usuario = banco.Usuarios.FirstOrDefault();

        if (usuario != null)
            return usuario;

        Console.Clear();
        Console.WriteLine("===== CADASTRO =====");

        while (true)
        {
            try
            {
                usuario = new Usuario();

                Console.Write("Digite seu nome: ");
                usuario.Nome = Console.ReadLine() ?? "";

                Console.Write("Digite seu e-mail: ");
                usuario.Email = Console.ReadLine() ?? "";

                banco.Usuarios.Add(usuario);
                banco.SaveChanges();

                Console.WriteLine("Cadastro realizado com sucesso!");
                Pausar();

                return usuario;
            }
            catch (ArgumentException erro)
            {
                Console.WriteLine(erro.Message);
                Console.WriteLine("Tente novamente.");
            }
        }
    }

    static void Inicio(Usuario usuario)
    {
        Console.Clear();

        Console.WriteLine("===== INICIO =====");
        Console.WriteLine("Bem-vindo ao MoneyTeen, " + usuario.Nome + "!");
        Console.WriteLine();
        Console.WriteLine("Organize seu dinheiro e acompanhe suas metas.");

        Pausar();
    }

    static void MeuDinheiro(AppDbContext banco, Usuario usuario)
    {
        Console.Clear();

        var transacoes = banco.Transacoes
            .Where(t => t.UsuarioId == usuario.Id)
            .ToList();

        decimal receitas = 0;
        decimal despesas = 0;

        foreach (Transacao t in transacoes)
        {
            if (t.Tipo == TipoTransacao.Receita)
                receitas += t.Valor;
            else
                despesas += t.Valor;
        }

        Console.WriteLine("===== MEU DINHEIRO =====");
        Console.WriteLine("Receitas: R$ " + receitas.ToString("F2"));
        Console.WriteLine("Despesas: R$ " + despesas.ToString("F2"));
        Console.WriteLine("Saldo: R$ " + (receitas - despesas).ToString("F2"));
        Console.WriteLine();

        Console.WriteLine("1 - Criar orcamento");
        Console.WriteLine("2 - Ver orcamentos");
        Console.WriteLine("3 - Voltar");

        int opcao = LerNumero("Escolha uma opcao: ");

        if (opcao == 1)
            CriarOrcamento(banco, usuario);
        else if (opcao == 2)
            VerOrcamentos(banco, usuario);
    }

    static void CriarOrcamento(AppDbContext banco, Usuario usuario)
    {
        Console.Clear();
        Console.WriteLine("===== NOVO ORCAMENTO =====");

        try
        {
            Orcamento orcamento = new Orcamento();

            Console.Write("Digite o valor limite: R$ ");
            orcamento.ValorLimite = LerDecimal();

            Categoria categoria = SelecionarCategoria(banco);

            int mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;

            bool existe = banco.Orcamentos.Any(o =>
                o.UsuarioId == usuario.Id &&
                o.CategoriaId == categoria.Id &&
                o.Mes == mes &&
                o.Ano == ano);

            if (existe)
            {
                Console.WriteLine("Ja existe um orcamento para essa categoria neste mes.");
                Pausar();
                return;
            }

            orcamento.CategoriaId = categoria.Id;
            orcamento.UsuarioId = usuario.Id;
            orcamento.Mes = mes;
            orcamento.Ano = ano;

            banco.Orcamentos.Add(orcamento);
            banco.SaveChanges();

            Console.WriteLine("Orcamento criado com sucesso!");
        }
        catch (ArgumentException erro)
        {
            Console.WriteLine(erro.Message);
        }

        Pausar();
    }

    static void VerOrcamentos(AppDbContext banco, Usuario usuario)
    {
        Console.Clear();
        Console.WriteLine("===== MEUS ORCAMENTOS =====");

        var orcamentos = banco.Orcamentos
            .Where(o => o.UsuarioId == usuario.Id)
            .ToList();

        if (orcamentos.Count == 0)
        {
            Console.WriteLine("Nenhum orcamento cadastrado.");
            Pausar();
            return;
        }

        foreach (Orcamento o in orcamentos)
        {
            decimal gasto = banco.Transacoes
                .Where(t => t.UsuarioId == usuario.Id &&
                    t.Tipo == TipoTransacao.Despesa &&
                    t.CategoriaId == o.CategoriaId &&
                    t.Data.Month == o.Mes &&
                    t.Data.Year == o.Ano)
                .AsEnumerable()
                .Sum(t => t.Valor);

            Categoria? categoria = banco.Categorias
                .FirstOrDefault(c => c.Id == o.CategoriaId);

            Console.WriteLine();
            Console.WriteLine("Categoria: " + categoria?.Nome);
            Console.WriteLine("Mes: " + o.Mes + "/" + o.Ano);
            Console.WriteLine("Limite: R$ " + o.ValorLimite.ToString("F2"));
            Console.WriteLine("Gasto: R$ " + gasto.ToString("F2"));
            Console.WriteLine("Percentual gasto: " + o.CalcularPercentualGasto(gasto) + "%");

            if (o.Estourou(gasto))
                Console.WriteLine("Limite ultrapassado!");
            else
                Console.WriteLine("Dentro do limite.");
        }

        Pausar();
    }

    static void AdicionarTransacao(AppDbContext banco, Usuario usuario, TipoTransacao tipo)
    {
        Console.Clear();

        Console.WriteLine(tipo == TipoTransacao.Receita
            ? "===== NOVA RECEITA ====="
            : "===== NOVA DESPESA =====");

        try
        {
            Transacao transacao = new Transacao();

            Console.Write("Digite a descricao: ");
            transacao.Descricao = Console.ReadLine() ?? "";

            Console.Write("Digite o valor: R$ ");
            transacao.Valor = LerDecimal();

            Categoria categoria = SelecionarCategoria(banco);

            Console.Write("Digite a data (dd/MM/yyyy) ou ENTER para hoje: ");
            string entrada = Console.ReadLine() ?? "";

            DateTime data = DateTime.Today;

            if (entrada != "" && !DateTime.TryParseExact(
                entrada, "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out data))
            {
                throw new ArgumentException("Data invalida.");
            }

            transacao.Data = data;
            transacao.Tipo = tipo;
            transacao.CategoriaId = categoria.Id;
            transacao.UsuarioId = usuario.Id;

            banco.Transacoes.Add(transacao);
            banco.SaveChanges();

            Console.WriteLine("Transacao cadastrada com sucesso!");
        }
        catch (ArgumentException erro)
        {
            Console.WriteLine(erro.Message);
        }

        Pausar();
    }

    static Categoria SelecionarCategoria(AppDbContext banco)
    {
        Console.WriteLine();
        Console.WriteLine("===== CATEGORIAS =====");

        var categorias = banco.Categorias.ToList();

        foreach (Categoria c in categorias)
            Console.WriteLine(c.Id + " - " + c.Nome);

        while (true)
        {
            int id = LerNumero("Escolha uma categoria: ");

            Categoria? categoria = banco.Categorias
                .FirstOrDefault(c => c.Id == id);

            if (categoria != null)
                return categoria;

            Console.WriteLine("Categoria invalida! Tente novamente.");
        }
    }

    static void MetasEconomia(AppDbContext banco, Usuario usuario)
    {
        Console.Clear();

        Console.WriteLine("===== METAS DE ECONOMIA =====");
        Console.WriteLine("1 - Criar meta");
        Console.WriteLine("2 - Ver metas");
        Console.WriteLine("3 - Depositar em uma meta");
        Console.WriteLine("4 - Voltar");

        int opcao = LerNumero("Escolha uma opcao: ");

        if (opcao == 1)
            CriarMeta(banco, usuario);
        else if (opcao == 2)
            VerMetas(banco, usuario);
        else if (opcao == 3)
            DepositarMeta(banco, usuario);
    }

    static void CriarMeta(AppDbContext banco, Usuario usuario)
    {
        Console.Clear();
        Console.WriteLine("===== NOVA META =====");

        try
        {
            Meta meta = new Meta();

            Console.Write("Nome da meta: ");
            meta.Nome = Console.ReadLine() ?? "";

            Console.Write("Valor desejado: R$ ");
            meta.ValorAlvo = LerDecimal();

            Console.Write("Prazo (dd/MM/yyyy): ");
            string entrada = Console.ReadLine() ?? "";

            if (!DateTime.TryParseExact(
                entrada, "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime prazo))
            {
                throw new ArgumentException("Data invalida.");
            }

            meta.Prazo = prazo;
            meta.ValorAtual = 0;
            meta.UsuarioId = usuario.Id;

            banco.Metas.Add(meta);
            banco.SaveChanges();

            Console.WriteLine("Meta criada com sucesso!");
        }
        catch (ArgumentException erro)
        {
            Console.WriteLine(erro.Message);
        }

        Pausar();
    }

    static void VerMetas(AppDbContext banco, Usuario usuario)
    {
        Console.Clear();
        Console.WriteLine("===== MINHAS METAS =====");

        var metas = banco.Metas
            .Where(m => m.UsuarioId == usuario.Id)
            .ToList();

        if (metas.Count == 0)
        {
            Console.WriteLine("Nenhuma meta cadastrada.");
            Pausar();
            return;
        }

        foreach (Meta m in metas)
        {
            Console.WriteLine();
            Console.WriteLine("ID: " + m.Id);
            Console.WriteLine("Meta: " + m.Nome);
            Console.WriteLine("Valor desejado: R$ " + m.ValorAlvo.ToString("F2"));
            Console.WriteLine("Valor guardado: R$ " + m.ValorAtual.ToString("F2"));
            Console.WriteLine("Progresso: " + m.CalcularPercentual() + "%");
            Console.WriteLine("Prazo: " + m.Prazo.ToString("dd/MM/yyyy"));

            if (m.EstaConcluida())
                Console.WriteLine("Meta concluida!");
        }

        Pausar();
    }

    static void DepositarMeta(AppDbContext banco, Usuario usuario)
    {
        Console.Clear();
        Console.WriteLine("===== DEPOSITO NA META =====");

        var metas = banco.Metas
            .Where(m => m.UsuarioId == usuario.Id)
            .ToList();

        if (metas.Count == 0)
        {
            Console.WriteLine("Nenhuma meta cadastrada.");
            Pausar();
            return;
        }

        foreach (Meta m in metas)
            Console.WriteLine(m.Id + " - " + m.Nome);

        int id = LerNumero("Escolha uma meta: ");

        Meta? meta = banco.Metas.FirstOrDefault(
            m => m.Id == id && m.UsuarioId == usuario.Id);

        if (meta == null)
        {
            Console.WriteLine("Meta nao encontrada.");
            Pausar();
            return;
        }

        try
        {
            Console.Write("Valor do deposito: R$ ");
            decimal valor = LerDecimal();

            meta.Depositar(valor);
            banco.SaveChanges();

            Console.WriteLine("Deposito realizado com sucesso!");
            Console.WriteLine("Progresso: " + meta.CalcularPercentual() + "%");
        }
        catch (ArgumentException erro)
        {
            Console.WriteLine(erro.Message);
        }

        Pausar();
    }

    static void Relatorios(AppDbContext banco, Usuario usuario)
    {
        Console.Clear();

        decimal receitas = 0;
        decimal despesas = 0;

        var transacoes = banco.Transacoes
            .Where(t => t.UsuarioId == usuario.Id)
            .OrderByDescending(t => t.Data)
            .ToList();

        foreach (Transacao t in transacoes)
        {
            if (t.Tipo == TipoTransacao.Receita)
                receitas += t.Valor;
            else
                despesas += t.Valor;
        }

        Console.WriteLine("===== RELATORIOS =====");
        Console.WriteLine("Total de receitas: R$ " + receitas.ToString("F2"));
        Console.WriteLine("Total de despesas: R$ " + despesas.ToString("F2"));
        Console.WriteLine("Saldo final: R$ " + (receitas - despesas).ToString("F2"));

        Console.WriteLine();
        Console.WriteLine("===== HISTORICO =====");

        if (transacoes.Count == 0)
            Console.WriteLine("Nenhuma transacao cadastrada.");

        foreach (Transacao t in transacoes)
        {
            Categoria? categoria = banco.Categorias
                .FirstOrDefault(c => c.Id == t.CategoriaId);

            Console.WriteLine();
            Console.WriteLine("Descricao: " + t.Descricao);
            Console.WriteLine("Tipo: " + t.Tipo);
            Console.WriteLine("Valor: R$ " + t.Valor.ToString("F2"));
            Console.WriteLine("Categoria: " + categoria?.Nome);
            Console.WriteLine("Data: " + t.Data.ToString("dd/MM/yyyy"));
        }

        Pausar();
    }

    static void EditarCadastros(AppDbContext banco, Usuario usuario)
    {
        Console.Clear();

        Console.WriteLine("===== EDITAR CADASTROS =====");
        Console.WriteLine("1 - Editar usuario");
        Console.WriteLine("2 - Editar receitas e despesas");
        Console.WriteLine("3 - Editar categorias");
        Console.WriteLine("4 - Editar metas");
        Console.WriteLine("5 - Editar orcamentos");
        Console.WriteLine("6 - Voltar");

        int opcao = LerNumero("Escolha uma opcao: ");

        if (opcao == 1)
            EditarUsuario(banco, usuario);
        else if (opcao == 2)
            EditarTransacao(banco, usuario);
        else if (opcao == 3)
            EditarCategoria(banco);
        else if (opcao == 4)
            EditarMeta(banco, usuario);
        else if (opcao == 5)
            EditarOrcamento(banco, usuario);
    }

    static void EditarUsuario(AppDbContext banco, Usuario usuario)
    {
        Console.Clear();
        Console.WriteLine("===== EDITAR USUARIO =====");
        Console.WriteLine("DADOS ATUAIS");
        Console.WriteLine("ID: " + usuario.Id);
        Console.WriteLine("Nome: " + usuario.Nome);
        Console.WriteLine("E-mail: " + usuario.Email);
        Console.WriteLine();

        Console.Write("Novo nome (ENTER para manter): ");
        string nome = Console.ReadLine() ?? "";

        Console.Write("Novo e-mail (ENTER para manter): ");
        string email = Console.ReadLine() ?? "";

        try
        {
            usuario.Editar(
                nome == "" ? usuario.Nome : nome,
                email == "" ? usuario.Email : email);

            banco.SaveChanges();
            Console.WriteLine("Usuario editado com sucesso!");
        }
        catch (ArgumentException erro)
        {
            banco.Entry(usuario).Reload();
            Console.WriteLine(erro.Message);
        }

        Pausar();
    }

    static void EditarTransacao(AppDbContext banco, Usuario usuario)
    {
        Console.Clear();
        Console.WriteLine("===== EDITAR RECEITAS E DESPESAS =====");

        var transacoes = banco.Transacoes
            .Where(t => t.UsuarioId == usuario.Id)
            .ToList();

        if (transacoes.Count == 0)
        {
            Console.WriteLine("Nenhuma transacao cadastrada.");
            Pausar();
            return;
        }

        foreach (Transacao t in transacoes)
        {
            Categoria? cat = banco.Categorias.FirstOrDefault(c => c.Id == t.CategoriaId);
            Console.WriteLine();
            Console.WriteLine("ID: " + t.Id);
            Console.WriteLine("Descricao: " + t.Descricao);
            Console.WriteLine("Tipo: " + t.Tipo);
            Console.WriteLine("Valor: R$ " + t.Valor.ToString("F2"));
            Console.WriteLine("Data: " + t.Data.ToString("dd/MM/yyyy"));
            Console.WriteLine("Categoria: " + cat?.Nome);
        }

        int id = LerNumero("Digite o ID da transacao: ");

        Transacao? transacao = banco.Transacoes.FirstOrDefault(
            t => t.Id == id && t.UsuarioId == usuario.Id);

        if (transacao == null)
        {
            Console.WriteLine("Transacao nao encontrada.");
            Pausar();
            return;
        }

        Categoria? categoriaAtual = banco.Categorias
            .FirstOrDefault(c => c.Id == transacao.CategoriaId);

        Console.WriteLine();
        Console.WriteLine("===== DADOS ATUAIS DA TRANSACAO =====");
        Console.WriteLine("ID: " + transacao.Id);
        Console.WriteLine("Descricao: " + transacao.Descricao);
        Console.WriteLine("Valor: R$ " + transacao.Valor.ToString("F2"));
        Console.WriteLine("Data: " + transacao.Data.ToString("dd/MM/yyyy"));
        Console.WriteLine("Tipo: " + transacao.Tipo);
        Console.WriteLine("Categoria: " + categoriaAtual?.Nome);
        Console.WriteLine();

        try
        {
            Console.Write("Nova descricao (ENTER para manter): ");
            string descricao = Console.ReadLine() ?? "";

            Console.Write("Novo valor (ENTER para manter): R$ ");
            decimal valor = LerDecimalOpcional(transacao.Valor);

            Console.Write("Nova data (dd/MM/yyyy ou ENTER para manter): ");
            DateTime data = LerDataOpcional(transacao.Data);

            Console.WriteLine("Tipo atual: " + transacao.Tipo);
            Console.WriteLine("1 - Receita");
            Console.WriteLine("2 - Despesa");
            int tipo = LerNumeroOpcional(
                transacao.Tipo == TipoTransacao.Receita ? 1 : 2);

            if (tipo != 1 && tipo != 2)
                throw new ArgumentException("Tipo invalido.");

            Categoria categoria = SelecionarCategoriaOpcional(banco, transacao.CategoriaId);

            transacao.Editar(
                descricao == "" ? transacao.Descricao : descricao,
                valor,
                data,
                tipo == 1 ? TipoTransacao.Receita : TipoTransacao.Despesa,
                categoria.Id);

            banco.SaveChanges();
            Console.WriteLine("Transacao editada com sucesso!");
        }
        catch (ArgumentException erro)
        {
            banco.Entry(transacao).Reload();
            Console.WriteLine(erro.Message);
        }

        Pausar();
    }

    static void EditarCategoria(AppDbContext banco)
    {
        Console.Clear();
        Console.WriteLine("===== EDITAR CATEGORIAS =====");

        var categorias = banco.Categorias.ToList();

        if (categorias.Count == 0)
        {
            Console.WriteLine("Nenhuma categoria cadastrada.");
            Pausar();
            return;
        }

        foreach (Categoria c in categorias)
            Console.WriteLine(c.Id + " - " + c.Nome);

        int id = LerNumero("Digite o ID da categoria: ");
        Categoria? categoria = banco.Categorias.FirstOrDefault(c => c.Id == id);

        if (categoria == null)
        {
            Console.WriteLine("Categoria nao encontrada.");
            Pausar();
            return;
        }

        Console.WriteLine();
        Console.WriteLine("===== DADOS ATUAIS DA CATEGORIA =====");
        Console.WriteLine("ID: " + categoria.Id);
        Console.WriteLine("Nome: " + categoria.Nome);
        Console.Write("Novo nome (ENTER para manter): ");
        string nome = Console.ReadLine() ?? "";

        if (nome == "")
        {
            Console.WriteLine("Nenhuma alteracao realizada.");
            Pausar();
            return;
        }

        bool existe = banco.Categorias.Any(c =>
            c.Nome.ToLower() == nome.Trim().ToLower() && c.Id != categoria.Id);

        if (existe)
        {
            Console.WriteLine("Ja existe uma categoria com esse nome.");
            Pausar();
            return;
        }

        try
        {
            categoria.Editar(nome);
            banco.SaveChanges();
            Console.WriteLine("Categoria editada com sucesso!");
        }
        catch (ArgumentException erro)
        {
            banco.Entry(categoria).Reload();
            Console.WriteLine(erro.Message);
        }

        Pausar();
    }

    static void EditarMeta(AppDbContext banco, Usuario usuario)
    {
        Console.Clear();
        Console.WriteLine("===== EDITAR METAS =====");

        var metas = banco.Metas
            .Where(m => m.UsuarioId == usuario.Id)
            .ToList();

        if (metas.Count == 0)
        {
            Console.WriteLine("Nenhuma meta cadastrada.");
            Pausar();
            return;
        }

        foreach (Meta m in metas)
        {
            Console.WriteLine();
            Console.WriteLine("ID: " + m.Id);
            Console.WriteLine("Nome: " + m.Nome);
            Console.WriteLine("Valor desejado: R$ " + m.ValorAlvo.ToString("F2"));
            Console.WriteLine("Valor guardado: R$ " + m.ValorAtual.ToString("F2"));
            Console.WriteLine("Prazo: " + m.Prazo.ToString("dd/MM/yyyy"));
        }

        int id = LerNumero("Digite o ID da meta: ");
        Meta? meta = banco.Metas.FirstOrDefault(
            m => m.Id == id && m.UsuarioId == usuario.Id);

        if (meta == null)
        {
            Console.WriteLine("Meta nao encontrada.");
            Pausar();
            return;
        }

        Console.WriteLine();
        Console.WriteLine("===== DADOS ATUAIS DA META =====");
        Console.WriteLine("ID: " + meta.Id);
        Console.WriteLine("Nome: " + meta.Nome);
        Console.WriteLine("Valor desejado: R$ " + meta.ValorAlvo.ToString("F2"));
        Console.WriteLine("Valor guardado: R$ " + meta.ValorAtual.ToString("F2"));
        Console.WriteLine("Prazo: " + meta.Prazo.ToString("dd/MM/yyyy"));
        Console.WriteLine("Progresso: " + meta.CalcularPercentual() + "%");
        Console.WriteLine();

        try
        {
            Console.Write("Novo nome (ENTER para manter): ");
            string nome = Console.ReadLine() ?? "";

            Console.Write("Novo valor desejado (ENTER para manter): R$ ");
            decimal valorAlvo = LerDecimalOpcional(meta.ValorAlvo);

            Console.Write("Novo valor guardado (ENTER para manter): R$ ");
            decimal valorAtual = LerDecimalNaoNegativoOpcional(meta.ValorAtual);

            Console.Write("Novo prazo (dd/MM/yyyy ou ENTER para manter): ");
            DateTime prazo = LerDataOpcional(meta.Prazo);

            meta.Editar(
                nome == "" ? meta.Nome : nome,
                valorAlvo,
                valorAtual,
                prazo);

            banco.SaveChanges();
            Console.WriteLine("Meta editada com sucesso!");
        }
        catch (ArgumentException erro)
        {
            banco.Entry(meta).Reload();
            Console.WriteLine(erro.Message);
        }

        Pausar();
    }

    static void EditarOrcamento(AppDbContext banco, Usuario usuario)
    {
        Console.Clear();
        Console.WriteLine("===== EDITAR ORCAMENTOS =====");

        var orcamentos = banco.Orcamentos
            .Where(o => o.UsuarioId == usuario.Id)
            .ToList();

        if (orcamentos.Count == 0)
        {
            Console.WriteLine("Nenhum orcamento cadastrado.");
            Pausar();
            return;
        }

        foreach (Orcamento o in orcamentos)
        {
            Categoria? cat = banco.Categorias.FirstOrDefault(c => c.Id == o.CategoriaId);
            Console.WriteLine();
            Console.WriteLine("ID: " + o.Id);
            Console.WriteLine("Categoria: " + cat?.Nome);
            Console.WriteLine("Limite: R$ " + o.ValorLimite.ToString("F2"));
            Console.WriteLine("Mes: " + o.Mes + "/" + o.Ano);
        }

        int id = LerNumero("Digite o ID do orcamento: ");
        Orcamento? orcamento = banco.Orcamentos.FirstOrDefault(
            o => o.Id == id && o.UsuarioId == usuario.Id);

        if (orcamento == null)
        {
            Console.WriteLine("Orcamento nao encontrado.");
            Pausar();
            return;
        }

        Categoria? categoriaAtual = banco.Categorias
            .FirstOrDefault(c => c.Id == orcamento.CategoriaId);

        Console.WriteLine();
        Console.WriteLine("===== DADOS ATUAIS DO ORCAMENTO =====");
        Console.WriteLine("ID: " + orcamento.Id);
        Console.WriteLine("Categoria: " + categoriaAtual?.Nome);
        Console.WriteLine("Valor limite: R$ " + orcamento.ValorLimite.ToString("F2"));
        Console.WriteLine("Mes: " + orcamento.Mes);
        Console.WriteLine("Ano: " + orcamento.Ano);
        Console.WriteLine();

        try
        {
            Console.Write("Novo valor limite (ENTER para manter): R$ ");
            decimal valor = LerDecimalOpcional(orcamento.ValorLimite);

            Console.Write("Novo mes (ENTER para manter): ");
            int mes = LerNumeroOpcional(orcamento.Mes);

            Console.Write("Novo ano (ENTER para manter): ");
            int ano = LerNumeroOpcional(orcamento.Ano);

            Categoria categoria = SelecionarCategoriaOpcional(banco, orcamento.CategoriaId);

            orcamento.Editar(valor, mes, ano, categoria.Id);
            banco.SaveChanges();
            Console.WriteLine("Orcamento editado com sucesso!");
        }
        catch (ArgumentException erro)
        {
            banco.Entry(orcamento).Reload();
            Console.WriteLine(erro.Message);
        }

        Pausar();
    }

    static Categoria SelecionarCategoriaOpcional(AppDbContext banco, int categoriaAtualId)
    {
        Console.WriteLine();
        Console.WriteLine("===== CATEGORIAS DISPONIVEIS =====");

        var categorias = banco.Categorias.ToList();

        foreach (Categoria c in categorias)
            Console.WriteLine(c.Id + " - " + c.Nome);

        Console.Write("Digite o ID da nova categoria ou ENTER para manter: ");
        string entrada = Console.ReadLine() ?? "";

        if (entrada == "")
        {
            Categoria? atual = banco.Categorias.FirstOrDefault(c => c.Id == categoriaAtualId);

            if (atual == null)
                throw new ArgumentException("Categoria atual nao encontrada.");

            return atual;
        }

        if (!int.TryParse(entrada, out int id))
            throw new ArgumentException("ID da categoria invalido.");

        Categoria? categoria = banco.Categorias.FirstOrDefault(c => c.Id == id);

        if (categoria == null)
            throw new ArgumentException("Categoria nao encontrada.");

        return categoria;
    }

    static int LerNumero(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            string entrada = Console.ReadLine() ?? "";

            if (int.TryParse(entrada, out int numero))
                return numero;

            Console.WriteLine("Digite um numero valido.");
        }
    }

    static int LerNumeroOpcional(int atual)
    {
        Console.Write("Atual: " + atual + ". Novo valor: ");
        string entrada = Console.ReadLine() ?? "";

        if (entrada == "")
            return atual;

        if (int.TryParse(entrada, out int numero))
            return numero;

        throw new ArgumentException("Numero invalido.");
    }

    static decimal LerDecimal()
    {
        while (true)
        {
            string entrada = Console.ReadLine() ?? "";

            if (decimal.TryParse(
                entrada,
                NumberStyles.Number,
                CultureInfo.GetCultureInfo("pt-BR"),
                out decimal valor) && valor > 0)
            {
                return valor;
            }

            Console.Write("Valor invalido! Digite um valor maior que zero: R$ ");
        }
    }

    static decimal LerDecimalOpcional(decimal atual)
    {
        string entrada = Console.ReadLine() ?? "";

        if (entrada == "")
            return atual;

        if (decimal.TryParse(
            entrada,
            NumberStyles.Number,
            CultureInfo.GetCultureInfo("pt-BR"),
            out decimal valor))
        {
            return valor;
        }

        throw new ArgumentException("Valor invalido.");
    }

    static decimal LerDecimalNaoNegativoOpcional(decimal atual)
    {
        string entrada = Console.ReadLine() ?? "";

        if (entrada == "")
            return atual;

        if (decimal.TryParse(
            entrada,
            NumberStyles.Number,
            CultureInfo.GetCultureInfo("pt-BR"),
            out decimal valor) && valor >= 0)
        {
            return valor;
        }

        throw new ArgumentException("Valor invalido.");
    }

    static DateTime LerDataOpcional(DateTime atual)
    {
        string entrada = Console.ReadLine() ?? "";

        if (entrada == "")
            return atual;

        if (DateTime.TryParseExact(
            entrada,
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime data))
        {
            return data;
        }

        throw new ArgumentException("Data invalida.");
    }

    static void Pausar()
    {
        Console.WriteLine();
        Console.WriteLine("Pressione ENTER para continuar.");
        Console.ReadLine();
    }
}
    