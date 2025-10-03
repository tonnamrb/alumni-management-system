using Application.DTOs;
using Application.DTOs.Posts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Event management endpoints (ใช้สำหรับ Events)
/// </summary>
[ApiController]
[Route("api/v1/events")]
[Authorize]
public class EventController : BaseController
{
    private readonly IMediator _mediator;

    public EventController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// ดึงรายการโพสต์ทั้งหมด (แบบแบ่งหน้า)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponseDto<List<PostDto>>), 200)]
    public ActionResult<ApiResponseDto<List<PostDto>>> GetPosts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(ErrorResponse<object>("พารามิเตอร์ไม่ถูกต้อง"));
            }

            // Mock data สำหรับตอนนี้
            var mockPosts = new List<PostDto>
            {
                new PostDto
                {
                    Id = 1,
                    Content = "ข้อมูลข่าวสาร",
                    CreatedAt = DateTime.UtcNow
                }
            };

            return Ok(SuccessResponse(mockPosts));
        }
        catch (Exception)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงข้อมูลโพสต์"));
        }
    }

    /// <summary>
    /// ดึงข้อมูลโพสต์ตาม ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponseDto<PostDto>), 200)]
    public ActionResult<ApiResponseDto<PostDto>> GetPost(int id)
    {
        try
        {
            // Mock data สำหรับตอนนี้
            var mockPost = new PostDto
            {
                Id = id,
                Content = "ข้อมูลข่าวสาร",
                CreatedAt = DateTime.UtcNow
            };

            return Ok(SuccessResponse(mockPost));
        }
        catch (Exception)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงข้อมูลโพสต์"));
        }
    }

    /// <summary>
    /// สร้างโพสต์ใหม่
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<PostDto>), 201)]
    public ActionResult<ApiResponseDto<PostDto>> CreatePost([FromBody] CreatePostDto createPostDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return UnauthorizedResponse();
            }

            // Mock response สำหรับตอนนี้
            var mockPost = new PostDto
            {
                Id = new Random().Next(1, 1000),
                Content = createPostDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            return CreatedAtAction(nameof(GetPost), new { id = mockPost.Id }, 
                SuccessResponse(mockPost, "สร้างโพสต์สำเร็จ"));
        }
        catch (Exception)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการสร้างโพสต์"));
        }
    }
}