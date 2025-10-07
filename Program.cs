using DSD603_BikeShopDB.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var CORSAllowSpecificOrigins = "_CORSAllowed";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORSAllowSpecificOrigins,
    policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://www.contoso.com");
    });
});

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    options.SignIn.RequireConfirmedEmail = false;
    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.AddAuthorization(options =>
{
    // Simple role-based policy
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("Manager"));
    options.AddPolicy("RequireStaffRole", policy => policy.RequireRole("Staff"));

    // Permission-based claims
    options.AddPolicy("CanEditStock", policy => policy.RequireClaim("Permission", "Edit Stock"));
    options.AddPolicy("CanDeleteStock", policy => policy.RequireClaim("Permission", "Delete Stock"));

    // Age-based policy
    options.AddPolicy("Over18", policy => policy.RequireAssertion(context =>
    {
        var ageClaim = context.User.FindFirst("DateOfBirth");
        if(ageClaim != null && DateTime.TryParse(ageClaim.Value, out DateTime dateOfBirth))
        {
            int age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Today.AddYears(-age)) age--;
            return age >= 18;
        }

        return false;
    }));

    //View policy with claim and joining date over 6 months
    options.AddPolicy("ViewRolesPolicy", policyBuilder => policyBuilder.RequireAssertion(context =>
    {
        var joiningDateClaim = context.User.FindFirst(c => c.Type == "Joining Date")?.Value;
        var joiningDate = Convert.ToDateTime(joiningDateClaim);
        return context.User.HasClaim("Permission", "View Roles") && joiningDate > DateTime.MinValue && joiningDate < DateTime.Now.AddMonths(-6);
    }));

    options.AddPolicy("ViewClaimsPolicy", policyBuilder => policyBuilder.RequireAssertion(context =>
    {
        var joiningDateClaim = context.User.FindFirst(c => c.Type == "Joining Date")?.Value;
        var joiningDate = Convert.ToDateTime(joiningDateClaim);
        return context.User.HasClaim("Permission", "View Claims") && joiningDate > DateTime.MinValue && joiningDate < DateTime.Now.AddMonths(-6);
    }));
});

builder.Services.AddControllersWithViews();
builder.Services.AddOpenApiDocument();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    // Add OpenAPI 3.0 document serving middleware
    // Available at: http://localhost:<port>/swagger/v1/swagger.json
    app.UseOpenApi();

    // Add web UIs to interact with the document
    // Available at: http://localhost:<port>/swagger
    app.UseSwaggerUi(); // UseSwaggerUI Protected by if (env.IsDevelopment())
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(CORSAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();

public partial class Program { }