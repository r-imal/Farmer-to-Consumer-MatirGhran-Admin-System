using FarmerToConsumer.Data;
using FarmerToConsumer.Repos;
using FarmerToConsumer.Shared;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<FtcDbContext>(opt=>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("FtcDbContext"),
        sql => sql.EnableRetryOnFailure(1, TimeSpan.FromSeconds(1), null)));
builder.Services.AddScoped<UserInfoRepo>();
builder.Services.AddScoped<OrderRepo>();
builder.Services.AddScoped<AdminDashboardRepo>();
builder.Services.AddScoped<AgentPaymentRepo>();
builder.Services.AddScoped<AgentRepo>();
builder.Services.AddScoped<AgentAssignmentRepo>();
builder.Services.AddScoped<FarmerRepo>();
builder.Services.AddScoped<ProductRepo>();
builder.Services.AddScoped<ProductStockRepo>();
builder.Services.AddScoped<CustomerShopRepo>();
builder.Services.AddScoped<CurrentUserHelper>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication("FtcAuth")
    .AddCookie("FtcAuth", opt =>
    {
        opt.AccessDeniedPath = "/Account/Denied";
        opt.LoginPath = "/Account/Login";
        opt.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/AdminDashboard/Index");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
}

app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
