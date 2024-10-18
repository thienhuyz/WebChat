using Chat.Data;
using Chat.Data.Entities;
using Chat.IdentityServer;
using Chat.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

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

app.MapDefaultControllerRoute();
app.MapRazorPages();
app.Run();
