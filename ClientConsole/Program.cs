using ClientConsole.Service;
using FoxMQ.Proto;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using Grpc.Auth;

namespace ClientConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pressiona para iniciar");
            Console.ReadLine();


            using var channel = GrpcChannel.ForAddress("https://localhost:5051");
            var client = new FoxMQGrpcService.FoxMQGrpcServiceClient(channel);

            var reply = client.Publish(new PublishRequest { Queue = "RequestMessage" });

            Console.WriteLine("RequestMessage: " + reply.Id);

            //IServiceCollection service = new ServiceCollection();
            //service.AddScoped<IFoxMessage, FoxMessage>();
            //service.AddGrpcClient<FoxMQQueue.FoxMQQueueClient>(options =>
            //{
            //    options.Address = new Uri("https://localhost:5501");
            //});

            //Console.WriteLine("Pressiona para continuar");
            //Console.ReadLine();

            //ServiceProvider serviceProvider = service.BuildServiceProvider();

            //Comunicar comunicar = new Comunicar(serviceProvider.GetService<IFoxMessage>());

            //comunicar.ComunicarMensagem();

            Console.WriteLine("Mensagem Enviada!");
        }
    }

    public class Comunicar
    {
        private readonly IFoxMessage _foxMessage;

        public Comunicar(IFoxMessage foxMessage)
        {
            _foxMessage = foxMessage;
        }

        public void ComunicarMensagem()
        {
            _foxMessage.PublishMessage();
        }
    }
}
