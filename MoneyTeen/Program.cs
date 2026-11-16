using System;

class Program
{
    static void Main(string[] args)
    {
        int opcao = 0;

        while (opcao != 7)
        {
            Console.Clear();

            Console.WriteLine("===== MONEYTEEN =====");
            Console.WriteLine("1 - Inicio");
            Console.WriteLine("2 - Meu dinheiro");
            Console.WriteLine("3 - Receitas");
            Console.WriteLine("4 - Despesas");
            Console.WriteLine("5 - Metas de economia");
            Console.WriteLine("6 - Relatorios");
            Console.WriteLine("7 - Sair");

            Console.WriteLine();
            Console.Write("Escolha uma opcao: ");

            opcao = int.Parse(Console.ReadLine() ?? "0");

            switch (opcao)
            {
                case 1:
                    Console.WriteLine("Bem-vindo ao MoneyTeen!");
                    break;

                case 2:
                    Console.WriteLine("Meu dinheiro");
                    break;

                case 3:
                    Console.WriteLine("Area de receitas");
                    break;

                case 4:
                    Console.WriteLine("Area de despesas");
                    break;

                case 5:
                    Console.WriteLine("Metas de economia");
                    break;

                case 6:
                    Console.WriteLine("Relatorios");
                    break;

                case 7:
                    Console.WriteLine("Saindo do MoneyTeen...");
                    break;

                default:
                    Console.WriteLine("Opcao invalida!");
                    break;
            }

            if (opcao != 7)
            {
                Console.WriteLine();
                Console.WriteLine("Pressione ENTER para continuar.");
                Console.ReadLine();
            }
        }
    }
}