using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AvoskaIsReal.Domain;
using AvoskaIsReal.Domain.Repositories.Abstract;
using AvoskaIsReal.Domain.Repositories.EntityFramework;
using AvoskaIsReal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTransient<IArticlesRepository, EFArticlesRepository>();
builder.Services.AddTransient<ITextFieldsRepository, EFTextFieldsRepository>();
builder.Services.AddTransient<DataManager>();

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>((options) =>
{
    options.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 11)));
});
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Initialize Db.
using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    UserManager<User> userManager =
        (UserManager<User>)services.GetRequiredService(typeof(UserManager<User>));
    RoleManager<IdentityRole> roleManager = (RoleManager<IdentityRole>)services
        .GetRequiredService(typeof(RoleManager<IdentityRole>));
    await DbInitializer.InitializeAsync(userManager, roleManager, app.Configuration);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios,
    // see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(configure =>
{
    // configure.MapControllerRoute("areas", "{area:exists}/{Controller}/{Action=Index}/{id?}");
    configure.MapAreaControllerRoute(
        name: "admin_area",
        areaName: "admin",
        pattern: "admin/{controller}/{action}/{id?}"
        );
    configure.MapAreaControllerRoute(
        name: "moderator_area",
        areaName: "moderator",
        pattern: "moderator/{controller}/{action}/{id?}"
        );
    configure.MapControllerRoute("default", "{Controller=Home}/{Action=Index}/{id?}");
});

app.Run();
