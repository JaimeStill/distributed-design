using Distributed.Core.Data;
using Microsoft.EntityFrameworkCore;
using Workflows.Contracts;

namespace Workflows.Data;
public class WorkflowsContext : EntityContext<WorkflowsContext>
{
    public WorkflowsContext(DbContextOptions<WorkflowsContext> options) : base(options) { }

    public DbSet<Package> Packages => Set<Package>();
}