using tabuleiro;

namespace xadrez
{
  class PartidaDeXadrez
  {
    public Tabuleiro tab {get; private set;}
    public int turno {get; private set;}
    public Cor jogadorAtual {get; private set;}
    public bool terminada { get; private set; }
    private HashSet<Peca> pecas;
    private HashSet<Peca> capturadas;
    public bool xeque { get; private set; }

    public PartidaDeXadrez()
    {
      tab = new Tabuleiro(8, 8);
      turno = 1;
      jogadorAtual = Cor.Branca;
      terminada = false;
      xeque = false;
      pecas = new HashSet<Peca>();
      capturadas = new HashSet<Peca>();
      colocarPecas();
    }
    public void realizaJogada(Posicao origem, Posicao destino)
    {
      Peca pecaCapturada = executarMovimento(origem, destino);
      if(estaEmCheck(jogadorAtual))
      {
        desfazMovimento(origem, destino, pecaCapturada);
        throw new TabuleiroException("Voceê não pode se colocar em check!");
      }
      if (estaEmCheck(adversaria(jogadorAtual)))
      {
        xeque = true;
      }
      else
      {
        xeque = false;
      }
      if(testeXequemate(adversaria(jogadorAtual)))
      {
        terminada = true;
      }
      else
      {
        turno++;
        mudaJogador();
      }
    }
    public Peca executarMovimento(Posicao origem, Posicao destino) 
    {
      Peca p = tab.retirarPeca(origem);
      p.incrementarQtdMovimentos();
      Peca pecaCapturada = tab.retirarPeca(destino);
      tab.ColocarPeca(p, destino);

      if (pecaCapturada != null)
      {
        capturadas.Add(pecaCapturada);
      }
      return pecaCapturada;
    }
    public void desfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
    {
      Peca p = tab.retirarPeca(destino);
      p.decrementarQtdMovimentos();

      if (pecaCapturada != null)
      {
        tab.ColocarPeca(pecaCapturada, destino);
        capturadas.Remove(pecaCapturada);
      }

      tab.ColocarPeca(p, origem);
    }
    public void validarPosicaoOrigem(Posicao pos)
    {
      if(tab.peca(pos) == null)
      {
        throw new TabuleiroException("Não existe peça na posição escolhida!");
      }
      if(jogadorAtual != tab.peca(pos).cor)
      {
        throw new TabuleiroException("A peça de escolha não é sua!");
      }
      if(!tab.peca(pos).existeMovimentosPossiveis())
      {
        throw new TabuleiroException("Não há movimentos possíveis para a peça!");
      }
    }
    public static void validaEntrada(char c)
    {
      if (c < 'a' || c > 'h')
      {
        throw new TabuleiroException("Coluna inválida!");
      }
    }
    public static void validaEntrada(int i)
    {
      if (i < 1 || i > 8)
      {
        throw new TabuleiroException("Linha inválida!");
      }
      if(i == null)
      {
        throw new TabuleiroException("Linha inválida!");
      }
    }
    private Cor adversaria(Cor cor)
    {
      if(cor == Cor.Branca)
      {
        return Cor.Preta;
      }
      else
      {
        return Cor.Branca;
      }
    }
    private Peca rei(Cor cor)
    {
      foreach (Peca x in pecasEmJogo(cor))
      {
        if(x is Rei)
        {
          return x;
        }
      }
      return null;
    }
    public bool estaEmCheck(Cor cor)
    {
      Peca R = rei(cor);
      if(R == null)
      {
        throw new TabuleiroException("Não tem rei da cor " + cor + " no tabuleiro!");
      }
      foreach (Peca x in pecasEmJogo(adversaria(cor)))
      {
        bool[,] mat = x.movimentosPossiveis();
        if(mat[R.posicao.linha, R.posicao.coluna])
        {
          return true;
        }
      }
      return false;
    }
    public void validarPosicaoDestino(Posicao origem, Posicao destino)
    {
      if(!tab.peca(origem).movimentoPossivel(destino))
      {
        throw new TabuleiroException("Posição inválida!");
      }
    }
    private void mudaJogador()
    {
      if(jogadorAtual == Cor.Branca)
      {
        jogadorAtual = Cor.Preta;
      }
      else
      {
        jogadorAtual = Cor.Branca;
      }
    }
    public HashSet<Peca> pecasCapturadas(Cor cor)
    {
      HashSet<Peca> aux = new HashSet<Peca>();
      foreach (Peca x in capturadas)
      {
        if(x.cor == cor)
        {
          aux.Add(x);
        }
      }
      return aux;
    }
    public HashSet<Peca> pecasEmJogo(Cor cor)
    {
      HashSet<Peca> aux = new HashSet<Peca>();
      foreach (Peca x in pecas)
      {
        if(x.cor == cor)
        {
          aux.Add(x);
        }
      }
      aux.ExceptWith(pecasCapturadas(cor));
      return aux;
    }
    public void colocarNovaPeca(char coluna, int linha, Peca peca)
    {
      tab.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
      pecas.Add(peca);
    }
    public bool testeXequemate(Cor cor)
    {
      if(!estaEmCheck(cor))
      {
          return false;
      }
      foreach (Peca x in pecasEmJogo(cor))
      {
        bool[,] mat = x.movimentosPossiveis();
        for (int i = 0; i < tab.linhas; i++)
        {
          for (int j = 0; j < tab.colunas; j++)
          {
            if(mat[i, j])
            {
              Posicao origem = x.posicao;
              Posicao destino = new Posicao(i, j);
              Peca pecaCapturada = executarMovimento(origem, destino);
              bool testeXeque = estaEmCheck(cor);
              desfazMovimento(origem, destino, pecaCapturada);

              if(!testeXeque)
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }
    public void colocarPecas()
    {
      colocarNovaPeca('a', 1, new Torre(tab, Cor.Branca));
      colocarNovaPeca('b', 1, new Cavalo(tab, Cor.Branca));
      colocarNovaPeca('c', 1, new Bispo(tab, Cor.Branca));
      colocarNovaPeca('d', 1, new Rei(tab, Cor.Branca));
      colocarNovaPeca('e', 1, new Dama(tab, Cor.Branca));
      colocarNovaPeca('f', 1, new Bispo(tab, Cor.Branca));
      colocarNovaPeca('g', 1, new Cavalo(tab, Cor.Branca));
      colocarNovaPeca('h', 1, new Torre(tab, Cor.Branca));
      
      colocarNovaPeca('a', 2, new Peao(tab, Cor.Branca));
      colocarNovaPeca('b', 2, new Peao(tab, Cor.Branca));
      colocarNovaPeca('c', 2, new Peao(tab, Cor.Branca));
      colocarNovaPeca('d', 2, new Peao(tab, Cor.Branca));
      colocarNovaPeca('e', 2, new Peao(tab, Cor.Branca));
      colocarNovaPeca('f', 2, new Peao(tab, Cor.Branca));
      colocarNovaPeca('g', 2, new Peao(tab, Cor.Branca));
      colocarNovaPeca('h', 2, new Peao(tab, Cor.Branca));

      colocarNovaPeca('a', 8, new Torre(tab, Cor.Preta));
      colocarNovaPeca('b', 8, new Cavalo(tab, Cor.Preta));
      colocarNovaPeca('c', 8, new Bispo(tab, Cor.Preta));
      colocarNovaPeca('d', 8, new Rei(tab, Cor.Preta));
      colocarNovaPeca('e', 8, new Dama(tab, Cor.Preta));
      colocarNovaPeca('f', 8, new Bispo(tab, Cor.Preta));
      colocarNovaPeca('g', 8, new Cavalo(tab, Cor.Preta));
      colocarNovaPeca('h', 8, new Torre(tab, Cor.Preta));
      
      colocarNovaPeca('a', 7, new Peao(tab, Cor.Preta));
      colocarNovaPeca('b', 7, new Peao(tab, Cor.Preta));
      colocarNovaPeca('c', 7, new Peao(tab, Cor.Preta));
      colocarNovaPeca('d', 7, new Peao(tab, Cor.Preta));
      colocarNovaPeca('e', 7, new Peao(tab, Cor.Preta));
      colocarNovaPeca('f', 7, new Peao(tab, Cor.Preta));
      colocarNovaPeca('g', 7, new Peao(tab, Cor.Preta));
      colocarNovaPeca('h', 7, new Peao(tab, Cor.Preta));      


    }
  }
}