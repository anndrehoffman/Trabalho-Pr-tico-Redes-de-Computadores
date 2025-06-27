using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;

class Jogador
{
    public string Nome;
    public int Pontuacao = 0;
    public bool Ativo = true;
    public DateTime UltimoKeepAlive = DateTime.Now;
}

class Program
{
    static Dictionary<IPEndPoint, Jogador> jogadores = new();
    static UdpClient servidor = new UdpClient(11000);
    static System.Timers.Timer keepAliveTimer;

    static void Main()
    {
        Console.WriteLine("Servidor iniciado na porta 11000...");
        IniciarVerificacaoKeepAlive();

        while (true)
        {
            IPEndPoint clienteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] dadosRecebidos = servidor.Receive(ref clienteEP);
            string mensagem = Encoding.UTF8.GetString(dadosRecebidos);

            Console.WriteLine($"Recebido de {clienteEP}: {mensagem}");
            ProcessarMensagem(mensagem, clienteEP);
        }
    }

    static void ProcessarMensagem(string mensagem, IPEndPoint clienteEP)
    {
        if (!jogadores.ContainsKey(clienteEP) && !mensagem.StartsWith("ENTRAR:")) return;

        if (mensagem.StartsWith("ENTRAR:"))
        {
            string nome = mensagem.Split(':')[1];
            jogadores[clienteEP] = new Jogador { Nome = nome };
            Enviar(clienteEP, $"MENSAGEM:Bem-vindo ao jogo, {nome}!");
            RegistrarLog($"Jogador '{nome}' entrou no jogo ({clienteEP})");

            if (jogadores.Count < 2)
                Enviar(clienteEP, "MENSAGEM:Aguardando outros jogadores para iniciar a rodada...");
            else
                Enviar(clienteEP, "MENSAGEM:Jogadores conectados. Você pode começar a jogar!");
        }
        else if (mensagem == "PEDIR_CARTA")
        {
            var jogador = jogadores[clienteEP];
            if (!jogador.Ativo) return;

            int carta = new Random().Next(1, 12);
            jogador.Pontuacao += carta;
            Enviar(clienteEP, $"CARTA:{carta}");
            RegistrarLog($"{jogador.Nome} recebeu a carta {carta}. Total: {jogador.Pontuacao}");

            if (jogador.Pontuacao > 21)
            {
                jogador.Ativo = false;
                Enviar(clienteEP, "RESULTADO:perdeu");
                VerificarEncerramento();
            }
        }
        else if (mensagem == "PARAR")
        {
            var jogador = jogadores[clienteEP];
            jogador.Ativo = false;

            string textoParada = $"MENSAGEM:{jogador.Nome} parou com {jogador.Pontuacao} pontos.";
            foreach (var ep in jogadores.Keys)
            {
                Enviar(ep, textoParada);
            }

            RegistrarLog($"{jogador.Nome} parou com {jogador.Pontuacao} pontos.");
            VerificarEncerramento();
        }
        else if (mensagem == "KEEPALIVE")
        {
            jogadores[clienteEP].UltimoKeepAlive = DateTime.Now;
        }
    }

    static void VerificarEncerramento()
    {
        bool todosPararam = true;
        foreach (var j in jogadores.Values)
        {
            if (j.Ativo && j.Pontuacao <= 21)
            {
                todosPararam = false;
                break;
            }
        }

        if (todosPararam)
        {
            int maior = -1;
            string vencedor = "";

            foreach (var par in jogadores)
            {
                var j = par.Value;
                if (j.Pontuacao <= 21 && j.Pontuacao > maior)
                {
                    maior = j.Pontuacao;
                    vencedor = j.Nome;
                }
            }

            foreach (var par in jogadores)
            {
                string resultado = (par.Value.Nome == vencedor) ? "ganhou" : "perdeu";
                Enviar(par.Key, $"RESULTADO:{resultado}");
                RegistrarLog($"{par.Value.Nome} {resultado.ToUpper()} com {par.Value.Pontuacao} pontos.");
            }

            RegistrarLog("Rodada finalizada.");
            Console.WriteLine("Rodada encerrada. Reinicie o servidor para nova rodada.");
            Environment.Exit(0);
        }
    }

    static void Enviar(IPEndPoint destino, string mensagem)
    {
        byte[] dados = Encoding.UTF8.GetBytes(mensagem);
        servidor.Send(dados, dados.Length, destino);
    }

    static void RegistrarLog(string mensagem)
    {
        string linha = $"[{DateTime.Now:HH:mm:ss}] {mensagem}";
        File.AppendAllText("log.txt", linha + Environment.NewLine);
        Console.WriteLine("LOG: " + linha);
    }

    static void IniciarVerificacaoKeepAlive()
    {
        keepAliveTimer = new System.Timers.Timer(10000); // a cada 10s
        keepAliveTimer.Elapsed += (s, e) =>
        {
            List<IPEndPoint> desconectados = new();
            foreach (var par in jogadores)
            {
                if ((DateTime.Now - par.Value.UltimoKeepAlive).TotalSeconds > 30)
                {
                    desconectados.Add(par.Key);
                }
            }

            foreach (var ep in desconectados)
            {
                RegistrarLog($"Jogador '{jogadores[ep].Nome}' desconectado por inatividade.");
                jogadores.Remove(ep);
            }
        };
        keepAliveTimer.Start();
    }
}        
