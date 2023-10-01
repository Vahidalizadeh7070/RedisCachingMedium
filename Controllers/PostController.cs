using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using PostService.API.DTO;
using PostService.API.Models;
using PostService.API.RedisCaching;
using PostService.API.Services;

namespace PostService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IPostServices _postServices;
    private readonly ICacheService _cacheService;
    private static object _lock = new Object();

    public PostController(IMapper mapper, IPostServices postServices, ICacheService cacheService)
    {
        _mapper = mapper;
        _postServices = postServices;
        _cacheService = cacheService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var cache = _cacheService.GetAll();
        if (cache != null)
        {
            var mapRes = _mapper.Map<IEnumerable<ResponsePostDTO>>(cache);
            return Ok(mapRes);
        }
        var res = _postServices.GetAll();
        if (res.Any())
        {
            var mapRes = _mapper.Map<IEnumerable<ResponsePostDTO>>(res);
            lock (_lock)
            {
                
                foreach (var item in res)
                {
                    _cacheService.SetPost("posts", item);
                }
            }
            return Ok(mapRes);
        }
        return NoContent();
    }

    [HttpGet("details")]
    public IActionResult GetById(string id)
    {
        var cache = _cacheService.GetData("posts", id);

        if (cache is not null)
        {
            var mapRes = _mapper.Map<ResponsePostDTO>(cache);
            return Ok(mapRes);
        }
        var res = _postServices.GetPostById(id);
        if (res is not null)
        {
            var mapRes = _mapper.Map<ResponsePostDTO>(res);
            lock (_lock)
            {
                var expirationTime = DateTimeOffset.Now.AddMinutes(60);
                _cacheService.SetPost("posts", res);
            }

            return Ok(mapRes);
        }
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreatePostDTO createPostDTO)
    {
        var mapInput = _mapper.Map<Post>(createPostDTO);
        var res = await _postServices.AddPostAsync(mapInput);
        if (res is not null)
        {
             // Set new to the redis instance
            lock (_lock)
            {
                _cacheService.SetPost("posts", mapInput);
            }
            var mapRes = _mapper.Map<ResponsePostDTO>(res);
            return CreatedAtAction(nameof(GetById), new { id = mapRes.Id }, mapRes);
        }
        return BadRequest();
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UpdatePostDTO updatePostDTO)
    {
        var mapInput = _mapper.Map<Post>(updatePostDTO);


        var res = await _postServices.EditPostAsync(mapInput);
        if (res is not null)
        {
            // Set new to the redis instance
            lock (_lock)
            {
                // Remove from Redis
                var getData = _cacheService.GetData("posts", updatePostDTO.Id);
                if (getData is not null)
                {
                    var removeResult = _cacheService.RemovePost("posts", getData.Id);
                }
                _cacheService.SetPost("posts", mapInput);
            }
            var mapRes = _mapper.Map<ResponsePostDTO>(res);
            return Ok(mapRes);
        }
        return BadRequest();
    }

    [HttpDelete]
    public IActionResult Delete(string id)
    {
        var getData = _cacheService.GetData("posts", id);
        if (getData is not null)
        {
            var removeResult = _cacheService.RemovePost("posts", getData.Id);
        }

        var res = _postServices.DeletePost(id);
        if (res is true)
        {
            return Ok($"post with id {id} has been deleted successfully");
        }
        return BadRequest("There is an error.");
    }
}