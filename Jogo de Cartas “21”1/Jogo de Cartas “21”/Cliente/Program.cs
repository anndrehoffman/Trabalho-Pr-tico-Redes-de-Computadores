using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static UdpClient cliente = new UdpClient();
    static string nome = "";
    static bool executando = true;

    static void Main()
    {
        Thread keepAliveThread = new Thread(EnviarKeepAlive);
        keepAliveThread.Start();

        Console.WriteLine("🎲 === Bem-vindo ao Jogo de Cartas 21 ===");

        while (true)
        {
            Console.WriteLine("\n📋 Menu Inicial:");
            Console.WriteLine("1 - Entrar no jogo");
            Console.WriteLine("2 - Sair");

            Console.Write("Escolha uma opção: ");
            string opcao = Console.ReadLine();

            if (opcao == "1")
            {
                Console.Write("Digite seu nome: ");
                nome = Console.ReadLine();
                Enviar($"ENTRAR:{nome}");

                // Espera resposta
                IPEndPoint servidorEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dadosRecebidos = cliente.Receive(ref servidorEP);
                string mensagem = Encoding.UTF8.GetString(dadosRecebidos);

                if (mensagem.StartsWith("MENSAGEM:"))
                {
                    string texto = mensagem.Split(':')[1];
                    Console.WriteLine($"\n📢 {texto}");

                    if (texto.Contains("Aguardando"))
                    {
                        Console.WriteLine("⏳ Aguarde mais jogadores entrarem...\n");
                        Thread.Sleep(3000);
                    }
                }

                Jogar();
                break;
            }
            else if (opcao == "2")
            {
                Console.WriteLine("👋 Saindo do jogo...");
                executando = false;
                return;
            }
            else
            {
                Console.WriteLine("❌ Opção inválida.");
            }
        }
    }

    static void Jogar()
    {
        while (true)
        {
            Console.WriteLine("\n🎯 === Sua Jogada ===");
            Console.WriteLine("1 - 🃏 Pedir carta");
            Console.WriteLine("2 - ✋ Parar");

            Console.Write("Escolha: ");
            string escolha = Console.ReadLine();

            if (escolha == "1")
            {
                Enviar("PEDIR_CARTA");
            }
            else if (escolha == "2")
            {
                Enviar("PARAR");
                
                        try
                        {
                            // Mostra mensagem de parada
                            IPEndPoint servidorEP = new IPEndPoint(IPAddress.Any, 0);
                            byte[] dadosRecebidos = cliente.Receive(ref servidorEP);
                            string mensagem = Encoding.UTF8.GetString(dadosRecebidos);

                            if (mensagem.StartsWith("MENSAGEM:"))
                            {
                                string texto = mensagem.Split(':')[1];
                                Console.WriteLine($"📢 {texto}");
                            }
                        }
                        catch
                        {
                            Console.WriteLine("⚠️ Nenhuma resposta após PARAR.");
                        }

                        // Continua esperando pelo resultado final
                        continue;
            }

            try
            {
                IPEndPoint servidorEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dadosRecebidos = cliente.Receive(ref servidorEP);
                string mensagem = Encoding.UTF8.GetString(dadosRecebidos);

                if (mensagem.StartsWith("CARTA:"))
                    Console.WriteLine($"🃏 Você recebeu a carta: {mensagem.Split(':')[1]}");
                else if (mensagem.StartsWith("RESULTADO:"))
                {
                    Console.WriteLine(mensagem.Contains("ganhou") ? "🎉 Você venceu!" : "💥 Você perdeu.");
                    break;
                }
                else if (mensagem.StartsWith("MENSAGEM:"))
                    Console.WriteLine($"📢 {mensagem.Split(':')[1]}");
            }
            catch
            {
                Console.WriteLine("⚠️ Nenhuma resposta do servidor.");
            }
        }

        cliente.Close();
        executando = false;
        Console.WriteLine("🎮 Jogo encerrado.");
    }

    static void Enviar(string mensagem)
    {
        byte[] dados = Encoding.UTF8.GetBytes(mensagem);
        cliente.Send(dados, dados.Length, "127.0.0.1", 11000);
    }

    static void EnviarKeepAlive()
    {
        while (executando)
        {
            Enviar("KEEPALIVE");
            Thread.Sleep(5000);
        }
    }
}
