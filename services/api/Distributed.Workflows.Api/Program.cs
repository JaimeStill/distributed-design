using Distributed.Core.Extensions;
using Distributed.Workflows.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDbContext<WorkflowsContext>(
    builder.Configuration,
    "App"
);

builder.ConfigureGatewayOptions();
builder.ConfigureCorsService();
builder.ConfigureApiServices();
builder.ConfigureSignalRServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
