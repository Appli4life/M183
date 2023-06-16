using System.Diagnostics;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Session.Context;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .MinimumLevel.Debug()
    .WriteTo.File("logs/logs-.txt", rollingInterval: RollingInterval.Day, formatProvider: CultureInfo.InvariantCulture)
    .CreateLogger();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<SessionDBContext>(o =>
{
    o.UseInMemoryDatabase("SessionM183");
});

// Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(o =>
{
    o.Cookie.IsEssential = true;
    o.IdleTimeout = TimeSpan.FromHours(1);
    o.Cookie.HttpOnly = true;
    o.IOTimeout = TimeSpan.FromMinutes(1);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SessionDBContext>();
    context.Seed();
}

app.Run();
