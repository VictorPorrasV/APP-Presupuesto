using APP_Presupuesto.Interfaces.Repositorios;
using APP_Presupuesto.Interfaces.Servicios;
using APP_Presupuesto.Repositorio;
using APP_Presupuesto.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRepositorioTipoCuenta, RepositorioTipoCuenta>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuario>();
builder.Services.AddTransient<IRepositorioCuentas, RepositorioCuentas>();
builder.Services.AddTransient<IRepositorioCategorias, RepositorioCategorias>();
builder.Services.AddTransient<IRepositorioTransacciones, RepositorioTransacciones>();
builder.Services.AddTransient<IServicioReportes,ServicioReportes>();

builder.Services.AddHttpContextAccessor();
//agregar auto mapper
builder.Services.AddAutoMapper(typeof(Program));
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transacciones}/{action=Index}/{id?}");

app.Run();
