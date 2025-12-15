using Blackjack.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// EF Core + Azure SQL
builder.Services.AddDbContext<BlackjackDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

//Static files (wwwroot)
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
