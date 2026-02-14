using System;

namespace HeuristicasDistribuicaoTarefas
{
    /// <summary>
    /// Têmpera Simulada (Simulated Annealing)
    /// Aceita soluções piores com probabilidade decrescente
    /// </summary>
    public class TemperaSimulada : HeuristicaBase
    {
        private const double TEMPERATURA_INICIAL = 1000.0;
        private const double TEMPERATURA_MINIMA = 0.01;

        public TemperaSimulada(int seed = 0) : base(seed)
        {
            Nome = "TemperaSimulada";
        }

        protected override Solucao ExecutarBusca(Solucao solucaoInicial, double fatorResfriamento)
        {
            Solucao solucaoAtual = new Solucao(solucaoInicial);
            Solucao melhorSolucao = new Solucao(solucaoInicial);
            
            double temperatura = TEMPERATURA_INICIAL;
            iteracoesSemMelhora = 0;
            totalIteracoes = 0;

            while (iteracoesSemMelhora < MAX_ITERACOES_SEM_MELHORA && temperatura > TEMPERATURA_MINIMA)
            {
                totalIteracoes++;

                // Gera vizinho aleatório
                Solucao vizinho = GerarVizinhoAleatorio(solucaoAtual);

                // Calcula diferença de energia (makespan)
                double delta = vizinho.Makespan - solucaoAtual.Makespan;

                // Aceita se melhorou ou com probabilidade baseada na temperatura
                if (delta < 0 || random.NextDouble() < Math.Exp(-delta / temperatura))
                {
                    solucaoAtual = vizinho;

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
                else
                {
                    iteracoesSemMelhora++;
                }

                // Resfria a temperatura
                temperatura *= fatorResfriamento;
            }

            return melhorSolucao;
        }

        /// <summary>
        /// Gera um vizinho aleatório movendo uma tarefa aleatória para uma máquina aleatória
        /// </summary>
        private Solucao GerarVizinhoAleatorio(Solucao solucao)
        {
            Solucao vizinho = new Solucao(solucao);

            int tarefaAleatoria = random.Next(solucao.NumTarefas);
            int maquinaAleatoria = random.Next(solucao.NumMaquinas);

            // Garante que a máquina é diferente da atual
            while (maquinaAleatoria == solucao.AlocacaoTarefas[tarefaAleatoria])
            {
                maquinaAleatoria = random.Next(solucao.NumMaquinas);
            }

            vizinho.MoverTarefa(tarefaAleatoria, maquinaAleatoria);
            vizinho.CalcularMakespan();

            return vizinho;
        }
    }
}
