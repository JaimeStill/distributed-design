using Distributed.Core.Extensions;
using Distributed.Workflows.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddCommandLine(args);

builder.Services.ConfigureDbContext<WorkflowsContext>(builder.Configuration, "App");

using IHost host = builder.Build();

await host.RunAsync();