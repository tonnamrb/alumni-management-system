using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Profile management endpoints
/// </summary>
[ApiController]
[Route("api/v1/profiles")]
public class ProfileController : BaseController
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// ดึงรายชื่อโปรไฟล์ศิษย์เก่าทั้งหมด (แบบแบ่งหน้า)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponseDto<List<AlumniProfileDto>>), 200)]
    public async Task<ActionResult<ApiResponseDto<List<AlumniProfileDto>>>> GetAlumniProfiles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? graduationYear = null,
        [FromQuery] string? faculty = null)
    {
        try
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(ErrorResponse<object>("พารามิเตอร์ไม่ถูกต้อง"));
            }

            // Mock data สำหรับตอนนี้
            var mockProfiles = new List<AlumniProfileDto>
            {
                new AlumniProfileDto
                {
                    Id = 1,
                    UserId = 1,
                    Bio = "Software Developer",
                    Major = "Computer Engineering",
                    GraduationYear = "2020",
                    CurrentJobTitle = "Senior Developer",
                    CurrentCompany = "Tech Company"
                }
            };

            return Ok(SuccessResponse(mockProfiles));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงข้อมูลโปรไฟล์"));
        }
    }

    /// <summary>
    /// ดึงข้อมูลโปรไฟล์ตาม ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponseDto<AlumniProfileDto>), 200)]
    public async Task<ActionResult<ApiResponseDto<AlumniProfileDto>>> GetProfile(int id)
    {
        try
        {
            // Mock data สำหรับตอนนี้
            var mockProfile = new AlumniProfileDto
            {
                Id = id,
                UserId = 1,
                Bio = "Software Developer",
                Major = "Computer Engineering",
                GraduationYear = "2020",
                CurrentJobTitle = "Senior Developer",
                CurrentCompany = "Tech Company"
            };

            return Ok(SuccessResponse(mockProfile));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงข้อมูลโปรไฟล์"));
        }
    }

    /// <summary>
    /// สร้างโปรไฟล์ใหม่
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponseDto<AlumniProfileDto>), 201)]
    public async Task<ActionResult<ApiResponseDto<AlumniProfileDto>>> CreateProfile([FromBody] CreateAlumniProfileDto createDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return UnauthorizedResponse();
            }

            // Mock response สำหรับตอนนี้
            var mockProfile = new AlumniProfileDto
            {
                Id = new Random().Next(1, 1000),
                UserId = currentUserId.Value,
                Bio = createDto.Bio,
                Major = createDto.Major,
                GraduationYear = createDto.GraduationYear,
                CurrentJobTitle = createDto.CurrentJobTitle,
                CurrentCompany = createDto.CurrentCompany
            };

            return CreatedAtAction(nameof(GetProfile), new { id = mockProfile.Id },
                SuccessResponse(mockProfile, "สร้างโปรไฟล์สำเร็จ"));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการสร้างโปรไฟล์"));
        }
    }

    /// <summary>
    /// ค้นหาโปรไฟล์
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponseDto<List<AlumniProfileDto>>), 200)]
    public async Task<ActionResult<ApiResponseDto<List<AlumniProfileDto>>>> SearchProfiles([FromQuery] string searchTerm)
    {
        try
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest(ErrorResponse<object>("คำค้นหาเป็นข้อมูลที่จำเป็น"));
            }

            // Mock data สำหรับตอนนี้
            var mockResults = new List<AlumniProfileDto>
            {
                new AlumniProfileDto
                {
                    Id = 1,
                    UserId = 1,
                    Bio = "Software Developer",
                    Major = "Computer Engineering"
                }
            };

            return Ok(SuccessResponse(mockResults));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการค้นหา"));
        }
    }
}