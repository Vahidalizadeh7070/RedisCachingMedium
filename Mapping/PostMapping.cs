using Mapster;
using PostService.API.DTO;
using PostService.API.Models;

namespace PostService.API.Mapping;

public class PostMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreatePostDTO,Post>();
        config.NewConfig<UpdatePostDTO,Post>();
        config.NewConfig<Post, ResponsePostDTO>();
    }
}