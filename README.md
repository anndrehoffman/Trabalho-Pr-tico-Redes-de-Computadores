# Trabalho-Prático-Redes-de-Computadores

# 🃏 Jogo de Cartas “21” – Cliente/Servidor em C#

## 🎯 Propósito do Projeto

Este projeto implementa uma versão em rede do clássico jogo de cartas “21”, no qual jogadores tentam acumular pontos até chegar o mais próximo possível de 21 sem ultrapassar esse valor. A arquitetura é baseada no modelo cliente-servidor: o servidor gerencia o jogo, as cartas e as pontuações, enquanto os clientes representam os jogadores que se conectam e interagem com o jogo.

---

## 💻 Linguagem Utilizada

- C# 
- Projeto estruturado com duas aplicações: `Servidor` e `Cliente`.

---

## 🔧 Como Compilar

### ✅ Pré-requisitos

- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) instalado no sistema.
- Um terminal ou prompt de comando.

### 🧱 Compilação

Abra o terminal na raiz do repositório e execute:

``bash
cd Servidor
dotnet build

cd ../Cliente
dotnet build
``
## 🕹️ Comandos do Jogo (pelo Cliente)

Durante a execução do cliente, o jogador deve interagir via menus no terminal:

### 📋 Menu Inicial
- `1` → Entrar no jogo
- `2` → Sair

### 🎯 Durante a Jogada
- `1` → 🃏 Pedir uma carta
- `2` → ✋ Parar (finalizar sua rodada)


