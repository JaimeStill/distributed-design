using Distributed.Core.Schema;
using Distributed.Core.Sync.Client;

namespace Distributed.Core.Services;
public abstract class EntitySaga<T> : SyncClient<T>, IEntitySaga<T>
where T : Entity
{
    /*
        Use the AddHostedService factory method to initialize the
        endpoint from IConfiguration
        
    */
    public EntitySaga(string endpoint) : base(endpoint)
    { }
}