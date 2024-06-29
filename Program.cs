using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
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
app.MapControllers();


// DEL
var usernames = new List<string>
{
    "Admin"
};

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

app.MapPost("/login", async (HttpContext context) => 
{
    var form = context.Request.Form;
    string username = form["username"]!;
    if (!usernames.Contains(username))
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

app.Map("/", (HttpContext context) =>
{
    string responseHTML = "";
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var username = context.User.FindFirst(ClaimTypes.Name)!.Value;
        responseHTML =
        $"""
        <p>Hello {username}</p>
        <br>
        <a href="/logout">Log out<a/>
        """;
    }
    else
    {
        responseHTML =
        """
        <p>Anonimous users don't have access to this page</p>
        <p>To get access try to <a href="/login">Log in<a/><p/>
        """;
    }

    return Results.Content(responseHTML, "text/html");
});

app.Run();
