using auth_abac.Databases;
using Casbin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
string? connectionString = builder.Configuration.GetConnectionString("SQLiteConnection");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        });

var app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/login", (string? message, HttpContext context) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        return Results.Redirect("/");
    }

    string formHTML =
        $"""
        <p>{message}</p>
        <form method='post'>
            <p>
                <label>Username</label><br />
                <input name='username' />
            </p>
            <input type='submit' value='Login' />
        </form>
        """;

    return Results.Content(formHTML, "text/html");
});

app.MapPost("/login", async (HttpContext context, DataContext db) => 
{
    var form = context.Request.Form;
    string username = form["username"]!;
    if (!db.Users.Any(u => u.Name == username))
    {
        string message = $"No user with username = {username}";
        return Results.Redirect($"/login?message={message}");
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, username)
    };
    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
    return Results.Redirect("/");
});

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.Map("/", (HttpContext context, DataContext db) =>
{
    string responseHTML = "";
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var username = context.User.FindFirst(ClaimTypes.Name)!.Value;

        var e = new Enforcer("Authorization/model.conf");

        var sub = db.Users
            .Where(u => u.Name == username)
            .Include(u => u.Position)
            .Include(u => u.Department)
            .Select(u => new
            {
                Position = u.Position.Name,
                Department = u.Department.Name,
                EnrollmentDate = u.EnrollmentDate
            })
            .FirstOrDefault();

        var accessProducts = db.Products
            .Include(p => p.Department)
            .Select(p => new
            {
                Name = p.Name,
                Department = p.Department.Name,
                Timestamp = p.Timestamp
            })
            .ToList()
            .Where(p => e.Enforce(sub, p))
            .ToList();

        var productsList = accessProducts
            .Select(p => $"<li>{p.Department} {p.Timestamp} {p.Name}</li>")
            .Aggregate("", (f, n) => $"{f}{n}");

        var productsHTML =
            $"""
            <p>You have access to these products:</p>
            <ul>
                {productsList}
            </ul>
            """;

        responseHTML =
            $"""
            <p>Hello {username}</p>
            <br>
            <p>Info:</p>
            <ul>
                <li>Position: {sub.Position}</li>
                <li>Department: {sub.Department ?? "-"}</li>
                <li>EnrollmentDate : {sub.EnrollmentDate}</li>
            </ul>
            <br>
            {productsHTML}
            <br>
            <a href="/logout">Log out<a/>
            """;
    }
    else
    {
        responseHTML =
        """
        <p>Anonymous users don't have access to this page</p>
        <p>To get access try to <a href="/login">Log in<a/><p/>
        """;
    }

    return Results.Content(responseHTML, "text/html");
});

app.Run();
