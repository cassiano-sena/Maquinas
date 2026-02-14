#!/bin/bash

echo "===================================================="
echo "  HEURÍSTICAS DE DISTRIBUIÇÃO DE TAREFAS"
echo "===================================================="
echo ""

echo "Verificando .NET SDK..."
if ! command -v dotnet &> /dev/null; then
    echo "ERRO: .NET SDK não encontrado!"
    echo "Baixe em: https://dotnet.microsoft.com/download"
    exit 1
fi

echo "Versão do .NET: $(dotnet --version)"
echo ""

echo "Compilando projeto..."
dotnet build -c Release

if [ $? -ne 0 ]; then
    echo ""
    echo "ERRO na compilação!"
    exit 1
fi

echo ""
echo "===================================================="
echo "  INICIANDO EXECUÇÃO DOS EXPERIMENTOS"
echo "===================================================="
echo ""
echo "ATENÇÃO: A execução pode levar de 5 a 15 minutos!"
echo ""
read -p "Pressione ENTER para continuar..."

dotnet run -c Release

echo ""
echo "===================================================="
echo "  EXECUÇÃO CONCLUÍDA!"
echo "===================================================="
echo ""
echo "Arquivos gerados:"
ls -1 resultados_*.csv 2>/dev/null
ls -1 analise_*.txt 2>/dev/null
echo ""
