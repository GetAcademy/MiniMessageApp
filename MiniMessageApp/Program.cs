using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    // Slår på autentisering i API-et.
    // API-et forventer nå at klienten sender et JWT-token i headeren:
    // Authorization: Bearer <token>

    .AddJwtBearer(options =>
        // Konfigurerer hvordan JWT-tokenet skal valideres.
        // Når en request kommer inn skjer dette:
        // 1. API-et leser Authorization-headeren
        // 2. JWT-tokenet valideres
        // 3. Claims fra tokenet gjøres tilgjengelige i requesten
    {
        options.Authority = "https://getacademy.eu.auth0.com";
        // URL til Auth0-tenantet ditt.
        // API-et bruker denne til å hente informasjon om hvem som utsteder tokenet
        // og hvordan signaturen skal verifiseres.

        options.Audience = "https://get-mini-messages";
        // Audience sier hvilket API tokenet er ment for.
        // Hvis tokenet ikke er laget for dette API-et blir requesten avvist.
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("read:messages", policy =>
        // Lager en autorisasjonspolicy med navnet "read:messages".
        // Endpoints kan kreve denne policyen for å få tilgang.

        policy.RequireClaim("permissions", "read:messages"));
    // Krever at JWT-tokenet inneholder denne claimen:
    // "permissions": ["read:messages"]
    // Hvis tokenet mangler denne tillatelsen returnerer API-et 403 Forbidden.
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

var messages = new List<string>
{
    "Hello",
    "Secure world"
};

app.MapGet("/public", () => "Public endpoint");

app.MapGet("/messages", () => {
        //var user = context.User.Identity?.Name ?? "Unknown user";
        return messages;
    })
    .RequireAuthorization("read:messages");

app.Run();