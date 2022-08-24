namespace Ordering.AsyncDataServices
{
    public class MessageBusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageBusBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            using (var scope = _serviceProvider.CreateScope())
            {
                var messageBusSubscriber = scope.ServiceProvider.GetService<IMessageBusSubscriber>();

                if (messageBusSubscriber != null)
                {
                    messageBusSubscriber.CreateConsumer();
                }
            }

            return Task.CompletedTask;
        }
    }
}