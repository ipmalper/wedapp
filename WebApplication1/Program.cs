using WebApplication1.Helpers;
using WebApplication1.Repositorio.Implementaciones;
using WebApplication1.Repositorio.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // tiempo opcional
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("https://sistema-n-front-prod-erhecchba3fza6cc.canadacentral-01.azurewebsites.net") // puerto de Angular
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "tu_cadena_conexion_redis";
});
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseMiddleware<TokenValidationMiddleware>(); 
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
