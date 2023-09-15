using Distributed.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Proposals.Data;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddCommandLine(args);

builder.Services.ConfigureDbContext<ProposalsContext>(builder.Configuration, "App");

using IHost host = builder.Build();

await host.RunAsync();