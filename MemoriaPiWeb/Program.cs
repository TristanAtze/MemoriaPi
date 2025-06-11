using MemoriaPiDataCore.Data;
using MemoriaPiDataCore.LocalStorage;
using MemoriaPiDataCore.Models;
using MemoriaPiDataCore.SQL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MemoriaPiWeb.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Konfiguration lesen ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Eigene Dienste mit der Konfiguration initialisieren ---
if (!string.IsNullOrEmpty(connectionString))
{
    DatabaseSetup databaseSetup = new DatabaseSetup(connectionString);
    databaseSetup.InitializeDatabase();
    LocalSetup.SetupDataStructure();
}
else
{
    throw new InvalidOperationException("DefaultConnection string not found in configuration.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
    })
    .AddGitHub(githubOptions =>
    {
        githubOptions.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
        githubOptions.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });


builder.Services.AddRazorPages();
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// --- HTTP Request Pipeline konfigurieren ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();