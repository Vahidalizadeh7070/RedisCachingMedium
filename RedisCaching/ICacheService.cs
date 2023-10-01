using PostService.API.Models;

namespace PostService.API.RedisCaching;

public interface ICacheService
{
    Post GetData(string key, string id);
    IEnumerable<Post> GetAll();
    Post SetPost(string key, Post post);
    bool RemovePost(string key, string id);
}