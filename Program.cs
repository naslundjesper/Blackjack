using Blackjack.Data;
using Blackjack.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Kestrel konfiguration
builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(80); });

// Registrera tjänster
builder.Services.AddControllersWithViews(); // Denna räcker för både MVC och API

var connString = builder.Configuration.GetConnectionString("DefaultConnection")
                 ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

builder.Services.AddDbContext<BlackjackDbContext>(options => options.UseSqlServer(connString));

builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<RoundService>();
builder.Services.AddScoped<BlackjackRuleService>();

var app = builder.Build();

// AUTOMATISK MIGRERING
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BlackjackDbContext>();
    for (int i = 0; i < 10; i++)
    {
        try { db.Database.Migrate(); break; }
        catch { Console.WriteLine("Väntar på SQL Server..."); Thread.Sleep(3000); }
    }
}


app.UseStaticFiles();
app.UseRouting();

// Mappa rutter
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();