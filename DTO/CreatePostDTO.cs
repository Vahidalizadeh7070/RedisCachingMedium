namespace PostService.API.DTO;

public class CreatePostDTO
{
    public string Id {get;set;} = Guid.NewGuid().ToString();
    public string Title {get;set;}
    public string UserName {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
}