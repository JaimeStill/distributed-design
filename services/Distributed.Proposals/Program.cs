using Distributed.Core.Extensions;
using Distributed.Proposals.Data;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureCorsService();
builder.ConfigureApiServices<ProposalsContext>("App");
builder.ConfigureSignalRServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
