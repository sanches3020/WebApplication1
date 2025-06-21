using Microsoft.EntityFrameworkCore;
using MoodMapper.Data;
using MoodMapper.Services.Json;
using MoodMapper.Configurations;

var builder = WebApplication.CreateBuilder(args);

// База данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Кэш, сессии, контекст
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(50);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Razor и контроллеры
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// JSON-сервисы и настройки
builder.Services.AddScoped<JsonExporter>();
builder.Services.AddScoped<JsonImporter>();
builder.Services.AddScoped<EmotionService>();
builder.Services.Configure<JsonSettings>(builder.Configuration.GetSection("JsonSettings"));

// CORS (если нужно)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.WithOrigins("https://moodmapper.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("DefaultPolicy");
app.UseSession();
app.UseAuthorization();

// Роутинг
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapRazorPages();

app.Run();
