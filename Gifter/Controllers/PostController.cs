using Gifter.Models;
using Gifter.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlTypes;

namespace Gifter.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly IPostRepository _postRepository;
    public PostController(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_postRepository.GetAll());
    }

    [HttpGet("GetWithComments")]
    public IActionResult GetWithComments()
    {
        var posts = _postRepository.GetAll(true);
        return Ok(posts);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var post = _postRepository.GetById(id);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var post = _postRepository.GetById(id);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpPost]
    public IActionResult Post(Post post)
    {
        _postRepository.Add(post);
        return CreatedAtAction("Get", new { id = post.Id }, post);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, Post post)
    {
         return id != post.Id 
                        ? BadRequest() 
                        : _postRepository.Update(post) 
                            ? NoContent() 
                            : NotFound();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return _postRepository.Delete(id)
                    ? NoContent()
                    : NotFound();
    }
}