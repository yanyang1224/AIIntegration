using AIIntegration.Library.Interfaces;
using AIIntegration.Library.Models;
using AIIntegration.Library.Services.OpenAI;
using AIIntegration.Library.Services.Claude;
using AIIntegration.Library.Services.DeepSpeek;
using AIIntegration.Library.Services.Qwen;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AI Integration API", Version = "v1" });
});

// Configure AI services
builder.Services.AddSingleton<AIServiceConfig>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new AIServiceConfig
    {
        ApiUrl = configuration["OpenAI:ApiUrl"]!,
        ApiKey = configuration["OpenAI:ApiKey"]!,
        Model = configuration["OpenAI:Model"]!
    };
});

builder.Services.AddHttpClient<IAIService, OpenAIService>();
builder.Services.AddHttpClient<IAIService, ClaudeService>();
builder.Services.AddHttpClient<IAIService, DeepSpeekService>();
builder.Services.AddHttpClient<IAIService, QwenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Integration API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();