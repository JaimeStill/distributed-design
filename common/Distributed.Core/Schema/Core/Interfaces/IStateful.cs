namespace Distributed.Core.Schema;
public interface IStateful<T> : IEntity where T : Enum
{
    T State { get; set; }
}