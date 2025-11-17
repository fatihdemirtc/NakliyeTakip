using NakliyeTakip.Location.API;
using NakliyeTakip.Location.API.Features.Locations;
using NakliyeTakip.Location.API.Options;
using NakliyeTakip.Location.API.Repositories;
using NakliyeTakip.Shared.Extensions;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOptionsExt();
builder.Services.AddDatabaseServiceExt();
builder.Services.AddCommonServiceExt(typeof(LocationAssembly));
builder.Services.AddVersioningExt();
builder.Services.AddAuthenticationAndAuthorizationExt(builder.Configuration);

var app = builder.Build();
app.AddLocationGroupEndpointExt(app.AddVersionSetExt());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.Run();



