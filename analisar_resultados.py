"""
Script para análise e visualização dos resultados das heurísticas
Requer: pandas, matplotlib, seaborn
Instalar: pip install pandas matplotlib seaborn
"""

import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import sys
import os

def analisar_resultados(arquivo_csv):
    """Carrega e analisa os resultados dos experimentos"""
    
    # Carregar dados
    print(f"Carregando dados de {arquivo_csv}...")
    df = pd.read_csv(arquivo_csv)
    
    print(f"\nTotal de experimentos: {len(df)}")
    print(f"Heurísticas: {df['heuristica'].unique()}")
    print(f"Configurações (m): {sorted(df['m'].unique())}")
    
    return df

def criar_visualizacoes(df, pasta_saida='graficos'):
    """Cria visualizações dos resultados"""
    
    # Criar pasta para gráficos
    if not os.path.exists(pasta_saida):
        os.makedirs(pasta_saida)
    
    # Configurar estilo
    sns.set_style("whitegrid")
    plt.rcParams['figure.figsize'] = (12, 8)
    
    # 1. Comparação de Makespan por Heurística
    print("\n1. Gerando gráfico de comparação de makespan...")
    plt.figure(figsize=(14, 8))
    sns.boxplot(data=df, x='heuristica', y='valor', palette='Set2')
    plt.xticks(rotation=45, ha='right')
    plt.title('Comparação de Makespan por Heurística', fontsize=16, fontweight='bold')
    plt.xlabel('Heurística', fontsize=12)
    plt.ylabel('Makespan (Valor)', fontsize=12)
    plt.tight_layout()
    plt.savefig(f'{pasta_saida}/01_comparacao_makespan.png', dpi=300)
    plt.close()
    
    # 2. Tempo de Execução por Heurística
    print("2. Gerando gráfico de tempo de execução...")
    plt.figure(figsize=(14, 8))
    sns.boxplot(data=df, x='heuristica', y='tempo', palette='Set3')
    plt.xticks(rotation=45, ha='right')
    plt.title('Tempo de Execução por Heurística', fontsize=16, fontweight='bold')
    plt.xlabel('Heurística', fontsize=12)
    plt.ylabel('Tempo (segundos)', fontsize=12)
    plt.tight_layout()
    plt.savefig(f'{pasta_saida}/02_tempo_execucao.png', dpi=300)
    plt.close()
    
    # 3. Número de Iterações por Heurística
    print("3. Gerando gráfico de iterações...")
    plt.figure(figsize=(14, 8))
    sns.boxplot(data=df, x='heuristica', y='iteracoes', palette='Pastel1')
    plt.xticks(rotation=45, ha='right')
    plt.title('Número de Iterações por Heurística', fontsize=16, fontweight='bold')
    plt.xlabel('Heurística', fontsize=12)
    plt.ylabel('Iterações', fontsize=12)
    plt.tight_layout()
    plt.savefig(f'{pasta_saida}/03_numero_iteracoes.png', dpi=300)
    plt.close()
    
    # 4. Impacto do Parâmetro no Makespan (por heurística)
    print("4. Gerando gráficos de impacto de parâmetros...")
    heuristicas = df['heuristica'].unique()
    
    fig, axes = plt.subplots(2, 2, figsize=(16, 12))
    axes = axes.ravel()
    
    for idx, heuristica in enumerate(heuristicas):
        df_h = df[df['heuristica'] == heuristica]
        
        # Remover valores NA e agrupar por parâmetro
        df_h_clean = df_h[df_h['parametro'] != 'NA'].copy()
        if len(df_h_clean) > 0:
            df_h_clean['parametro'] = pd.to_numeric(df_h_clean['parametro'])
            
            # Calcular média e desvio padrão
            stats = df_h_clean.groupby('parametro')['valor'].agg(['mean', 'std']).reset_index()
            
            axes[idx].plot(stats['parametro'], stats['mean'], marker='o', linewidth=2, markersize=8)
            axes[idx].fill_between(stats['parametro'], 
                                   stats['mean'] - stats['std'], 
                                   stats['mean'] + stats['std'], 
                                   alpha=0.3)
            axes[idx].set_title(f'{heuristica}', fontsize=12, fontweight='bold')
            axes[idx].set_xlabel('Parâmetro', fontsize=10)
            axes[idx].set_ylabel('Makespan Médio', fontsize=10)
            axes[idx].grid(True, alpha=0.3)
    
    plt.suptitle('Impacto do Parâmetro no Makespan', fontsize=16, fontweight='bold')
    plt.tight_layout()
    plt.savefig(f'{pasta_saida}/04_impacto_parametros.png', dpi=300)
    plt.close()
    
    # 5. Qualidade vs Tempo (Trade-off)
    print("5. Gerando gráfico de trade-off qualidade vs tempo...")
    plt.figure(figsize=(14, 10))
    
    for heuristica in heuristicas:
        df_h = df[df['heuristica'] == heuristica]
        plt.scatter(df_h['tempo'], df_h['valor'], label=heuristica, alpha=0.6, s=50)
    
    plt.xlabel('Tempo de Execução (segundos)', fontsize=12)
    plt.ylabel('Makespan (Valor)', fontsize=12)
    plt.title('Trade-off: Qualidade vs Tempo de Execução', fontsize=16, fontweight='bold')
    plt.legend(fontsize=10)
    plt.grid(True, alpha=0.3)
    plt.tight_layout()
    plt.savefig(f'{pasta_saida}/05_tradeoff_qualidade_tempo.png', dpi=300)
    plt.close()
    
    # 6. Desempenho por Tamanho de Instância
    print("6. Gerando gráfico de desempenho por tamanho...")
    plt.figure(figsize=(14, 8))
    
    df['config'] = df['m'].astype(str) + 'm'
    sns.boxplot(data=df, x='config', y='valor', hue='heuristica', palette='Set2')
    plt.title('Makespan por Tamanho de Instância e Heurística', fontsize=16, fontweight='bold')
    plt.xlabel('Configuração (número de máquinas)', fontsize=12)
    plt.ylabel('Makespan', fontsize=12)
    plt.legend(title='Heurística', bbox_to_anchor=(1.05, 1), loc='upper left')
    plt.tight_layout()
    plt.savefig(f'{pasta_saida}/06_desempenho_tamanho.png', dpi=300)
    plt.close()
    
    print(f"\nGráficos salvos em: {pasta_saida}/")

