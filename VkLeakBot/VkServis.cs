public class VkService
{
    private readonly string _token = "vk1.a.ТВОЙ_ТОКЕН_СООБЩЕСТВА_СЮДА_ПОЛНОСТЬЮ";
    private readonly HttpClient _http = new();

    private string GetKeyboardJson()
    {
        return @"{
            ""one_time"": false,
            ""buttons"": [
                [
                    { ""action"": { ""type"": ""text"", ""label"": ""🚰 Перекрыть воду"", ""payload"": ""{\""command\"": \""close\""}"" }, ""color"": ""negative"" }
                ],
                [
                    { ""action"": { ""type"": ""text"", ""label"": ""📡 Статус"", ""payload"": ""{\""command\"": \""status\""}"" }, ""color"": ""primary"" },
                    { ""action"": { ""type"": ""text"", ""label"": ""🔧 Привязать датчик"", ""payload"": ""{\""command\"": \""register\""}"" }, ""color"": ""positive"" }
                ]
            ]
        }";
    }

    public async Task SendMessageWithKeyboard(long peerId, string text)
    {
        var keyboard = GetKeyboardJson();
        var url = $"https://api.vk.com/method/messages.send?v=5.199" +
                  $"&access_token={_token}" +
                  $"&peer_id={peerId}" +
                  $"&message={Uri.EscapeDataString(text)}" +
                  $"&keyboard={Uri.EscapeDataString(keyboard)}" +
                  $"&random_id={Random.Shared.Next(1000000, 9999999)}";

        await _http.GetStringAsync(url);
    }
}