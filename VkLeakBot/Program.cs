using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<VkService>();

var app = builder.Build();

// === ТВОИ НАСТРОЙКИ (замени своими!) ===
const string AccessToken = "2Hcl1-C05wvk1.a.gnweqCC6CEOGB9m9BfQqCuSwQcn2hUb7fQBeUDaFA8mO1y3sqhPUJpYpJ1x-bNZtSN4Vcy0wW6eOQ0KmNIWEJqaA-JZes_RirZ2hJ9DM9g60O_jcRqSHyogMP4e8V9A2a2Cmo4R_3En_GAeh8ezoNbKoKLC8dLQRMd52rI5mDWU9dZScMC5uQdz4myu8xu1Lc2h5SdMvHaRQ"; 
const string ConfirmationSecret = "superbot2026"; 
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