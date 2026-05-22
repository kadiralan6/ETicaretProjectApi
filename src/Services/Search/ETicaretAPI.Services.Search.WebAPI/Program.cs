using ETicaretAPI.Common.Application;
using ETicaretAPI.Common.Infrastructure;
using ETicaretAPI.Services.Search.Application;
using ETicaretAPI.Services.Search.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Search Application (SearchService + Event Handlers)
ETicaretAPI.Services.Search.Application.ApplicationServiceRegistration.AddApplicationServices(builder.Services);


builder.Services.AddSearchInfrastructure(builder.Configuration);

// Common Infrastructure (Redis, RabbitMQ connection, HttpClient)
builder.Services.AddCommonInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();


app.Run();