def gerar_estatisticas(df):
    """Gera estatísticas resumidas"""
    
    print("\n" + "="*70)
    print("ESTATÍSTICAS RESUMIDAS")
    print("="*70)
    
    # Por heurística
    print("\n1. MAKESPAN MÉDIO POR HEURÍSTICA:")
    print("-" * 70)
    makespan_stats = df.groupby('heuristica')['valor'].agg(['mean', 'std', 'min', 'max'])
    makespan_stats = makespan_stats.sort_values('mean')
    print(makespan_stats.to_string())
    
    print("\n2. TEMPO MÉDIO POR HEURÍSTICA:")
    print("-" * 70)
    tempo_stats = df.groupby('heuristica')['tempo'].agg(['mean', 'std', 'min', 'max'])
    tempo_stats = tempo_stats.sort_values('mean')
    print(tempo_stats.to_string())
    
    print("\n3. ITERAÇÕES MÉDIAS POR HEURÍSTICA:")
    print("-" * 70)
    iter_stats = df.groupby('heuristica')['iteracoes'].agg(['mean', 'std', 'min', 'max'])
    iter_stats = iter_stats.sort_values('mean', ascending=False)
    print(iter_stats.to_string())
    
    print("\n4. MELHORES CONFIGURAÇÕES:")
    print("-" * 70)
    melhor_geral = df.loc[df['valor'].idxmin()]
    print(f"Melhor solução encontrada:")
    print(f"  Heurística: {melhor_geral['heuristica']}")
    print(f"  Makespan: {melhor_geral['valor']:.2f}")
    print(f"  Parâmetro: {melhor_geral['parametro']}")
    print(f"  Configuração: {melhor_geral['n']} tarefas, {melhor_geral['m']} máquinas")
    print(f"  Tempo: {melhor_geral['tempo']:.4f}s")
    print(f"  Iterações: {melhor_geral['iteracoes']}")
    
    print("\n5. PARÂMETROS ÓTIMOS POR HEURÍSTICA:")
    print("-" * 70)
    for heuristica in df['heuristica'].unique():
        df_h = df[df['heuristica'] == heuristica]
        df_h_clean = df_h[df_h['parametro'] != 'NA'].copy()
        
        if len(df_h_clean) > 0:
            df_h_clean['parametro'] = pd.to_numeric(df_h_clean['parametro'])
            melhor_param = df_h_clean.groupby('parametro')['valor'].mean().idxmin()
            makespan_medio = df_h_clean.groupby('parametro')['valor'].mean().min()
            
            print(f"\n{heuristica}:")
            print(f"  Melhor parâmetro: {melhor_param:.2f}")
            print(f"  Makespan médio: {makespan_medio:.2f}")

def main():
    """Função principal"""
    
    print("="*70)
    print(" ANÁLISE DE RESULTADOS - HEURÍSTICAS DE DISTRIBUIÇÃO DE TAREFAS")
    print("="*70)
    
    # Verificar se arquivo foi fornecido
    if len(sys.argv) < 2:
        print("\nUso: python analisar_resultados.py <arquivo_resultados.csv>")
        print("\nProcurando arquivo de resultados na pasta atual...")
        
        # Procurar arquivos CSV que começam com 'resultados_'
        arquivos = [f for f in os.listdir('.') if f.startswith('resultados_') and f.endswith('.csv')]
        
        if not arquivos:
            print("Nenhum arquivo de resultados encontrado!")
            return
        
        # Usar o mais recente
        arquivo = sorted(arquivos)[-1]
        print(f"Usando: {arquivo}")
    else:
        arquivo = sys.argv[1]
    
    # Verificar se arquivo existe
    if not os.path.exists(arquivo):
        print(f"Erro: Arquivo '{arquivo}' não encontrado!")
        return
    
    # Analisar
    df = analisar_resultados(arquivo)
    
    # Gerar estatísticas
    gerar_estatisticas(df)
    
    # Criar visualizações
    criar_visualizacoes(df)
    
    print("\n" + "="*70)
    print("Análise concluída!")
    print("="*70)

if __name__ == '__main__':
    main()
