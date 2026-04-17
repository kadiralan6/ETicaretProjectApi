using ETicaretAPI.Common.Infrastructure;
using ETicaretAPI.Services.Catalog.Application;
using ETicaretAPI.Services.Catalog.Persistence;

var builder = WebApplication.CreateBuilder(args);



// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Persistence (DbContext + Repositories + UnitOfWork)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services: IBrandService, ICategoryService, IProductService + AutoMapper)
builder.Services.AddApplicationServices();

// Common Infrastructure (Redis, RabbitMQ, HttpClient)
builder.Services.AddCommonInfrastructure(builder.Configuration);
builder.Services.AddControllers();

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

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
