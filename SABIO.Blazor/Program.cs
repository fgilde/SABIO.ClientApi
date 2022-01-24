using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SABIO.Blazor.Data;
using SABIO.Blazor.Extensions;
using SABIO.ClientApi.Core;
using SABIO.ClientApi.Core.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton(provider => new SabioClient("https://maestro-fg-knowledge.nomad-internal.sabio.de/sabio-web/services", "qa-test"))
    .AddDerivedFrom<SabioApiBase>((provider, type) => provider.GetService<SabioClient>().Api(type));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
