using ChatApplicationServer.Models2;
using System;
using System.Linq;

namespace ChatApplicationServer.Services
{
    public class DeleteChatsService : BackgroundService
    {
        private readonly PeriodicTimer _timer = new(TimeSpan.FromDays(30));

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
            using (var appContext = new ChatAppContext())
            {
                var chatRooms = appContext.ChatRooms.ToList();

                foreach (var chatRoom in chatRooms)
                {
                    var messageSentAtList = appContext.MessagesChatRooms.Where(mcr => mcr.ChatRoomId == chatRoom.Id)
                                                                        .Select(mcr => mcr.Message.SentAt)
                                                                        .OrderBy(dateTime => dateTime.Date).ThenBy(dateTime => dateTime.TimeOfDay)
                                                                        .ToList();

                    if (messageSentAtList.Any())
                    {
                        if (messageSentAtList.Last() < DateTime.Now.AddDays(-30))
                        {
                            appContext.ChatRooms.Remove(chatRoom);
                            appContext.Messages.RemoveRange(appContext.Messages.Where(m => m.ChatId == chatRoom.Id));
                            appContext.MessagesChatRooms.RemoveRange(appContext.MessagesChatRooms.Where(ucr => ucr.ChatRoomId == chatRoom.Id));
                            appContext.UsersChatRooms.RemoveRange(appContext.UsersChatRooms.Where(ucr => ucr.ChatRoomId == chatRoom.Id));
                        }
                    }
                }
                appContext.SaveChanges();
            }
        }
    }
}
