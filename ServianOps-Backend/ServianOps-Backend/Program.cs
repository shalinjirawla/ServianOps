using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServianOps_Backend.Application.Interfaces;
using ServianOps_Backend.Application.Services;
using ServianOps_Backend.Core.Interfaces;
using ServianOps_Backend.Core.Interfaces.Repositories;
using ServianOps_Backend.EntityFramework.Contexts;
using ServianOps_Backend.EntityFramework.Repositories;
using ServianOps_Backend.Infrastructure.Authentication;
using ServianOps_Backend.Infrastructure.Multitenancy;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure FluentValidation Auto Validation
builder.Services.AddFluentValidationAutoValidation();
// Register all validators from the Application assembly
builder.Services.AddValidatorsFromAssemblyContaining<ServianOps_Backend.Application.Validations.LoginDtoValidator>();
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ServianOps_Backend.Application.Common.DTOs.PagedRequestDto).Assembly));
builder.Services.AddEndpointsApiExplorer();

//Enable CORS

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "ServianOps API", Version = "v1" });
    
    // Use Action as the operationId to allow NSwag to generate clean method names.
    c.CustomOperationIds(e => e.ActionDescriptor.RouteValues["action"]);
    
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {token}"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configure HttpContextAccessor for CurrentTenant/CurrentUser
builder.Services.AddHttpContextAccessor();

// 3. Configure JWT Settings
var jwtSection = builder.Configuration.GetSection(JwtSettings.SectionName);
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>();

// 4. Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

// 5. Dependency Injection
builder.Services.AddScoped<ICurrentTenant, CurrentTenantService>();
builder.Services.AddScoped<ICurrentUser, CurrentUserService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));






// Services
builder.Services.AddScoped<ServianOps_Backend.Application.PlanModule.Plan.IPlanService, ServianOps_Backend.Application.PlanModule.Plan.PlanService>();
builder.Services.AddScoped<ServianOps_Backend.Application.TenantModule.Tenant.ITenantService, ServianOps_Backend.Application.TenantModule.Tenant.TenantService>();
builder.Services.AddScoped<ServianOps_Backend.Application.UserModule.User.IUserService, ServianOps_Backend.Application.UserModule.User.UserService>();
builder.Services.AddScoped<ServianOps_Backend.Application.RoleModule.Role.IRoleService, ServianOps_Backend.Application.RoleModule.Role.RoleService>();

// Crm Services
builder.Services.AddScoped<ServianOps_Backend.Application.CustomerTypeModule.CustomerType.ICustomerTypeService, ServianOps_Backend.Application.CustomerTypeModule.CustomerType.CustomerTypeService>();
builder.Services.AddScoped<ServianOps_Backend.Application.CustomerModule.Customer.ICustomerService, ServianOps_Backend.Application.CustomerModule.Customer.CustomerService>();
builder.Services.AddScoped<ServianOps_Backend.Application.SiteModule.Site.ISiteService, ServianOps_Backend.Application.SiteModule.Site.SiteService>();

// Jobs Services
builder.Services.AddScoped<ServianOps_Backend.Application.TradeModule.Trade.ITradeService, ServianOps_Backend.Application.TradeModule.Trade.TradeService>();
builder.Services.AddScoped<ServianOps_Backend.Application.JobModule.Job.IJobService, ServianOps_Backend.Application.JobModule.Job.JobService>();



var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    await ServianOps_Backend.EntityFramework.Seeders.DbSeeder.SeedAsync(context);
}

// 1. Global Exception Handling first
app.UseMiddleware<ServianOps_Backend.Middlewares.GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ServianOps_Backend.Middlewares.TenantResolverMiddleware>();

app.MapControllers();

app.Run();
