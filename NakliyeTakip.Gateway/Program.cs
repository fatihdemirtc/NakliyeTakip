using NakliyeTakip.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthenticationAndAuthorizationExt(builder.Configuration);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.MapGet("/", () => "YARP (Gateway)");
app.Run();