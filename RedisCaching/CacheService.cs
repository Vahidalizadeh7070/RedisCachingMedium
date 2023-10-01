using System.Text.Json;
using PostService.API.Models;
using StackExchange.Redis;

namespace PostService.API.RedisCaching;

public class CacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private IDatabase _db;
    public CacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }
    public IEnumerable<Post> GetAll()
    {
        var value = _db.HashGetAll("posts");
        if(value.Length > 0){
            var obj = Array.ConvertAll(value, val => JsonSerializer.Deserialize<Post>(val.Value));
            return obj;
        }
        return null;
    }

    public Post GetData(string key, string id)
    {
        var value = _db.HashGet(key,id);
        if(!value.IsNullOrEmpty){
            var obj = JsonSerializer.Deserialize<Post>(value);
            return obj;
        }
        return null;
    }

    public bool RemovePost(string key, string id)
    {
        bool isDeleted = _db.HashDelete(key, id);
        return isDeleted;
    }

    public Post SetPost(string key, Post post)
    {
        var expirationTime = DateTimeOffset.Now.AddMinutes(60);
        var expiration = expirationTime.DateTime.Subtract(DateTime.Now);
        var serializedObject = JsonSerializer.Serialize(post);
        _db.HashSet(key, new HashEntry[]{
            new HashEntry(post.Id, serializedObject)
        });
        _db.KeyExpire(key, expiration);
        return post;
    }
}