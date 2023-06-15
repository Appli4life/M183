using Microsoft.EntityFrameworkCore;
using Session.Context;

var builder = WebApplication.CreateBuilder(args);

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
    app.UseExceptionHandler("/Home/Error");
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
