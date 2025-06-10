using MemoriaPiDataCore.SQL;
using MemoriaPiDataCore.LocalStorage;
using MemoriaPiDataCore.Models; // Hier befindet sich Ihre "User"-Klasse
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
// using MemoriaPiDataCore.Data; // Überprüfen, ob beide DbContexts nötig sind
using MemoriaPiWeb.Data;
using ApplicationDbContext = MemoriaPiDataCore.Data.ApplicationDbContext;

// Initialisierung deiner eigenen Dienste
DatabaseSetup databaseSetup = new DatabaseSetup();
databaseSetup.InitializeDatabase();
LocalSetup.SetupDataStructure();

var builder = WebApplication.CreateBuilder(args);

// --- Services zum Container hinzufügen ---

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext für Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity hinzufügen und konfigurieren
// HIER "User" anstelle von "ApplicationUser" verwenden
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false) 
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

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
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();