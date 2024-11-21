using tabuleiro;
using xadrez;

namespace xadrez_console
{
  class Tela
  {
    public static void imprimirPartida(PartidaDeXadrez partida)
    {
    imprimirTabuleiro(partida.tab);
    Console.WriteLine();
    imprimirPecasCapturadas(partida);
    Console.WriteLine("Turno: " + partida.turno);

    if(!partida.terminada)
    {
      Console.WriteLine("Aguardando jogada: " + partida.jogadorAtual);
      if (partida.xeque)
      {
        Console.WriteLine("XEQUE!");
      }
    }
    else
    {
      Console.WriteLine("XEQUEMATE!");
      Console.WriteLine("Partida encerrada!");
      Console.WriteLine("Ganhador: " + partida.jogadorAtual);
    }
    }
    public static void imprimirPecasCapturadas(PartidaDeXadrez partida)
    {
      Console.WriteLine("Pecas capturadas: ");
      Console.Write("Brancas: ");
      imprimirCorBranca(partida);
      Console.WriteLine();
      Console.Write("Pretas: ");
      imprimirCorPreta(partida);
      Console.WriteLine();
    }
    public static void imprimirCorPreta(PartidaDeXadrez partida)
    {
      ConsoleColor aux = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.Yellow;
      imprimirConjunto(partida.pecasCapturadas(Cor.Preta));
      Console.ForegroundColor = aux;
    }
    public static void imprimirCorBranca(PartidaDeXadrez partida)
    {
      imprimirConjunto(partida.pecasCapturadas(Cor.Branca));
    }
    public static void imprimirConjunto(HashSet<Peca> conjunto)
    {
      Console.Write("[");
      foreach (Peca x in conjunto)
      {
        Console.Write(x + " ");
      }
      Console.WriteLine("]");
    }
    public static void imprimirTabuleiro(Tabuleiro tab)
    {
      for (int i = 0; i < tab.linhas; i++)
      {
        Console.Write(8 - i + " ");
        for (int j = 0; j < tab.colunas; j++)
        {
          imprimirPeca(tab.peca(i, j));
        }
        Console.WriteLine();
      }
      Console.Write("  a b c d e f g h");
    }
    public static void imprimirTabuleiro(Tabuleiro tab, bool[,] posicoesPossiveis)
    {
      ConsoleColor fundoOriginal = Console.BackgroundColor;
      ConsoleColor fundoAlterado = ConsoleColor.DarkGray;

      for (int i = 0; i < tab.linhas; i++)
      {
        Console.Write(8 - i + " ");
        for (int j = 0; j < tab.colunas; j++)
        {
          if (posicoesPossiveis[i,j] )
          {
            Console.BackgroundColor = fundoAlterado;
          }
          else
          {
            Console.BackgroundColor = fundoOriginal;
          }

          imprimirPeca(tab.peca(i, j));
          Console.BackgroundColor = fundoOriginal;
        }
        Console.WriteLine();
      }
      Console.WriteLine("  a b c d e f g h");

      Console.BackgroundColor = fundoOriginal;
    }
    public static void imprimirPeca(Peca peca)
    {
      if (peca == null)
      {
        Console.Write("- ");
      }
      else
      {
        if (peca.cor == Cor.Branca)
        {
          Console.Write(peca);
        }
        else
        {
          ConsoleColor aux = Console.ForegroundColor;
          Console.ForegroundColor = ConsoleColor.Yellow;
          Console.Write(peca);
          Console.ForegroundColor = aux;
        }
        Console.Write(" ");
      }
    }
    public static PosicaoXadrez lerPosicaoXadrez()
    {
      string s = Console.ReadLine();
      if(s.Length < 2) throw new TabuleiroException("Erro ao ler a posição!");
      char coluna = s[0];
      int linha = int.Parse(s[1] + "");
      PartidaDeXadrez.validaEntrada(coluna);
      PartidaDeXadrez.validaEntrada(linha);
      
      return new PosicaoXadrez(coluna, linha);
    }
  }
}