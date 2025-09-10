using ApiSyncUser.Business;
using ApiSyncUser.Repositories;
using System.Linq;

namespace ApiSyncUser.Services;

public class SyncService : ISyncService
{
    private readonly IUserRepository _repository;

    public SyncService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task SyncAsync()
    {
        var unsynced = await _repository.GetUnsyncedAsync();
        if (unsynced.Count == 0)
            return;

        unsynced.ForEach(u =>
        {
            u.IsSync = true;
            u.DateSynced = DateTime.UtcNow;
        });

        await _repository.InsertReadAsync(unsynced);
        var ids = unsynced.Select(u => u.Id);
        await _repository.MarkSyncedAsync(ids);
    }
}
