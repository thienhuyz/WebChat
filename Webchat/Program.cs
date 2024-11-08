using Chat.Data;
using Chat.Data.Entities;
using Chat.Hubs;
using Chat.IdentityServer;
using Chat.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Định nghĩa mvcBuilder bằng cách gán kết quả của AddRazorPages
var mvcBuilder = builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddAreaFolderRouteModelConvention("Identity", "/Account/", model =>
    {
        foreach (var selector in model.Selectors)
        {
            var attributeRouteModel = selector.AttributeRouteModel;
            attributeRouteModel.Order = -1;
            attributeRouteModel.Template = attributeRouteModel.Template.Remove(0, "Identity".Length);
        }
    });
});

// Thêm SignalR
builder.Services.AddSignalR();

// Kiểm tra môi trường và thêm Razor Runtime Compilation nếu là môi trường Development
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (environment == Environments.Development)
{
    mvcBuilder.AddRazorRuntimeCompilation();
}


// Add Authentication using Bearer token
builder.Services.AddAuthentication()
    .AddLocalApi("Bearer", option =>
    {
        option.ExpectedScope = "api.WebApp";
    });

// Add Authorization with Bearer policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Bearer", policy =>
    {
        policy.AddAuthenticationSchemes("Bearer");
        policy.RequireAuthenticatedUser();
    });
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add Controllers for API
builder.Services.AddControllers();

// Add services DbContext
builder.Services.AddDbContext<ChatDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Sử dụng AddIdentity (nếu cần roles)
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ChatDbContext>()
    .AddDefaultTokenProviders();

// Thêm dịch vụ gửi email (IEmailSender và EmailSenderService)
builder.Services.AddTransient<IEmailSender, EmailSenderService>();

// Thêm IdentityServer và các cấu hình cho API, IdentityResources, Clients, Scopes
builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
})
.AddInMemoryApiResources(Config.Apis)
.AddInMemoryClients(Config.Clients)
.AddInMemoryIdentityResources(Config.Ids)
.AddInMemoryApiScopes(Config.ApiScopes)
.AddAspNetIdentity<User>()
.AddDeveloperSigningCredential();

// Thêm Swagger với cấu hình bảo mật OAuth2 (Bearer Token)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApp API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration["AuthorityUrl"] + "/connect/authorize"),
                Scopes = new Dictionary<string, string>
                {
                    { "api.WebApp", "WebApp API" }
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string> { "api.WebApp" }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

// Kích hoạt Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.OAuthClientId("swagger");
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp API V1");
});

// Đảm bảo các API được ánh xạ
app.MapControllers();
app.MapRazorPages();

// Thêm định tuyến mặc định cho Controller
app.MapDefaultControllerRoute();

// Map SignalR Hub
app.MapHub<ChatHub>("/chatHub");

app.Run();
