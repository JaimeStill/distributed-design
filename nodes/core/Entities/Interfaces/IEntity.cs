using Microsoft.EntityFrameworkCore;

namespace Distributed.Core.Entities;

public interface IEntity
{
    int Id { get; set; }
    string Type { get; set; }
    string Value { get; set; }
    DateTime DateCreated { get; set; }
    DateTime DateModified { get; set; }

    public void OnSaving(EntityState state);
}