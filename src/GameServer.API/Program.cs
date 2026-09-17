using GameServer.API.Middleware;
using GameServer.Application;
using GameServer.Domain;
using GameServer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<HttpGameContext>();
builder.Services.AddScoped<IGameContext>(sp => sp.GetRequiredService<HttpGameContext>());

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("role", "admin"));
});

var app = builder.Build();

// Domain exception → structured error response (must be first).
app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (DomainException ex)
    {
        ctx.Response.StatusCode = ex.StatusCode;
        await ctx.Response.WriteAsJsonAsync(new { error = ex.Code, message = ex.Message });
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<GameContextMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
