using Glosy.Constants;
using Glosy.Interfaces;
using Glosy.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAudioProcessingService, AudioProcessingService>();
builder.Services.AddLocalization();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = AudioConstants.FileSizeLimit;
});

var app = builder.Build();

var basePath = builder.Configuration.GetValue<string>("Config:BasePath");

if (!string.IsNullOrEmpty(basePath))
{
    app.UsePathBase(basePath);
}

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
    pattern: "{controller=Home}/{action=Index}/{id?}");


var supportedCultures = new[] { "en-US", "pl" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.Run();
