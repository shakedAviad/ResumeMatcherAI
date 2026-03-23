using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace Infrastructure.Storage
{
    public class MessageStorage : ChatHistoryProvider
    {
        private readonly List<ChatMessage> _messages = [];
        public string? SessionId { get; set; }

        public string SessionPath => Path.Combine("data/messages", $"{SessionId}.json");

        protected override async ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(InvokingContext context, CancellationToken cancellationToken = default)
        {
            if (context.Session!.StateBag.TryGetValue("SessionId", out string? sessionId))
            {
                SessionId = sessionId!;
            }
            else
            {
                SessionId = Guid.NewGuid().ToString();
                context.Session.StateBag.SetValue("SessionId", SessionId);
            }

            if (File.Exists(SessionPath))
            {
                var json = await File.ReadAllTextAsync(SessionPath, cancellationToken);

                return JsonSerializer.Deserialize<List<ChatMessage>>(json)!;
            }

            return [];
        }

        protected override async ValueTask StoreChatHistoryAsync(InvokedContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            _messages.AddRange(context.RequestMessages.Concat(context.ResponseMessages ?? []));

            Directory.CreateDirectory(Path.GetDirectoryName(SessionPath)!);

            await File.WriteAllTextAsync(SessionPath, JsonSerializer.Serialize(_messages), cancellationToken);
        }
    }
}
