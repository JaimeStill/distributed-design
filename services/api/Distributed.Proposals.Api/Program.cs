using Distributed.Core.Extensions;
using Distributed.Proposals.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDbContext<ProposalsContext>(
    builder.Configuration,
    "App"
);

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
