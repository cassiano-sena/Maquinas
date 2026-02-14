using System;
using System.Diagnostics;

namespace HeuristicasDistribuicaoTarefas
{
    /// <summary>
    /// Classe base para todas as heurísticas
    /// </summary>
    public abstract class HeuristicaBase
    {
        protected Random random;
        protected int iteracoesSemMelhora;
        protected int totalIteracoes;
        protected double tempoExecucao;
        protected const int MAX_ITERACOES_SEM_MELHORA = 1000;

        public string Nome { get; protected set; }

        public HeuristicaBase(int seed = 0)
        {
            random = seed == 0 ? new Random() : new Random(seed);
        }

        /// <summary>
        /// Executa a heurística
        /// </summary>
        public virtual ResultadoExecucao Executar(Solucao solucaoInicial, double parametro)
        {
            Stopwatch sw = Stopwatch.StartNew();
            iteracoesSemMelhora = 0;
            totalIteracoes = 0;

            Solucao melhorSolucao = ExecutarBusca(solucaoInicial, parametro);

            sw.Stop();
            tempoExecucao = sw.Elapsed.TotalSeconds;

            return new ResultadoExecucao
            {
                NomeHeuristica = Nome,
                Makespan = melhorSolucao.Makespan,
                TempoExecucao = tempoExecucao,
                TotalIteracoes = totalIteracoes,
                Parametro = parametro
            };
        }

        /// <summary>
        /// Implementação específica de cada heurística
        /// </summary>
        protected abstract Solucao ExecutarBusca(Solucao solucaoInicial, double parametro);

        /// <summary>
        /// Busca local gulosa - move tarefas para melhorar o makespan
        /// </summary>
        protected Solucao BuscaLocal(Solucao solucao)
        {
            bool melhorou = true;

            while (melhorou)
            {
                melhorou = false;
                Solucao melhorVizinho = new Solucao(solucao);

                // Tenta mover cada tarefa para cada máquina
                for (int tarefa = 0; tarefa < solucao.NumTarefas; tarefa++)
                {
                    int maquinaOriginal = solucao.AlocacaoTarefas[tarefa];

                    for (int novaMaquina = 0; novaMaquina < solucao.NumMaquinas; novaMaquina++)
                    {
                        if (novaMaquina == maquinaOriginal) continue;

                        // Cria vizinho
                        Solucao vizinho = new Solucao(solucao);
                        vizinho.MoverTarefa(tarefa, novaMaquina);
                        vizinho.CalcularMakespan();

                        // Se melhorou, atualiza
                        if (vizinho.Makespan < melhorVizinho.Makespan)
                        {
                            melhorVizinho = vizinho;
                            melhorou = true;
                        }
                    }
                }

                if (melhorou)
                {
                    solucao = melhorVizinho;
                }
            }

            return solucao;
        }

        /// <summary>
        /// Obtém o melhor vizinho (first improvement)
        /// </summary>
        protected Solucao ObterMelhorVizinho(Solucao solucao)
        {
            Solucao melhorVizinho = new Solucao(solucao);

            // Tenta mover cada tarefa para cada máquina
            for (int tarefa = 0; tarefa < solucao.NumTarefas; tarefa++)
            {
                int maquinaOriginal = solucao.AlocacaoTarefas[tarefa];

                for (int novaMaquina = 0; novaMaquina < solucao.NumMaquinas; novaMaquina++)
                {
                    if (novaMaquina == maquinaOriginal) continue;

                    // Cria vizinho
                    Solucao vizinho = new Solucao(solucao);
                    vizinho.MoverTarefa(tarefa, novaMaquina);
                    vizinho.CalcularMakespan();

                    // Se melhorou, atualiza
                    if (vizinho.Makespan < melhorVizinho.Makespan)
                    {
                        melhorVizinho = vizinho;
                    }
                }
            }

            return melhorVizinho;
        }

        /// <summary>
        /// Reinicia o gerador aleatório
        /// </summary>
        public void ReiniciarRandom(int seed)
        {
            random = new Random(seed);
        }
    }

    /// <summary>
    /// Armazena o resultado de uma execução
    /// </summary>
    public class ResultadoExecucao
    {
        public string NomeHeuristica { get; set; }
        public double Makespan { get; set; }
        public double TempoExecucao { get; set; }
        public int TotalIteracoes { get; set; }
        public double Parametro { get; set; }
        public int NumTarefas { get; set; }
        public int NumMaquinas { get; set; }
        public int Replicacao { get; set; }

        public string ToCSVLine()
        {
            string parametroStr = double.IsNaN(Parametro) ? "NA" : Parametro.ToString("F2");
            return $"{NomeHeuristica},{NumTarefas},{NumMaquinas},{Replicacao}," +
                   $"{TempoExecucao:F2},{TotalIteracoes},{Makespan:F2},{parametroStr}";
        }
    }
}
