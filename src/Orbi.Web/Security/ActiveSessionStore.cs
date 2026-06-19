using Microsoft.Extensions.Caching.Memory;

namespace Orbi.Web.Security;

public class ActiveSessionStore
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromHours(8);
    private readonly IMemoryCache _cache;

    public ActiveSessionStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool IsActive(string userId)
    {
        return _cache.TryGetValue(GetKey(userId), out _);
    }

    public void MarkActive(string userId)
    {
        _cache.Set(GetKey(userId), true, SessionLifetime);
    }

    public void Clear(string userId)
    {
        _cache.Remove(GetKey(userId));
    }

    private static string GetKey(string userId) => $"auth:active-session:{userId}";
}
