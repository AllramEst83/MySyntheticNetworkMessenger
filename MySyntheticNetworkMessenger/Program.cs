using Microsoft.Extensions.DependencyInjection;
using MySyntheticNetworkMessenger.Hubs;
using MySyntheticNetworkMessenger.Models;
using MySyntheticNetworkMessenger.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("ApplicationSettings"));

builder.Services
    .AddTransient<IChatGptService, ChatGptService>()
    .AddSingleton<IChatHistoryService, ChatHistoryService>()
    .AddSingleton<IContactService, ContactService>()
    .AddSingleton<ITemplateBuilderService, TemplateBuilderService>();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub");

app.Run();
