using System;
using System.Linq;

namespace HeuristicasDistribuicaoTarefas
{
    /// <summary>
    /// Representa uma solução para o problema de distribuição de tarefas entre máquinas
    /// </summary>
    public class Solucao
    {
        public int NumTarefas { get; private set; }
        public int NumMaquinas { get; private set; }
        public int[] TemposTarefas { get; private set; }
        public int[] AlocacaoTarefas { get; private set; } // AlocacaoTarefas[i] = máquina da tarefa i
        public double Makespan { get; private set; }

        public Solucao(int numTarefas, int numMaquinas, int[] temposTarefas)
        {
            NumTarefas = numTarefas;
            NumMaquinas = numMaquinas;
            TemposTarefas = temposTarefas;
            AlocacaoTarefas = new int[numTarefas];
        }

        /// <summary>
        /// Construtor de cópia
        /// </summary>
        public Solucao(Solucao outra)
        {
            NumTarefas = outra.NumTarefas;
            NumMaquinas = outra.NumMaquinas;
            TemposTarefas = outra.TemposTarefas;
            AlocacaoTarefas = (int[])outra.AlocacaoTarefas.Clone();
            Makespan = outra.Makespan;
        }

        /// <summary>
        /// Gera uma solução inicial aleatória
        /// </summary>
        public void GerarSolucaoInicialAleatoria(Random random)
        {
            for (int i = 0; i < NumTarefas; i++)
            {
                AlocacaoTarefas[i] = random.Next(NumMaquinas);
            }
            CalcularMakespan();
        }

        /// <summary>
        /// Gera uma solução inicial usando heurística gulosa (menor carga)
        /// </summary>
        public void GerarSolucaoInicialGulosa()
        {
            double[] cargaMaquinas = new double[NumMaquinas];

            for (int i = 0; i < NumTarefas; i++)
            {
                // Encontra a máquina com menor carga
                int maquinaMenorCarga = 0;
                for (int m = 1; m < NumMaquinas; m++)
                {
                    if (cargaMaquinas[m] < cargaMaquinas[maquinaMenorCarga])
                    {
                        maquinaMenorCarga = m;
                    }
                }

                AlocacaoTarefas[i] = maquinaMenorCarga;
                cargaMaquinas[maquinaMenorCarga] += TemposTarefas[i];
            }

            CalcularMakespan();
        }

        /// <summary>
        /// Calcula o makespan (tempo máximo entre todas as máquinas)
        /// </summary>
        public void CalcularMakespan()
        {
            double[] cargaMaquinas = new double[NumMaquinas];

            for (int i = 0; i < NumTarefas; i++)
            {
                cargaMaquinas[AlocacaoTarefas[i]] += TemposTarefas[i];
            }

            Makespan = cargaMaquinas.Max();
        }

        /// <summary>
        /// Obtém a carga de cada máquina
        /// </summary>
        public double[] ObterCargaMaquinas()
        {
            double[] cargaMaquinas = new double[NumMaquinas];

            for (int i = 0; i < NumTarefas; i++)
            {
                cargaMaquinas[AlocacaoTarefas[i]] += TemposTarefas[i];
            }

            return cargaMaquinas;
        }

        /// <summary>
        /// Move uma tarefa para outra máquina
        /// </summary>
        public void MoverTarefa(int indiceTarefa, int novaMaquina)
        {
            AlocacaoTarefas[indiceTarefa] = novaMaquina;
        }

        /// <summary>
        /// Aplica uma perturbação à solução
        /// </summary>
        public void Perturbar(double intensidade, Random random)
        {
            int numTarefasPerturbar = (int)(NumTarefas * intensidade);

            for (int i = 0; i < numTarefasPerturbar; i++)
            {
                int tarefaAleatoria = random.Next(NumTarefas);
                int maquinaAleatoria = random.Next(NumMaquinas);
                AlocacaoTarefas[tarefaAleatoria] = maquinaAleatoria;
            }

            CalcularMakespan();
        }

        public override string ToString()
        {
            return $"Makespan: {Makespan:F2}";
        }
    }
}
