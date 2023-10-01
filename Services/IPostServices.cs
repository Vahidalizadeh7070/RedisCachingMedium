using System.Runtime.CompilerServices;
using PostService.API.Models;

namespace PostService.API.Services;

public interface IPostServices
{
    Task<Post> AddPostAsync(Post post);
    Task<Post> EditPostAsync(Post post);
    bool DeletePost(string id);
    Post GetPostById(string id);
    IEnumerable<Post> GetAll();
}