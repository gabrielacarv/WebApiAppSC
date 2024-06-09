using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApiAppSS.Data;
using WebApiAppSS.Service.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adicione a configuração do token para a seção "token"
var appSettings = builder.Configuration.GetSection("token");
builder.Services.Configure<TokenSettings>(appSettings);

builder.Services.AddScoped<AuthService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(builder =>
    builder.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
);

app.Use(next => context =>
{
    context.Request.EnableBuffering();
    return next(context);
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();