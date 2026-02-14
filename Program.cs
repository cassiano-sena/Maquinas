using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HeuristicasDistribuicaoTarefas
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("  AVALIAÇÃO DE HEURÍSTICAS PARA DISTRIBUIÇÃO");
            Console.WriteLine("  DE TAREFAS ENTRE MÁQUINAS PARALELAS");
            Console.WriteLine("=================================================\n");

            // Configurações do experimento
            int[] numeroMaquinas = { 10, 20, 50 };
            double[] fatoresTarefas = { 1.5, 2.0 };
            int numReplicacoes = 10;

            // Parâmetros das heurísticas
            double[] parametrosBuscaIterada = { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };
            double[] parametrosBuscaMonotona = { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };
            double[] parametrosBuscaTabu = { -1.0, 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09 }; // -1 = Random
            double[] parametrosTemperaSimulada = { 0.8, 0.85, 0.9, 0.95, 0.99 };

            List<ResultadoExecucao> todosResultados = new List<ResultadoExecucao>();

            int totalExperimentos = numeroMaquinas.Length * fatoresTarefas.Length * numReplicacoes *
                                   (parametrosBuscaIterada.Length + parametrosBuscaMonotona.Length +
                                    parametrosBuscaTabu.Length + parametrosTemperaSimulada.Length);
            int experimentoAtual = 0;

            Console.WriteLine($"Total de experimentos a executar: {totalExperimentos}\n");

            // Loop principal dos experimentos
            foreach (int m in numeroMaquinas)
            {
                foreach (double r in fatoresTarefas)
                {
                    int n = (int)(m * r);
                    Console.WriteLine($"\n>>> Configuração: m={m} máquinas, r={r}, n={n} tarefas");

                    for (int replicacao = 1; replicacao <= numReplicacoes; replicacao++)
                    {
                        // Gera instância com seed baseada na replicação
                        GeradorInstancias gerador = new GeradorInstancias(replicacao * 1000 + m * 10 + (int)(r * 10));
                        int numTarefas;
                        int[] temposTarefas = gerador.GerarInstancia(m, r, out numTarefas);

                        Console.WriteLine($"\n  Replicação {replicacao}/{numReplicacoes}");

                        // ===== BUSCA LOCAL MONÓTONA RANDOMIZADA =====
                        foreach (double alfa in parametrosBuscaMonotona)
                        {
                            experimentoAtual++;
                            MostrarProgresso(experimentoAtual, totalExperimentos, "Busca Monótona Randomizada");

                            BuscaLocalMonotonaRandomizada heuristica = new BuscaLocalMonotonaRandomizada(replicacao);
                            Solucao solucaoInicial = new Solucao(numTarefas, m, temposTarefas);
                            solucaoInicial.GerarSolucaoInicialGulosa();

                            ResultadoExecucao resultado = heuristica.Executar(solucaoInicial, alfa);
                            resultado.NumMaquinas = m;
                            resultado.NumTarefas = numTarefas;
                            resultado.Replicacao = replicacao;
                            todosResultados.Add(resultado);
                        }

                        // ===== BUSCA LOCAL ITERADA =====
                        foreach (double per in parametrosBuscaIterada)
                        {
                            experimentoAtual++;
                            MostrarProgresso(experimentoAtual, totalExperimentos, "Busca Local Iterada");

                            BuscaLocalIterada heuristica = new BuscaLocalIterada(replicacao);
                            Solucao solucaoInicial = new Solucao(numTarefas, m, temposTarefas);
                            solucaoInicial.GerarSolucaoInicialGulosa();

                            ResultadoExecucao resultado = heuristica.Executar(solucaoInicial, per);
                            resultado.NumMaquinas = m;
                            resultado.NumTarefas = numTarefas;
                            resultado.Replicacao = replicacao;
                            todosResultados.Add(resultado);
                        }

                        // ===== BUSCA TABU =====
                        foreach (double alfa in parametrosBuscaTabu)
                        {
                            experimentoAtual++;
                            MostrarProgresso(experimentoAtual, totalExperimentos, "Busca Tabu");

                            BuscaTabu heuristica = new BuscaTabu(replicacao);
                            Solucao solucaoInicial = new Solucao(numTarefas, m, temposTarefas);
                            solucaoInicial.GerarSolucaoInicialGulosa();

                            ResultadoExecucao resultado = heuristica.Executar(solucaoInicial, alfa);
                            resultado.NumMaquinas = m;
                            resultado.NumTarefas = numTarefas;
                            resultado.Replicacao = replicacao;
                            todosResultados.Add(resultado);
                        }

                        // ===== TÊMPERA SIMULADA =====
                        foreach (double alfa in parametrosTemperaSimulada)
                        {
                            experimentoAtual++;
                            MostrarProgresso(experimentoAtual, totalExperimentos, "Têmpera Simulada");

                            TemperaSimulada heuristica = new TemperaSimulada(replicacao);
                            Solucao solucaoInicial = new Solucao(numTarefas, m, temposTarefas);
                            solucaoInicial.GerarSolucaoInicialGulosa();

                            ResultadoExecucao resultado = heuristica.Executar(solucaoInicial, alfa);
                            resultado.NumMaquinas = m;
                            resultado.NumTarefas = numTarefas;
                            resultado.Replicacao = replicacao;
                            todosResultados.Add(resultado);
                        }
                    }
                }
            }

            // Salva resultados
            Console.WriteLine("\n\n=================================================");
            Console.WriteLine("  SALVANDO RESULTADOS");
            Console.WriteLine("=================================================\n");

            SalvarResultados(todosResultados);
            GerarRelatorioAnalise(todosResultados);

            Console.WriteLine("\nExperimentos concluídos com sucesso!");
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        static void MostrarProgresso(int atual, int total, string heuristica)
        {
            double percentual = (double)atual / total * 100;
            Console.Write($"\r  [{atual}/{total}] {percentual:F1}% - {heuristica}              ");
        }

        static void SalvarResultados(List<ResultadoExecucao> resultados)
        {
            string nomeArquivo = $"resultados_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            
            using (StreamWriter sw = new StreamWriter(nomeArquivo, false, Encoding.UTF8))
            {
                // Cabeçalho
                sw.WriteLine("heuristica,n,m,replicacao,tempo,iteracoes,valor,parametro");

                // Dados
                foreach (var resultado in resultados)
                {
                    sw.WriteLine(resultado.ToCSVLine());
                }
            }

            Console.WriteLine($"Resultados salvos em: {nomeArquivo}");
        }

        static void GerarRelatorioAnalise(List<ResultadoExecucao> resultados)
        {
            string nomeArquivo = $"analise_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            using (StreamWriter sw = new StreamWriter(nomeArquivo, false, Encoding.UTF8))
            {
                sw.WriteLine("==========================================================");
                sw.WriteLine("  RELATÓRIO DE ANÁLISE - HEURÍSTICAS DE OTIMIZAÇÃO");
                sw.WriteLine("==========================================================\n");

                // Agrupa por heurística
                var porHeuristica = resultados.GroupBy(r => r.NomeHeuristica);

                sw.WriteLine("1. MÉDIA DE ITERAÇÕES POR HEURÍSTICA");
                sw.WriteLine("-----------------------------------------------------");
                var iteracoesPorHeuristica = porHeuristica
                    .Select(g => new { Heuristica = g.Key, MediaIteracoes = g.Average(r => r.TotalIteracoes) })
                    .OrderByDescending(x => x.MediaIteracoes);

                foreach (var item in iteracoesPorHeuristica)
                {
                    sw.WriteLine($"{item.Heuristica,-30}: {item.MediaIteracoes,10:F2} iterações");
                }

                sw.WriteLine("\n2. MÉDIA DE TEMPO POR HEURÍSTICA");
                sw.WriteLine("-----------------------------------------------------");
                var tempoPorHeuristica = porHeuristica
                    .Select(g => new { Heuristica = g.Key, MediaTempo = g.Average(r => r.TempoExecucao) })
                    .OrderByDescending(x => x.MediaTempo);

                foreach (var item in tempoPorHeuristica)
                {
                    sw.WriteLine($"{item.Heuristica,-30}: {item.MediaTempo,10:F4} segundos");
                }

                sw.WriteLine("\n3. QUALIDADE DAS SOLUÇÕES (Menor Makespan = Melhor)");
                sw.WriteLine("-----------------------------------------------------");
                var qualidadePorHeuristica = porHeuristica
                    .Select(g => new { 
                        Heuristica = g.Key, 
                        MediaMakespan = g.Average(r => r.Makespan),
                        MelhorMakespan = g.Min(r => r.Makespan)
                    })
                    .OrderBy(x => x.MediaMakespan);

                foreach (var item in qualidadePorHeuristica)
                {
                    sw.WriteLine($"{item.Heuristica,-30}: Média={item.MediaMakespan,10:F2}  Melhor={item.MelhorMakespan,10:F2}");
                }

                sw.WriteLine("\n4. MELHORES PARÂMETROS POR HEURÍSTICA");
                sw.WriteLine("-----------------------------------------------------");
                foreach (var grupo in porHeuristica)
                {
                    var melhorParametro = grupo
                        .GroupBy(r => r.Parametro)
                        .Select(g => new {
                            Parametro = g.Key,
                            MediaMakespan = g.Average(r => r.Makespan),
                            MediaTempo = g.Average(r => r.TempoExecucao)
                        })
                        .OrderBy(x => x.MediaMakespan)
                        .First();

                    sw.WriteLine($"\n{grupo.Key}:");
                    sw.WriteLine($"  Melhor parâmetro (qualidade): {melhorParametro.Parametro:F2}");
                    sw.WriteLine($"  Makespan médio: {melhorParametro.MediaMakespan:F2}");
                    sw.WriteLine($"  Tempo médio: {melhorParametro.MediaTempo:F4}s");

                    var maisRapido = grupo
                        .GroupBy(r => r.Parametro)
                        .Select(g => new {
                            Parametro = g.Key,
                            MediaTempo = g.Average(r => r.TempoExecucao)
                        })
                        .OrderBy(x => x.MediaTempo)
                        .First();

                    sw.WriteLine($"  Parâmetro mais rápido: {maisRapido.Parametro:F2} ({maisRapido.MediaTempo:F4}s)");
                }

                sw.WriteLine("\n5. RESUMO GERAL");
                sw.WriteLine("-----------------------------------------------------");
                var melhorHeuristica = qualidadePorHeuristica.First();
                var maisRapida = tempoPorHeuristica.OrderBy(x => x.MediaTempo).First();
                var maisIteracoes = iteracoesPorHeuristica.First();

                sw.WriteLine($"Melhor qualidade: {melhorHeuristica.Heuristica} (Makespan médio: {melhorHeuristica.MediaMakespan:F2})");
                sw.WriteLine($"Mais rápida: {maisRapida.Heuristica} ({maisRapida.MediaTempo:F4}s)");
                sw.WriteLine($"Mais iterações: {maisIteracoes.Heuristica} ({maisIteracoes.MediaIteracoes:F0} iterações)");

                sw.WriteLine("\n==========================================================");
            }

            Console.WriteLine($"Análise salva em: {nomeArquivo}");
        }
    }
}
