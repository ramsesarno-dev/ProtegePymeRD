using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProtegePymeRD.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración de SQL Server
var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddDatabaseDeveloperPageExceptionFilter();

// Configuración de Identity y roles
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;

        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;

        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Crear roles y asignar el administrador
using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

    string[] roles =
    {
        "Administrador",
        "Analista"
    };

    foreach (string nombreRol in roles)
    {
        bool existeRol =
            await roleManager.RoleExistsAsync(nombreRol);

        if (!existeRol)
        {
            var resultadoRol =
                await roleManager.CreateAsync(
                    new IdentityRole(nombreRol));

            if (!resultadoRol.Succeeded)
            {
                string errores = string.Join(
                    ", ",
                    resultadoRol.Errors.Select(error =>
                        error.Description));

                throw new InvalidOperationException(
                    $"No se pudo crear el rol " +
                    $"{nombreRol}: {errores}");
            }
        }
    }

    const string correoAdministrador =
        "admin@protegepyme.com";

    var usuarioAdministrador =
        await userManager.FindByEmailAsync(
            correoAdministrador);

    if (usuarioAdministrador != null)
    {
        bool yaEsAdministrador =
            await userManager.IsInRoleAsync(
                usuarioAdministrador,
                "Administrador");

        if (!yaEsAdministrador)
        {
            var resultadoAsignacion =
                await userManager.AddToRoleAsync(
                    usuarioAdministrador,
                    "Administrador");

            if (!resultadoAsignacion.Succeeded)
            {
                string errores = string.Join(
                    ", ",
                    resultadoAsignacion.Errors.Select(error =>
                        error.Description));

                throw new InvalidOperationException(
                    "No se pudo asignar el rol " +
                    $"Administrador: {errores}");
            }
        }
    }
    else
    {
        Console.WriteLine(
            "No se encontró el usuario " +
            $"{correoAdministrador}. " +
            "El rol se asignará cuando el usuario exista.");
    }
}

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();