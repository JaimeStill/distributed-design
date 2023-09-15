using Distributed.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Workflows.Services;
public class Registrant : ServiceRegistrant
{
    public Registrant(IServiceCollection services) : base(services)
    { }

    public override void Register()
    {
        services.AddScoped<PackageQuery>();
        services.AddScoped<PackageCommand>();

        services.AddScoped<ProcessQuery>();
        services.AddScoped<ProcessCommand>();

        services.AddScoped<WorkflowQuery>();
        services.AddScoped<WorkflowCommand>();

        services.AddScoped<ProcessTemplateQuery>();
        services.AddScoped<ProcessTemplateCommand>();

        services.AddScoped<WorkflowTemplateQuery>();
        services.AddScoped<WorkflowTemplateCommand>();
    }
}