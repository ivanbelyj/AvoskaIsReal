using AvoskaIsReal.Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

string connection = builder.Configuration.GetSection("Project")
    .GetConnectionString("Connection");
builder.Services.AddDbContext<AppDbContext>((options) =>
{
    options.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 11)));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios,
    // see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// app.UseAuthorization();

app.UseEndpoints(configure =>
{
    configure.MapControllerRoute("default", "{Controller=Home}/{Action=Index}/{id?}");
});
// app.MapRazorPages();

app.Run();
