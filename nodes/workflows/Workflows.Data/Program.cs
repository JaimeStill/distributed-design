using Distributed.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Workflows.Data;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddCommandLine(args);

builder.Services.ConfigureDbContext<WorkflowsContext>(builder.Configuration, "App");

using IHost host = builder.Build();

await host.RunAsync();