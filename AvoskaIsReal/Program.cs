using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AvoskaIsReal.Domain;
using AvoskaIsReal.Domain.Repositories.Abstract;
using AvoskaIsReal.Domain.Repositories.EntityFramework;
using AvoskaIsReal;
using Microsoft.AspNetCore.Authentication.Cookies;
using AvoskaIsReal.Service;
using Microsoft.AspNetCore.Authorization;
using AvoskaIsReal.Service.Images;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

builder.Services.AddTransient<IArticlesRepository, EFArticlesRepository>();
builder.Services.AddTransient<ITextFieldsRepository, EFTextFieldsRepository>();
builder.Services.AddTransient<DataManager>();
builder.Services.AddTransient<AppUserRoleManager>();
builder.Services.AddScoped<IAuthorizationHandler, EditOrDeleteAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ChangeRoleAuthorizationHandler>();
builder.Services.AddTransient<ImageService>();
builder.Services.AddTransient<IImageProfile, ContentImageProfile>();
builder.Services.AddTransient<IImageProfile, AvatarProfile>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "avoskaIsRealAuth";
        options.Cookie.HttpOnly = true;
        options.LoginPath = "/account/login";
        options.AccessDeniedPath = "/account/accessdenied";  // Todo: access denied page
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminArea", options => options.RequireRole("admin"));
    options.AddPolicy("ModeratorArea", options => options.RequireRole("moderator"));
    options.AddPolicy("EditOrDeleteUserPolicy", options => options.Requirements
        .Add(new EditOrDeleteRequirement()));
    //options.AddPolicy("ChangeRolePolicy", options => options.Requirements
    //    .Add(new ChangeRoleAuthorizationRequirement()));
});

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>((options) =>
{
    options.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 11)));
}, ServiceLifetime.Scoped);

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddControllersWithViews(options =>
{
    options.Conventions.Add(new PolicyAreaAuthorization("AdminArea", "admin"));
    options.Conventions.Add(new PolicyAreaAuthorization("ModeratorArea", "moderator"));
});

var app = builder.Build();

// Initialize Db
using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    var userManager =
        (UserManager<User>)services.GetRequiredService(typeof(UserManager<User>));
    var roleManager = (RoleManager<IdentityRole>)services
        .GetRequiredService(typeof(RoleManager<IdentityRole>));
    DataManager dataManager = (DataManager)services.GetRequiredService(typeof(DataManager));
    await DbInitializer.InitializeAsync(userManager, roleManager, dataManager, app.Configuration);
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
