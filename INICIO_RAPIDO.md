# 🚀 Guia de Início Rápido

## Pré-requisitos

- **.NET 6.0 SDK** ou superior
  - Download: https://dotnet.microsoft.com/download
  - Verificar instalação: `dotnet --version`

## Execução Rápida

### Windows
1. Clique duas vezes em `executar.bat`
2. Aguarde a compilação
3. Pressione qualquer tecla para iniciar
4. Aguarde conclusão (5-15 minutos)

### Linux/Mac
```bash
chmod +x executar.sh
./executar.sh
```

### Manual (Qualquer SO)
```bash
# Compilar
dotnet build -c Release

# Executar
dotnet run -c Release
```

## Saídas Esperadas

### 1. Arquivo CSV de Resultados
- **Nome**: `resultados_YYYYMMDD_HHmmss.csv`
- **Linhas**: ~5.400 experimentos
- **Formato**: heuristica,n,m,replicacao,tempo,iteracoes,valor,parametro

### 2. Arquivo de Análise Textual
- **Nome**: `analise_YYYYMMDD_HHmmss.txt`
- **Conteúdo**: Estatísticas e comparações

## Análise Gráfica (Opcional)

### Instalar dependências Python
```bash
pip install pandas matplotlib seaborn
```

### Gerar gráficos
```bash
python analisar_resultados.py resultados_YYYYMMDD_HHmmss.csv
```

Isso criará uma pasta `graficos/` com 6 visualizações:
1. Comparação de Makespan
2. Tempo de Execução
3. Número de Iterações
4. Impacto dos Parâmetros
5. Trade-off Qualidade vs Tempo
6. Desempenho por Tamanho

## Estrutura de Arquivos

```
HeuristicasDistribuicaoTarefas/
│
├── *.cs                          # Código-fonte C#
├── *.csproj                      # Arquivo de projeto
├── executar.bat                  # Script Windows
├── executar.sh                   # Script Linux/Mac
├── analisar_resultados.py        # Script de análise Python
├── README.md                     # Documentação completa
└── INICIO_RAPIDO.md             # Este arquivo
│
└── (após execução)
    ├── resultados_*.csv          # Resultados experimentais
    ├── analise_*.txt             # Análise textual
    └── graficos/                 # Gráficos (se usar Python)
        ├── 01_comparacao_makespan.png
        ├── 02_tempo_execucao.png
        ├── 03_numero_iteracoes.png
        ├── 04_impacto_parametros.png
        ├── 05_tradeoff_qualidade_tempo.png
        └── 06_desempenho_tamanho.png
```

## Perguntas Frequentes

### Q: Quanto tempo leva a execução?
**A:** Entre 5 e 15 minutos, dependendo do hardware.

### Q: Posso interromper a execução?
**A:** Sim, mas os resultados parciais não serão salvos. Use Ctrl+C.

### Q: Como interpretar os resultados?
**A:** 
- **Makespan menor** = melhor qualidade
- **Tempo menor** = mais rápido
- Veja o arquivo `analise_*.txt` para resumo automático

### Q: Como modificar os experimentos?
**A:** Edite o arquivo `Program.cs`:
- Linha ~14-15: Configurações de máquinas e tarefas
- Linha ~16: Número de replicações
- Linha ~19-22: Parâmetros das heurísticas

### Q: Preciso do Python?
**A:** Não! O programa C# já gera análises completas em texto. Python é opcional para gráficos.

## Troubleshooting

### "dotnet: comando não encontrado"
→ Instale o .NET SDK: https://dotnet.microsoft.com/download

### "Erro de compilação"
→ Verifique se todos os arquivos .cs estão na mesma pasta

### "Demora muito tempo"
→ Normal! São 5.400 experimentos. Para testar rápido, reduza:
- Número de replicações (linha 16 do Program.cs)
- Número de parâmetros (linhas 19-22)

### Python: "ModuleNotFoundError"
→ Instale: `pip install pandas matplotlib seaborn`

## Suporte

Para dúvidas sobre o código ou implementação:
1. Consulte o README.md completo
2. Verifique os comentários no código-fonte
3. Analise os exemplos no README.md

---

**Desenvolvido para UNIVALI - Complexidade de Algoritmos**
