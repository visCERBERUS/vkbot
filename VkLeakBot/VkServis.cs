public class VkService
{
    private readonly string _token = "vk1.a.lCH5o_lW7LJEsHLT95gPZqx4hgHiPQD9luP5bz0AUGwRvqLFPdewo8yEDLd3-y65VBQ52FJ4q3M8lrkL60oMFK6iuV7TqzX-e8hpo7wcvTV3I__0TddR7rUbsp29oend7FJNfO_TnR0ySVL_iiIsg4K3lnklxcJdZxe424AgbL2eirUhsIT2e1nPMRusJbjYsjJpIonbGMX_-rvoaKea2Q";
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
