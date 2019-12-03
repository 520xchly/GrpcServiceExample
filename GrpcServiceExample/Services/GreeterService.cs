using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcServiceExample
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// һԪ����
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        /// <summary>
        /// �ͻ�����ʽ����
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<HelloReply> SayClientStreamingHello(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            // ȫ����ȡ
            await foreach (var request in requestStream.ReadAllAsync())
            {
                Console.WriteLine("���յ��ͻ�����ʽ��Ϣ:" + request.Name);
            }

            // ���
            //while (await requestStream.MoveNext())
            //{
            //    Console.WriteLine("���յ��ͻ�����ʽ��Ϣ:" + requestStream.Current.Name);
            //}

            return new HelloReply() { Message = "ok" };
        }

        /// <summary>
        /// �������Ӧ��
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task SayServerStreamingHello(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                string requestName = request.Name;
                string msg = !string.IsNullOrWhiteSpace(requestName)
                    ? $"���� :{ requestName} !"
                    : "δ��ȡ������Ϣ";
                await responseStream.WriteAsync(new HelloReply()
                {
                    Message = msg
                });

                await Task.Delay(5000);
            }

        }

        public override async Task SayBiDirectionalStreamingHello(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var name = requestStream.Current.Name;
                string msg = string.Empty;
                if (string.IsNullOrEmpty(name))
                {
                    msg = "����ô���˸�����Ϣ����";
                }
                else 
                {
                    msg = "���յ����㷢����Ϣ:"+name;
                }
                await responseStream.WriteAsync(new HelloReply()
                {
                    Message = msg
                });
            }
        }
    }
}
