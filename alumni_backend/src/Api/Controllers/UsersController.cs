using Application.Commands.Users;
using Application.DTOs;
using Application.Queries.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// User management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IValidator<UpdateUserDto> _updateUserValidator;

    public UsersController(
        IMediator mediator,
        IValidator<UpdateUserDto> updateUserValidator)
    {
        _mediator = mediator;
        _updateUserValidator = updateUserValidator;
    }

    /// <summary>
    /// ดึงรายชื่อผู้ใช้งานทั้งหมด (แบบแบ่งหน้า)
    /// </summary>
    /// <param name="page">หน้าที่ต้องการ</param>
    /// <param name="pageSize">จำนวนรายการต่อหน้า</param>
    /// <returns>รายชื่อผู้ใช้งานแบบแบ่งหน้า</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<PaginatedResultDto<UserDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    public async Task<ActionResult<ApiResponseDto<PaginatedResultDto<UserDto>>>> GetUsers(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(ErrorResponse<object>("พารามิเตอร์ไม่ถูกต้อง"));
            }

            var query = new GetUsersPaginatedQuery(page, pageSize);
            var result = await _mediator.Send(query);

            return Ok(SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงข้อมูลผู้ใช้งาน"));
        }
    }

    /// <summary>
    /// ดึงข้อมูลผู้ใช้งานตาม ID
    /// </summary>
    /// <param name="id">ID ของผู้ใช้งาน</param>
    /// <returns>ข้อมูลผู้ใช้งาน</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> GetUser(int id)
    {
        try
        {
            var query = new GetUserByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFoundResponse("ไม่พบผู้ใช้งาน");
            }

            return Ok(SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงข้อมูลผู้ใช้งาน"));
        }
    }

    /// <summary>
    /// ดึงรายชื่อผู้ใช้งานที่ active ทั้งหมด
    /// </summary>
    /// <returns>รายชื่อผู้ใช้งานที่ active</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<UserDto>>), 200)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<UserDto>>>> GetActiveUsers()
    {
        try
        {
            var query = new GetAllActiveUsersQuery();
            var result = await _mediator.Send(query);

            return Ok(SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการดึงข้อมูลผู้ใช้งาน"));
        }
    }

    /// <summary>
    /// อัพเดตข้อมูลผู้ใช้งาน (เฉพาะตนเองหรือ Admin)
    /// </summary>
    /// <param name="id">ID ของผู้ใช้งาน</param>
    /// <param name="updateUserDto">ข้อมูลที่ต้องการอัพเดต</param>
    /// <returns>ข้อมูลผู้ใช้งานที่อัพเดตแล้ว</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return UnauthorizedResponse();
            }

            // ตรวจสอบสิทธิ์: อัพเดตได้เฉพาะตนเองหรือเป็น Admin
            if (currentUserId != id && !IsCurrentUserAdmin())
            {
                return ForbiddenResponse("ไม่มีสิทธิ์แก้ไขข้อมูลผู้ใช้งานคนนี้");
            }

            // Validation
            var validationResult = await _updateUserValidator.ValidateAsync(updateUserDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new ValidationErrorDto
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList();

                return ValidationErrorResponse("ข้อมูลไม่ถูกต้อง", errors);
            }

            // Execute command
            var command = new UpdateUserCommand(id, updateUserDto);
            var result = await _mediator.Send(command);

            return Ok(SuccessResponse(result, "อัพเดตข้อมูลผู้ใช้งานสำเร็จ"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ErrorResponse<object>(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการอัพเดตข้อมูลผู้ใช้งาน"));
        }
    }

    /// <summary>
    /// ลบผู้ใช้งาน (เฉพาะ Admin)
    /// </summary>
    /// <param name="id">ID ของผู้ใช้งาน</param>
    /// <returns>ผลการลบ</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 403)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteUser(int id)
    {
        try
        {
            var command = new DeleteUserCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFoundResponse("ไม่พบผู้ใช้งาน");
            }

            return Ok(SuccessResponse(result, "ลบผู้ใช้งานสำเร็จ"));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการลบผู้ใช้งาน"));
        }
    }

    /// <summary>
    /// ค้นหาผู้ใช้งานตามอีเมล
    /// </summary>
    /// <param name="email">อีเมลที่ต้องการค้นหา</param>
    /// <returns>ข้อมูลผู้ใช้งาน</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponseDto<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> SearchUserByEmail([FromQuery] string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(ErrorResponse<object>("อีเมลเป็นข้อมูลที่จำเป็น"));
            }

            var query = new GetUserByEmailQuery(email);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFoundResponse("ไม่พบผู้ใช้งาน");
            }

            return Ok(SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ErrorResponse<object>("เกิดข้อผิดพลาดในการค้นหาผู้ใช้งาน"));
        }
    }
}