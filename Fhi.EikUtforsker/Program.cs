using Fhi.EikUtforsker;
using Fhi.EikUtforsker.Controllers;
using Fhi.EikUtforsker.Tjenester.Analyse;
using Fhi.EikUtforsker.Tjenester.Dekryptering;
using Fhi.EikUtforsker.Tjenester.Meldingsformater;
using Fhi.EikUtforsker.Tjenester.WebDav;
using Serilog;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.Configure<EikUtforskerOptions>(builder.Configuration.GetSection(EikUtforskerOptions.EikUtforsker));

builder.Services.AddScoped<Analysetjeneste>();
builder.Services.AddScoped<Dekrypteringstjeneste>();
builder.Services.AddScoped<Meldingsformater>();
builder.Services.AddScoped<WebDavTjeneste>();
builder.Services.AddScoped<NyesteElementerTjeneste>();


builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen();

LesBuildDate();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();


void LesBuildDate()
{
    var assembly = Assembly.GetExecutingAssembly();
    var path = $"Fhi.EikUtforsker.Resources.BuildDate.txt";
    using var stream = assembly.GetManifestResourceStream(path) ?? throw new Exception($"Fant ikke embeded resource {path}");
    using var reader = new StreamReader(stream);
    BuildDateController.BuildDate = reader.ReadToEnd().Trim();
}
