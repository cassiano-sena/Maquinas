@echo off
echo ====================================================
echo   HEURISTICAS DE DISTRIBUICAO DE TAREFAS
echo ====================================================
echo.

echo Verificando .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERRO: .NET SDK nao encontrado!
    echo Baixe em: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo.
echo Compilando projeto...
dotnet build -c Release

if errorlevel 1 (
    echo.
    echo ERRO na compilacao!
    pause
    exit /b 1
)

echo.
echo ====================================================
echo   INICIANDO EXECUCAO DOS EXPERIMENTOS
echo ====================================================
echo.
echo ATENCAO: A execucao pode levar de 5 a 15 minutos!
echo.
pause

dotnet run -c Release

echo.
echo ====================================================
echo   EXECUCAO CONCLUIDA!
echo ====================================================
echo.
echo Arquivos gerados:
dir resultados_*.csv /b 2>nul
dir analise_*.txt /b 2>nul
echo.
pause
