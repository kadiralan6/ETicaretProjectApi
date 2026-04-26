using ETicaretAPI.Common.Infrastructure;
using ETicaretAPI.Services.Basket.Application;
using ETicaretAPI.Services.Basket.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Persistence (DbContext + Repositories + UnitOfWork)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services + AutoMapper + IRestApiService)
builder.Services.AddApplicationServices();

// Common Infrastructure (Redis, RabbitMQ, HttpClient)
builder.Services.AddCommonInfrastructure(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
