using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EPGroup30.Data;
using EPGroup30.Areas.Identity.Data;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Microsoft.Extensions.DependencyInjection;
using EPGroup30.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("EPGroup30ContextConnection") ?? throw new InvalidOperationException("Connection string 'EPGroup30ContextConnection' not found.");

builder.Services.AddDbContext<EPGroup30Context>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<EPGroup30User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<EPGroup30Context>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
AWSSDKHandler.RegisterXRayForAllServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseXRay("E&P Web App"); // Initialize X-Ray tracing

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<EPGroup30Context>();
        // Seed your initial data here if needed
        // For example:
        // SeedData.Initialize(context); // Replace with your seeding method
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
