# Avaliação de Heurísticas para Distribuição de Tarefas

Implementação em C# de heurísticas de busca local para o problema de distribuição de tarefas entre máquinas paralelas, minimizando o makespan (tempo máximo de execução).

## 📋 Descrição do Problema

Dado:
- **m** máquinas paralelas
- **n** tarefas, onde n = m × r
- **pᵢ** tempo de processamento da tarefa i (entre 1 e 100)

Objetivo: Minimizar o makespan (tempo de uso máximo entre todas as máquinas)

## 🔧 Heurísticas Implementadas

### 1. Busca Local Monótona Randomizada ✅
- **Tipo**: Monótona (aceita apenas melhorias)
- **Parâmetro α**: Probabilidade de fazer movimento aleatório (0.1 a 0.9)
- **Característica**: Equilibra exploração (movimentos aleatórios) e exploitação (busca local)

### 2. Busca Local Iterada ❌
- **Tipo**: Não-Monótona (aceita pioras através de perturbação)
- **Parâmetro per**: Intensidade da perturbação (0.1 a 0.9)
- **Característica**: Aplica busca local, perturba a solução e repete

### 3. Busca Tabu ❌
- **Tipo**: Não-Monótona (mantém memória de movimentos proibidos)
- **Parâmetro α**: Define tamanho da lista tabu (d = α × n)
- **Característica**: Evita ciclos usando lista de movimentos proibidos

### 4. Têmpera Simulada ❌
- **Tipo**: Não-Monótona (aceita pioras com probabilidade decrescente)
- **Parâmetro α**: Fator de resfriamento (0.8 a 0.99)
- **Característica**: Inspirada no processo de recozimento de metais

## 📊 Configurações do Experimento

### Instâncias
- **Máquinas (m)**: 10, 20, 50
- **Fator de tarefas (r)**: 1.5, 2.0
- **Número de tarefas (n)**: m × r
- **Tempo das tarefas**: Aleatório entre 1 e 100
- **Replicações**: 10 execuções por configuração

### Parâmetros Testados

| Heurística | Parâmetros | Critério de Parada |
|------------|------------|-------------------|
| Busca Monótona Randomizada | α ∈ {0.1, 0.2, ..., 0.9} | 1000 iterações sem melhora |
| Busca Local Iterada | per ∈ {0.1, 0.2, ..., 0.9} | 1000 iterações sem melhora |
| Busca Tabu | α ∈ {R, 0.01, 0.02, ..., 0.09} | 1000 iterações sem melhora |
| Têmpera Simulada | α ∈ {0.8, 0.85, 0.9, 0.95, 0.99} | 1000 iterações sem melhora |

*Nota: R = escolha aleatória do parâmetro α*

## 🚀 Como Executar

### Requisitos
- .NET 6.0 SDK ou superior

### Compilação e Execução

```bash
# Compilar o projeto
dotnet build

# Executar
dotnet run

# Ou compilar e executar em Release para melhor performance
dotnet run -c Release
```

### Execução Direta (Windows)
```bash
csc /out:Heuristicas.exe *.cs
Heuristicas.exe
```

## 📁 Estrutura do Projeto

```
HeuristicasDistribuicaoTarefas/
├── Program.cs                          # Programa principal
├── Solucao.cs                          # Representação de uma solução
├── GeradorInstancias.cs                # Gerador de instâncias aleatórias
├── HeuristicaBase.cs                   # Classe base para heurísticas
├── BuscaLocalMonotonaRandomizada.cs    # Implementação Busca Monótona
├── BuscaLocalIterada.cs                # Implementação Busca Iterada
├── BuscaTabu.cs                        # Implementação Busca Tabu
├── TemperaSimulada.cs                  # Implementação Têmpera Simulada
└── HeuristicasDistribuicaoTarefas.csproj
```

## 📈 Saídas do Programa

### 1. Arquivo CSV de Resultados
**Nome**: `resultados_YYYYMMDD_HHmmss.csv`

**Formato**:
```csv
heuristica,n,m,replicacao,tempo,iteracoes,valor,parametro
BuscaMonotonaRandomizada,15,10,1,0.23,1029,88.50,0.10
BuscaLocalIterada,15,10,1,1.45,1500,85.30,0.20
BuscaTabu,15,10,1,2.10,2100,82.70,0.05
TemperaSimulada,15,10,1,3.50,5000,87.20,0.95
```

**Colunas**:
- `heuristica`: Nome da heurística
- `n`: Número de tarefas
- `m`: Número de máquinas
- `replicacao`: Número da replicação (1-10)
- `tempo`: Tempo de execução em segundos
- `iteracoes`: Total de iterações executadas
- `valor`: Makespan obtido (quanto menor, melhor)
- `parametro`: Valor do parâmetro usado

### 2. Arquivo de Análise
**Nome**: `analise_YYYYMMDD_HHmmss.txt`

**Conteúdo**:
1. Média de iterações por heurística
2. Média de tempo por heurística
3. Qualidade das soluções (makespan médio e melhor)
4. Melhores parâmetros por heurística
5. Resumo geral

## 🎯 Perguntas Respondidas

O programa gera análises automáticas para responder:

1. **Qual heurística demandou mais iterações?**
2. **Qual heurística demandou mais tempo?**
3. **Qual heurística encontrou resultados com maior qualidade?**
4. **Quais parâmetros garantem maior qualidade?**
5. **Quais parâmetros são mais rápidos?**

## 🔍 Detalhes de Implementação

### Solução Inicial
- Utiliza **heurística gulosa**: aloca cada tarefa à máquina com menor carga atual
- Garante ponto de partida consistente para todas as heurísticas

### Busca Local
- **Estratégia**: First Improvement
- **Vizinhança**: Move cada tarefa para cada máquina diferente
- Complexidade: O(n × m) por iteração

### Critérios de Aceitação
- **Monótona**: Aceita apenas se melhorou
- **Não-Monótona**: Pode aceitar soluções piores (depende da heurística)

### Controle de Aleatoriedade
- Seeds diferentes para cada replicação
- Garante reprodutibilidade dos experimentos

## 📊 Exemplo de Uso

```csharp
// Criar uma instância
GeradorInstancias gerador = new GeradorInstancias(42);
int[] tempos = gerador.GerarInstancia(10, 1.5, out int numTarefas);

// Criar solução inicial
Solucao solucao = new Solucao(numTarefas, 10, tempos);
solucao.GerarSolucaoInicialGulosa();

// Executar Busca Tabu com α = 0.05
BuscaTabu heuristica = new BuscaTabu(42);
ResultadoExecucao resultado = heuristica.Executar(solucao, 0.05);

Console.WriteLine($"Makespan: {resultado.Makespan}");
Console.WriteLine($"Tempo: {resultado.TempoExecucao}s");
Console.WriteLine($"Iterações: {resultado.TotalIteracoes}");
```

## 📝 Observações

- O programa executa **5.400 experimentos** no total
- Tempo estimado de execução: 5-15 minutos (depende do hardware)
- Todos os resultados são salvos automaticamente
- A análise estatística é gerada ao final

---

**Desenvolvido para avaliação de heurísticas de otimização combinatória**
