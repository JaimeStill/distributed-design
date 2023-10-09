using Distributed.Core.Extensions;
using Workflows.Data;
using Workflows.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDbContext<WorkflowsContext>(
    builder.Configuration,
    "Node"
);

builder.ConfigureGatewayOptions();
builder.ConfigureCorsService();
builder.ConfigureApiServices();
builder.ConfigureSignalRServices();

builder.Services.AddGatewayService();
builder.Services.AddAppServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseCors();

app.MapControllers();
app.MapHub<PackageEventHub>("/events/package");

app.Run();
