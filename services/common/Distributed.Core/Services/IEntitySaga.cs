using Distributed.Core.Sync.Client;

namespace Distributed.Core.Services;
public interface IEntitySaga<T> : ISyncClient<T>
{    
}