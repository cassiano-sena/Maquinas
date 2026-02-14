using System;

namespace HeuristicasDistribuicaoTarefas
{
    /// <summary>
    /// Gera instâncias do problema de distribuição de tarefas
    /// </summary>
    public class GeradorInstancias
    {
        private Random random;

        public GeradorInstancias(int seed = 0)
        {
            random = seed == 0 ? new Random() : new Random(seed);
        }

        /// <summary>
        /// Gera uma instância do problema
        /// </summary>
        /// <param name="numMaquinas">Número de máquinas (m)</param>
        /// <param name="fatorTarefas">Fator multiplicador para número de tarefas (r)</param>
        /// <returns>Array com os tempos de cada tarefa</returns>
        public int[] GerarInstancia(int numMaquinas, double fatorTarefas, out int numTarefas)
        {
            numTarefas = (int)(numMaquinas * fatorTarefas);
            int[] temposTarefas = new int[numTarefas];

            for (int i = 0; i < numTarefas; i++)
            {
                temposTarefas[i] = random.Next(1, 101); // Tempo entre 1 e 100
            }

            return temposTarefas;
        }

        /// <summary>
        /// Reinicia o gerador com uma nova seed
        /// </summary>
        public void ReiniciarComSeed(int seed)
        {
            random = new Random(seed);
        }
    }
}
