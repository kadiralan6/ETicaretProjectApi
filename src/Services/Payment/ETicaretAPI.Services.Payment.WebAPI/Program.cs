using ETicaretAPI.Common.Infrastructure;
using ETicaretAPI.Services.Payment.Application;
using ETicaretAPI.Services.Payment.Infrastructure;
using ETicaretAPI.Services.Payment.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// HttpContext accessor (required for ICurrentUserService / CurrentUserProvider)
builder.Services.AddHttpContextAccessor();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Persistence (DbContext + Repositories + UnitOfWork)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddPersistenceServices(builder.Configuration);

// Application (Services + AutoMapper + Cache + EventBus)
builder.Services.AddApplicationServices();

// Infrastructure (3rd-party integrations - reserved for future use)
builder.Services.AddPaymentServices();

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
