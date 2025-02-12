﻿using Findx.DependencyInjection;
using Findx.Extensions;
using Findx.Reflection;
using Findx.Serialization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Findx.RabbitMQ
{
    public class RabbitConsumerBuilder : IRabbitConsumerBuilder
    {
        private readonly IRabbitConsumerFinder _finder;
        private readonly IMethodInfoFinder _methodFinder;
        private readonly IRabbitMQConsumerFactory _factory;
        private readonly IJsonSerializer _serializer;

        public RabbitConsumerBuilder(IRabbitConsumerFinder finder, IMethodInfoFinder methodFinder, IRabbitMQConsumerFactory factory, IJsonSerializer serializer)
        {
            _finder = finder;
            _methodFinder = methodFinder;
            _factory = factory;
            _serializer = serializer;
            Consumers = new List<IRabbitMQConsumer>();
            MethodParameter = new ConcurrentDictionary<MethodInfo, Type>();
            Handlers = new ConcurrentDictionary<MethodInfo, Func<object, object[], object>>();
        }

        public void Build()
        {
            var types = _finder.FindAll(true);

            foreach (var type in types)
            {
                var methods = _methodFinder.Find(type, it => it.HasAttribute<RabbitConsumerAttribute>());
                foreach (var method in methods)
                {
                    var attr = method.GetAttribute<RabbitConsumerAttribute>();
                    var exchangeDeclareConfiguration = new ExchangeDeclareConfiguration(attr.ExchangeName, attr.Type);
                    var queueDeclareConfiguration = new QueueDeclareConfiguration(attr.QueueName, qos: attr.Qos) { Arguments = new Dictionary<string, object> { { "x-queue-mode", "lazy" } } };

                    var consumer = _factory.Create(exchange: exchangeDeclareConfiguration, queue: queueDeclareConfiguration);

                    consumer.Bind(attr.RoutingKey);

                    consumer.OnMessageReceived(async (channel, eventArgs) =>
                    {
                        var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                        using (var scope = ServiceLocator.ServiceProvider.CreateScope())
                        {
                            var serviceProvider = scope.ServiceProvider;

                            var instance = serviceProvider.GetService(type);

                            var parameterType = MethodParameter.GetOrAdd(method, it => { return method.GetParameters()[0].ParameterType; });

                            var parameter = _serializer.Deserialize(message, parameterType);

                            Func<object, object[], object> handler = Handlers.GetOrAdd(method, it => { return FastInvokeHandler.Create(method); });

                            var result = handler.Invoke(instance, new object[] { parameter });

                            if (method.IsAsync()) await (Task)result;
                        }
                    });

                    Consumers.Add(consumer);

                    // 启动开始消费
                    consumer.StartConsuming();
                }
            }
        }

        private List<IRabbitMQConsumer> Consumers { get; }
        private ConcurrentDictionary<MethodInfo, Type> MethodParameter { get; }
        private ConcurrentDictionary<MethodInfo, Func<object, object[], object>> Handlers { get; }
    }
}
