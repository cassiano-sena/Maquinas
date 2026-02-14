using System;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicasDistribuicaoTarefas
{
    /// <summary>
    /// Busca Tabu
    /// Mantém uma lista de movimentos proibidos para evitar ciclos
    /// </summary>
    public class BuscaTabu : HeuristicaBase
    {
        private Queue<Movimento> listaTabu;
        private int tamanhoListaTabu;

        public BuscaTabu(int seed = 0) : base(seed)
        {
            Nome = "BuscaTabu";
            listaTabu = new Queue<Movimento>();
        }

        protected override Solucao ExecutarBusca(Solucao solucaoInicial, double alfa)
        {
            Solucao solucaoAtual = new Solucao(solucaoInicial);
            Solucao melhorSolucao = new Solucao(solucaoInicial);
            
            iteracoesSemMelhora = 0;
            totalIteracoes = 0;
            listaTabu.Clear();

            while (iteracoesSemMelhora < MAX_ITERACOES_SEM_MELHORA)
            {
                totalIteracoes++;

                // Define tamanho da lista tabu
                // Se alfa < 0, escolhe aleatoriamente
                if (alfa < 0)
                {
                    tamanhoListaTabu = (int)(random.NextDouble() * 0.09 * solucaoAtual.NumTarefas);
                }
                else
                {
                    tamanhoListaTabu = (int)(alfa * solucaoAtual.NumTarefas);
                }

                // Encontra o melhor vizinho não-tabu
                (Solucao melhorVizinho, Movimento movimento) = EncontrarMelhorVizinhoNaoTabu(solucaoAtual, melhorSolucao);

                // Aceita o movimento
                solucaoAtual = melhorVizinho;

                // Adiciona movimento à lista tabu
                listaTabu.Enqueue(movimento);
                if (listaTabu.Count > tamanhoListaTabu)
                {
                    listaTabu.Dequeue();
                }

                // Verifica se é a melhor global
                if (solucaoAtual.Makespan < melhorSolucao.Makespan)
                {
                    melhorSolucao = new Solucao(solucaoAtual);
                    iteracoesSemMelhora = 0;
                }
                else
                {
                    iteracoesSemMelhora++;
                }
            }

            return melhorSolucao;
        }

        private (Solucao, Movimento) EncontrarMelhorVizinhoNaoTabu(Solucao solucao, Solucao melhorSolucaoGlobal)
        {
            Solucao melhorVizinho = null;
            Movimento melhorMovimento = null;
            double melhorMakespan = double.MaxValue;

            // Tenta mover cada tarefa para cada máquina
            for (int tarefa = 0; tarefa < solucao.NumTarefas; tarefa++)
            {
                int maquinaOriginal = solucao.AlocacaoTarefas[tarefa];

                for (int novaMaquina = 0; novaMaquina < solucao.NumMaquinas; novaMaquina++)
                {
                    if (novaMaquina == maquinaOriginal) continue;

                    Movimento movimento = new Movimento(tarefa, maquinaOriginal, novaMaquina);

                    // Verifica se o movimento é tabu
                    bool ehTabu = listaTabu.Contains(movimento);

                    // Cria vizinho
                    Solucao vizinho = new Solucao(solucao);
                    vizinho.MoverTarefa(tarefa, novaMaquina);
                    vizinho.CalcularMakespan();

                    // Critério de aspiração: aceita movimento tabu se for melhor que a melhor global
                    bool aceitarPorAspiracao = ehTabu && vizinho.Makespan < melhorSolucaoGlobal.Makespan;

                    // Se não é tabu ou satisfaz critério de aspiração
                    if (!ehTabu || aceitarPorAspiracao)
                    {
                        if (vizinho.Makespan < melhorMakespan)
                        {
                            melhorVizinho = vizinho;
                            melhorMovimento = movimento;
                            melhorMakespan = vizinho.Makespan;
                        }
                    }
                }
            }

            // Se não encontrou nenhum vizinho não-tabu, aceita o melhor tabu
            if (melhorVizinho == null)
            {
                for (int tarefa = 0; tarefa < solucao.NumTarefas; tarefa++)
                {
                    int maquinaOriginal = solucao.AlocacaoTarefas[tarefa];

                    for (int novaMaquina = 0; novaMaquina < solucao.NumMaquinas; novaMaquina++)
                    {
                        if (novaMaquina == maquinaOriginal) continue;

                        Solucao vizinho = new Solucao(solucao);
                        vizinho.MoverTarefa(tarefa, novaMaquina);
                        vizinho.CalcularMakespan();

                        if (vizinho.Makespan < melhorMakespan)
                        {
                            melhorVizinho = vizinho;
                            melhorMovimento = new Movimento(tarefa, maquinaOriginal, novaMaquina);
                            melhorMakespan = vizinho.Makespan;
                        }
                    }
                }
            }

            return (melhorVizinho, melhorMovimento);
        }
    }

    /// <summary>
    /// Representa um movimento (tarefa, máquina origem, máquina destino)
    /// </summary>
    public class Movimento : IEquatable<Movimento>
    {
        public int Tarefa { get; set; }
        public int MaquinaOrigem { get; set; }
        public int MaquinaDestino { get; set; }

        public Movimento(int tarefa, int maquinaOrigem, int maquinaDestino)
        {
            Tarefa = tarefa;
            MaquinaOrigem = maquinaOrigem;
            MaquinaDestino = maquinaDestino;
        }

        public bool Equals(Movimento other)
        {
            if (other == null) return false;
            return Tarefa == other.Tarefa && 
                   MaquinaOrigem == other.MaquinaOrigem && 
                   MaquinaDestino == other.MaquinaDestino;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Movimento);
        }

        public override int GetHashCode()
        {
            return Tarefa.GetHashCode() ^ MaquinaOrigem.GetHashCode() ^ MaquinaDestino.GetHashCode();
        }
    }
}
