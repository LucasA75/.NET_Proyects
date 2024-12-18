using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TareasMVC;
using Microsoft.AspNetCore.Mvc.Razor;


var builder = WebApplication.CreateBuilder(args);

var politicasUsuariosAutenticados = new AuthorizationPolicyBuilder()
	.RequireAuthenticatedUser()
	.Build();

// Add services to the container.
builder.Services.AddControllersWithViews(opciones =>
{
	opciones.Filters.Add(new AuthorizeFilter(politicasUsuariosAutenticados));

}).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
.AddDataAnnotationsLocalization(opciones => 
	opciones.DataAnnotationLocalizerProvider = (_,factoria) => 
	factoria.Create(typeof(RecursoCompartido))
);

builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));
builder.Services.AddAuthentication().AddMicrosoftAccount(opciones =>
{
	opciones.ClientId = builder.Configuration["MicrosoftClientId"];
	opciones.ClientSecret = builder.Configuration["MicrosoftSecretId"];
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opciones =>
{
	opciones.SignIn.RequireConfirmedAccount = false;

}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, opciones =>
{
	opciones.LoginPath = "/usuarios/login";
	opciones.AccessDeniedPath = "/usuarios/login";
});




builder.Services.AddLocalization(opciones =>
{
	opciones.ResourcesPath = "Resources";
});

var app = builder.Build();


var culturasUISoportadas = new[] { "es", "en"};

app.UseRequestLocalization(opciones =>
{
	opciones.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("es");
	opciones.SupportedUICultures = culturasUISoportadas.Select(cultura => new CultureInfo(cultura)).ToList(); 
});

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
