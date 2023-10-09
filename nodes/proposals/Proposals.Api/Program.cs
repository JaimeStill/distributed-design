using Distributed.Core.Extensions;
using Proposals.Api.Events;
using Proposals.Data;
using Proposals.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDbContext<ProposalsContext>(
    builder.Configuration,
    "Node"
);

builder.ConfigureGatewayOptions();
builder.ConfigureCorsService();
builder.ConfigureApiServices();
builder.ConfigureSignalRServices();

builder.Services.AddGatewayService();
builder.Services.AddAppServices();
builder.Services.RegisterEventListeners();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseJsonExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseCors();

app.MapControllers();
app.MapHub<ProposalEventHub>("/events/proposal");

app.Run();
