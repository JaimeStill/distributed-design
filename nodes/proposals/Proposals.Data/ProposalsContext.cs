using Distributed.Contracts;
using Distributed.Core.Data;
using Microsoft.EntityFrameworkCore;
using Proposals.Entities;

namespace Proposals.Data;
public class ProposalsContext : EntityContext<ProposalsContext>
{
    public ProposalsContext(DbContextOptions<ProposalsContext> options) : base(options) { }

    public DbSet<Proposal> Proposals => Set<Proposal>();
    public DbSet<Status> Statuses => Set<Status>();
}