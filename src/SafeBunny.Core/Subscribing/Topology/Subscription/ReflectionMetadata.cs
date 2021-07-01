using System;
using System.Reflection;
using SafeBunny.Core.Message;
using SafeBunny.Core.Subscribing.Pipeline;

namespace SafeBunny.Core.Subscribing.Topology.Subscription
{
    internal sealed class ReflectionMetadata
    {
        public ReflectionMetadata(Type type)
        {
            HandlerType = typeof(IMessageHandler<>).MakeGenericType(type);
            MethodInfo = HandlerType.GetMethod(nameof(IMessageHandler<HandlerMetadata>.HandleAsync));
            ProcessingContextType = typeof(ProcessingContext<>).MakeGenericType(type);
            ProcessingContextCtor = ProcessingContextType
                .GetConstructor(BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new[] {typeof(object), typeof(MessageProperties), typeof(HandlerMetadata)},
                    null);
        }

        internal readonly Type HandlerType;
        internal readonly MethodInfo MethodInfo;
        internal readonly Type ProcessingContextType;
        internal readonly ConstructorInfo ProcessingContextCtor;

        public IProcessingContext NewContext(object message, MessageProperties props, HandlerMetadata metadata)
            => (IProcessingContext) ProcessingContextCtor.Invoke(new[] {message, props, metadata});
    }
}