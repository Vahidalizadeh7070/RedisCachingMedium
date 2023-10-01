using PostService.API.Models;

namespace PostService.API.Services;

public class PostServices : IPostServices
{
    private readonly AppDbContext _dbContext;

    public PostServices(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Post> AddPostAsync(Post post)
    {
        if (post is not null)
        {
            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();
        }
        return post;
    }

    public bool DeletePost(string id)
    {
        var post = _dbContext.Posts.FirstOrDefault(p=>p.Id == id);
        if(post is not null){
            _dbContext.Posts.Remove(post);
            _dbContext.SaveChanges();
            return true;
        }
        return false;
    }

    public async Task<Post> EditPostAsync(Post post)
    {
        var postRes = _dbContext.Posts.FirstOrDefault(p=>p.Id == post.Id);
        if(postRes is not null){
            postRes.Title = post.Title;
            postRes.UserName = post.UserName;
            await _dbContext.SaveChangesAsync();
            return post;
        }
        return null;
    }

    public IEnumerable<Post> GetAll()
    {
        return _dbContext.Posts.ToList();
    }

    public Post GetPostById(string id)
    {
        return _dbContext.Posts.FirstOrDefault(p=>p.Id == id);
    }
}