using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TareasMVC;
using Microsoft.AspNetCore.Mvc.Razor;
using TareasMVC.Servicios;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser() // Requiere que el usuario est� autenticado
    .Build(); // Crea la pol�tica de autorizaci�n

// Add services to the container.
builder.Services.AddControllersWithViews(opciones =>
{
    opciones.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados)); // Aplica la pol�tica de autorizaci�n a todos los controladores y acciones
}).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
.AddDataAnnotationsLocalization(opciones =>
{
    opciones.DataAnnotationLocalizerProvider = (_, factoria) => 
        factoria.Create(typeof(RecursoCompartido));
});

builder.Services.AddDbContext<ApplicationDbContext>(opciones => 
    opciones.UseSqlServer("name=DefaultConnection"));
builder.Services.AddAuthentication().AddMicrosoftAccount(opciones =>
{
    opciones.ClientId = builder.Configuration["MicrosoftClientId"];
    opciones.ClientSecret = builder.Configuration["MicrosoftSecretId"];
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opciones =>
{
    opciones.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders(); // Configuraci�n de Identity para usar Entity Framework y el esquema por defecto de Identity

builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
    opciones =>
    {
        opciones.LoginPath = "/usuarios/login";
        opciones.LogoutPath = "/usuarios/login";
        opciones.AccessDeniedPath = "/usuarios/login";
    }); // Configuraci�n de las rutas de autenticaci�n (Para que sean personalizadas y no las que vienen por defecto al usar Identity)

builder.Services.AddLocalization(opciones =>
{
    opciones.ResourcesPath = "Recursos"; // Ruta donde se encuentran los archivos de recursos para la localizaci�n
}); // Configuraci�n de localizaci�n para la aplicaci�n


builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>(); // Registro del servicio para obtener el ID del usuario autenticado

builder.Services.AddAutoMapper(config => config.AddMaps(typeof(Program).Assembly)); // Registro de AutoMapper para mapear entidades a DTOs y viceversa  

var app = builder.Build();


app.UseRequestLocalization(opciones =>
{
    opciones.DefaultRequestCulture = new RequestCulture("es");
    opciones.SupportedUICultures = Constantes.CulturasUISoportadas
        .Select(cultura => new CultureInfo(cultura.Value)).ToList();
}); // Configuraci�n de las culturas soportadas para la UI

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // Permite obtener la data del usuario autenticado.

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
