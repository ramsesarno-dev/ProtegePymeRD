using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProtegePymeRD.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de SQL Server
var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddDatabaseDeveloperPageExceptionFilter();

// Configuraci�n de Identity y roles
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

    var correoAdministrador =
    builder.Configuration["AdminUser:Email"];

    var claveAdministrador =
        builder.Configuration["AdminUser:Password"];

    if (string.IsNullOrWhiteSpace(correoAdministrador) ||
        string.IsNullOrWhiteSpace(claveAdministrador))
    {
        throw new InvalidOperationException(
            "No se configuraron las credenciales del administrador.");
    }

    var usuarioAdministrador =
        await userManager.FindByEmailAsync(correoAdministrador);

    if (usuarioAdministrador == null)
    {
        usuarioAdministrador = new IdentityUser
        {
            UserName = correoAdministrador,
            Email = correoAdministrador,
            EmailConfirmed = true
        };

        var resultadoCreacion =
            await userManager.CreateAsync(
                usuarioAdministrador,
                claveAdministrador);

        if (!resultadoCreacion.Succeeded)
        {
            var errores = string.Join(
                ", ",
                resultadoCreacion.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"No se pudo crear el administrador: {errores}");
        }
    }

    if (!await userManager.IsInRoleAsync(
            usuarioAdministrador,
            "Administrador"))
    {
        var resultadoRol =
            await userManager.AddToRoleAsync(
                usuarioAdministrador,
                "Administrador");

        if (!resultadoRol.Succeeded)
        {
            var errores = string.Join(
                ", ",
                resultadoRol.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"No se pudo asignar el rol: {errores}");
        }
    }

}

    // Configuraci�n del pipeline HTTP
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