namespace ChatApplicationServer.Services
{
    public class DeleteChatsService : BackgroundService
    {
        private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(24));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken)
                   && !stoppingToken.IsCancellationRequested)
            {
                await DeleteChatsAsync();
            }
        }

        private static async Task DeleteChatsAsync()
        {
            // Here implement logic for deleting chats
        }
    }
}
