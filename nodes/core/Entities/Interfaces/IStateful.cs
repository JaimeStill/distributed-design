namespace Distributed.Core.Entities;
public interface IStateful<T> : IEntity where T : Enum
{
    T State { get; set; }
}