using Distributed.Core.Extensions;
using Proposals.Data;
using Proposals.Services;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(builder.Configuration.GetConnectionString("App"));

builder.Services.ConfigureDbContext<ProposalsContext>(
    builder.Configuration,
    "App"
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

app.UseJsonExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseCors();

app.MapControllers();
app.MapHub<ProposalEventHub>("/events/proposal");

app.Run();
