using System;

namespace HeuristicasDistribuicaoTarefas
{
    /// <summary>
    /// Busca Local Monótona Randomizada
    /// Aceita apenas movimentos que melhoram, mas com probabilidade alfa faz movimento aleatório
    /// </summary>
    public class BuscaLocalMonotonaRandomizada : HeuristicaBase
    {
        public BuscaLocalMonotonaRandomizada(int seed = 0) : base(seed)
        {
            Nome = "BuscaMonotonaRandomizada";
        }

        protected override Solucao ExecutarBusca(Solucao solucaoInicial, double alfa)
        {
            Solucao solucaoAtual = new Solucao(solucaoInicial);
            Solucao melhorSolucao = new Solucao(solucaoInicial);
            
            iteracoesSemMelhora = 0;
            totalIteracoes = 0;

            while (iteracoesSemMelhora < MAX_ITERACOES_SEM_MELHORA)
            {
                totalIteracoes++;

                // Com probabilidade alfa, faz movimento aleatório
                if (random.NextDouble() < alfa)
                {
                    // Caminhada aleatória
                    int tarefaAleatoria = random.Next(solucaoAtual.NumTarefas);
                    int maquinaAleatoria = random.Next(solucaoAtual.NumMaquinas);
                    
                    solucaoAtual.MoverTarefa(tarefaAleatoria, maquinaAleatoria);
                    solucaoAtual.CalcularMakespan();
                }
                else
                {
                    // Busca local - aceita apenas melhorias
                    Solucao melhorVizinho = ObterMelhorVizinho(solucaoAtual);

                    // Se encontrou melhoria, aceita
                    if (melhorVizinho.Makespan < solucaoAtual.Makespan)
                    {
                        solucaoAtual = melhorVizinho;
                        
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
                }
            }

            return melhorSolucao;
        }
    }
}
