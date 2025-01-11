using PaymentGateway.Adapters.AcquiringBank;
using PaymentGateway.ApplicationCore;
using PaymentGateway.Persistence.InMemory;
using PaymentGateway.WebApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationCore();
builder.Services.AddInMemoryPersistence();
builder.Services.AddAcquiringBankAdapter(builder.Configuration);

builder.Services.AddScoped<IOperationContext, OperationContext>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(options => options.OperationFilter<MerchantIdHeaderParameter>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();