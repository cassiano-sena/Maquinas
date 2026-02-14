using System;

namespace HeuristicasDistribuicaoTarefas
{
    /// <summary>
    /// Busca Local Iterada (Iterated Local Search)
    /// Aplica busca local, perturba a solução e repete
    /// </summary>
    public class BuscaLocalIterada : HeuristicaBase
    {
        public BuscaLocalIterada(int seed = 0) : base(seed)
        {
            Nome = "BuscaLocalIterada";
        }

        protected override Solucao ExecutarBusca(Solucao solucaoInicial, double intensidadePerturbacao)
        {
            // Aplica busca local na solução inicial
            Solucao solucaoAtual = BuscaLocal(new Solucao(solucaoInicial));
            Solucao melhorSolucao = new Solucao(solucaoAtual);
            
            iteracoesSemMelhora = 0;
            totalIteracoes = 0;

            while (iteracoesSemMelhora < MAX_ITERACOES_SEM_MELHORA)
            {
                totalIteracoes++;

                // Perturba a solução atual
                Solucao solucaoPerturbada = new Solucao(solucaoAtual);
                solucaoPerturbada.Perturbar(intensidadePerturbacao, random);

                // Aplica busca local na solução perturbada
                Solucao solucaoRefinada = BuscaLocal(solucaoPerturbada);

                // Critério de aceitação: aceita se melhorou
                if (solucaoRefinada.Makespan < solucaoAtual.Makespan)
                {
                    solucaoAtual = solucaoRefinada;

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

            return melhorSolucao;
        }
    }
}
