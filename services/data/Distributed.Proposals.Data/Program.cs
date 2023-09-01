using Distributed.Core.Extensions;
using Distributed.Proposals.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddCommandLine(args);

builder.Services.ConfigureDbContext<ProposalsContext>(builder.Configuration, "App");

using IHost host = builder.Build();

await host.RunAsync();