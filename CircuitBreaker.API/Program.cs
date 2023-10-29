using CircuitBreaker.Core.Interfaces.Handlers;
using CircuitBreaker.Core.Interfaces.States;
using CircuitBreaker.Core.States;
using CircuitBreaker.Service;
using CircuitBreaker.Service.Handlers;
using CircuitBreaker.Service.Queues;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IClosedState, ClosedState>();
builder.Services.AddScoped<IHalfOpenState, HalfOpenState>();
//builder.Services.AddScoped<IOpenState, OpenState>();
builder.Services.AddScoped<IForgetPasswordHandler, ForgetPasswordHandler>();
builder.Services.AddScoped<IEmailHandler, EmailHandler>();
builder.Services.AddScoped<ICheckoutHandler, CheckoutHandler>();
builder.Services.AddScoped<IPaymentHandler, PaymentHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
