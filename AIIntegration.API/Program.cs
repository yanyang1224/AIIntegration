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
        OpenAI = new ServiceConfig
        {
            ApiUrl = configuration["OpenAI:ApiUrl"]!,
            ApiKey = configuration["OpenAI:ApiKey"]!,
            Model = configuration["OpenAI:Model"]!
        },
        Claude = new ServiceConfig
        {
            ApiUrl = configuration["Claude:ApiUrl"]!,
            ApiKey = configuration["Claude:ApiKey"]!,
            Model = configuration["Claude:Model"]!
        },
        DeepSpeek = new ServiceConfig
        {
            ApiUrl = configuration["DeepSpeek:ApiUrl"]!,
            ApiKey = configuration["DeepSpeek:ApiKey"]!,
            Model = configuration["DeepSpeek:Model"]!
        },
        Qwen = new ServiceConfig
        {
            ApiUrl = configuration["Qwen:ApiUrl"]!,
            ApiKey = configuration["Qwen:ApiKey"]!,
            Model = configuration["Qwen:Model"]!
        }
    };
});

builder.Services.AddTransient<IAIService, OpenAIService>();
builder.Services.AddTransient<IAIService, ClaudeService>();
builder.Services.AddTransient<IAIService, DeepSpeekService>();
builder.Services.AddTransient<IAIService, QwenService>();

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