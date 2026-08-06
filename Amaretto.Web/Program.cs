using Amaretto.Application;
using Amaretto.Application.Profiles;
using Amaretto.Application.Services;
using Amaretto.Application.Services.Implementations;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Data;
using Amaretto.Infraestructure.Repositories;
using Amaretto.Infraestructure.Repository.Implementations;
using Amaretto.Infraestructure.Repository.Interfaces;
using Amaretto.Web.Middleware;
using Amaretto.Web.Scheduling;
using Libreria.Application.Config;
using Libreria.Application.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Mapeo de la clase AppConfig para leer appsettings.json
builder.Services.Configure<AppConfig>(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//***************************
//Configurar D.I.
//Repository
builder.Services.AddTransient<IRepositoryCombo, RepositoryCombo>();
builder.Services.AddTransient<IRepositoryProducto, RepositoryProducto>();
builder.Services.AddTransient<IRepositoryCategoria, RepositoryCategoria>();
builder.Services.AddTransient<IRepositoryMenuProducto, RepositoryMenuProducto>();
builder.Services.AddTransient<IRepositoryMenuCombo, RepositoryMenuCombo>();
builder.Services.AddTransient<IRepositoryCocinaOrden, RepositoryCocinaOrden>();
builder.Services.AddTransient<IRepositoryEstacion, RepositoryEstacion>();
builder.Services.AddTransient<IRepositoryUsuario, RepositoryUsuario>();
builder.Services.AddTransient<IRepositoryIngrediente, RepositoryIngrediente>();
builder.Services.AddScoped<IRepositoryPedidoDetalle, RepositoryPedidoDetalle>();
builder.Services.AddScoped<IRepositoryTareaMenuVencido, RepositoryTareaMenuVencido>();
builder.Services.AddScoped<IRepositoryPedido, RepositoryPedido>();

//Services
builder.Services.AddTransient<IServiceCombo, ServiceCombo>();
builder.Services.AddTransient<IServiceProducto, ServiceProducto>();
builder.Services.AddTransient<IServiceCategoria, ServiceCategoria>();
builder.Services.AddTransient<IServiceMenuProducto, ServiceMenuProducto>();
builder.Services.AddTransient<IServiceMenuCombo, ServiceMenuCombo>();
builder.Services.AddTransient<IServiceCocinaOrden, ServiceCocinaOrden>();
builder.Services.AddTransient<IServiceEstacion, ServiceEstacion>();
builder.Services.AddTransient<IServiceUsuario, UsuarioService>();
builder.Services.AddTransient<IServiceIngrediente, ServiceIngrediente>();
builder.Services.AddScoped<IServicePedidoDetalle, ServicePedidoDetalle>();
builder.Services.AddScoped<IServicioDesactivacionMenus, ServicioDesactivacionMenus>();
builder.Services.AddScoped<IServiceCarrito, ServiceCarrito>();
builder.Services.AddScoped<IServiceUsuarioActual, ServiceUsuarioActualSimulado>();
builder.Services.AddScoped<IServicePedido, ServicePedido>();

// Estado en memoria para mostrar el resultado en el panel /TareaProgramada.
// Debe ser Singleton: tiene que sobrevivir entre las distintas ejecuciones del BackgroundService
// y ser el mismo objeto que lee el controlador cuando alguien visita la página.
builder.Services.AddSingleton<ITareaProgramadaEstado, TareaProgramadaEstado>();

// El "programador": se registra como Hosted Service para que arranque junto con la app.
builder.Services.AddHostedService<DesactivacionMenusBackgroundService>();


//Seguridad
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Login/Index";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.AccessDeniedPath = "/Login/Forbidden";
    });
builder.Services.AddControllersWithViews(options => {
    options.Filters.Add(
        new ResponseCacheAttribute
        {
            NoStore = true,
            Location = ResponseCacheLocation.None,
        });
});

//Configurar Automapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<ComboProfile>();
    config.AddProfile<ProductoProfile>();
    config.AddProfile<CategoriaProfile>();
    config.AddProfile<MenuProductoProfile>();
    config.AddProfile<MenuDetalleProductoProfile>();
    config.AddProfile<MenuComboProfile>();
    config.AddProfile<CocinaOrdenProfile>();
    config.AddProfile<IngredienteProfile>();
    config.AddProfile<ProductoIngredienteProfile>();
    config.AddProfile<EstacionProfile>();
    config.AddProfile<UsuarioProfile>();
    config.AddProfile<PedidoDetalleProfile>();
});
// Configuar Conexión a la Base de Datos SQL
builder.Services.AddDbContext<AmarettoContext>(options =>
{
    // it read appsettings.json file

    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerDataBase"));
    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});
//Configuración Serilog
// Logger. P.E. Verbose = muestra SQl Statement
var logger = new LoggerConfiguration()
                    // Limitar la información de depuración
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                    .Enrich.FromLogContext()
                    // Log LogEventLevel.Verbose muestra mucha información, pero no es necesaria solo para el proceso de depuración
                    .WriteTo.Console(LogEventLevel.Information)
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(@"Logs\Info-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.File(@"Logs\Debug-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.File(@"Logs\Warning-.log", shared: true, encoding: System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File(@"Logs\Error-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.File(@"Logs\Fatal-.log", shared: true, encoding: Encoding.ASCII, rollingInterval: RollingInterval.Day))
                    .CreateLogger();

builder.Host.UseSerilog(logger);
//***************************
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Error control Middleware
    app.UseMiddleware<ErrorHandlingMiddleware>();
}
//Activar soporte a la solicitud de registro con SERILOG
app.UseSerilogRequestLogging();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// Activar Antiforgery 
app.UseAntiforgery();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


