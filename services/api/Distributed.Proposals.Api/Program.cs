using Distributed.Core.Extensions;
using Distributed.Proposals.Data;
using Distributed.Proposals.Logic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDbContext<ProposalsContext>(
    builder.Configuration,
    "App"
);

builder.ConfigureGatewayOptions();
builder.ConfigureCorsService();
builder.ConfigureApiServices();
builder.ConfigureSignalRServices();

builder.Services.AddAppServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseCors();

app.MapControllers();
app.MapHub<ProposalEventHub>("/events/proposal");

app.Run();
