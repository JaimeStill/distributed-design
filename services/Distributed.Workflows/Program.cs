using Distributed.Core.Extensions;
using Distributed.Workflows.Data;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureCorsService();
builder.ConfigureApiServices<WorkflowsContext>("App");
builder.ConfigureSignalRServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
