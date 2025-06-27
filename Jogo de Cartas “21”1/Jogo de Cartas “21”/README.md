# Jogo de Cartas 21 (Blackjack) com Sockets UDP - C#

Este projeto é um trabalho prático da disciplina de Redes de Computadores. Ele implementa o jogo de cartas "21" (Blackjack) com comunicação via Sockets UDP.

## 🖥️ Requisitos

- .NET SDK 6.0 ou superior
- Visual Studio Code ou outro editor C#

## ▶️ Como executar

### 1. Rodar o servidor

Abra o terminal e digite:

```bash
cd Servidor
dotnet run
```
### 2. Rodar o cliente (em outro terminal)

```bash
cd Cliente
dotnet run
```
### 📄 Log das partidas

O servidor salva todas as ações (entradas, cartas, paradas, resultado) no arquivo `log.txt` automaticamente.

Você pode abri-lo após o jogo terminar.