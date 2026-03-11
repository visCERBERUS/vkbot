using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<VkService>();

var app = builder.Build();

// === ТВОИ НАСТРОЙКИ (замени своими!) ===

const string ConfirmationSecret = "24649f82";
const string CallbackPath = "/callback";

// === Callback от VK ===
app.MapPost(CallbackPath, async (HttpRequest req) =>
{
    using var reader = new StreamReader(req.Body);
    var json = await reader.ReadToEndAsync();
    var data = JsonDocument.Parse(json).RootElement;

    if (data.GetProperty("type").GetString() == "confirmation")
        return Results.Text(ConfirmationSecret);

    if (data.GetProperty("type").GetString() == "message_new")
    {
        var msg = data.GetProperty("object").GetProperty("message");
        var userId = msg.GetProperty("from_id").GetInt64();
        var text = msg.GetProperty("text").GetString()?.ToLower() ?? "";

        string? payload = null;
        if (msg.TryGetProperty("payload", out var p))
            payload = p.GetString();

        var vk = app.Services.GetRequiredService<VkService>();
        await HandleUserMessage(userId, text, payload, vk);
    }

    return Results.Text("ok");
});

app.Run();

// === ОБРАБОТКА КНОПОК И СООБЩЕНИЙ ===
async Task HandleUserMessage(long userId, string text, string? payload, VkService vk)
{
    string response = "👋 Привет! Я бот защиты от протечек.\nВыбери действие ниже:";

    string command = "";
    if (!string.IsNullOrEmpty(payload))
    {
        try
        {
            var payloadJson = JsonDocument.Parse(payload).RootElement;
            command = payloadJson.GetProperty("command").GetString() ?? "";
        }
        catch { }
    }

    if (command == "close") response = "🚰 ВОДА ПЕРЕКРЫТА! (симуляция)";
    else if (command == "status") response = "📡 Статус: ВСЁ В НОРМЕ. Протечек нет.";
    else if (command == "register") response = "✅ Датчик привязан!\nТеперь можно управлять.";
    else if (text.Contains("старт") || text.Contains("привет")) 
        response = "🚀 Бот готов! Нажимай кнопки:";

    await vk.SendMessageWithKeyboard(userId, response);
}
