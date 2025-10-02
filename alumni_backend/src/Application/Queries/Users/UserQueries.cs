using Application.DTOs;
using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Queries.Users;

// Get User by ID Query
public record GetUserByIdQuery(int UserId) : IRequest<UserDto?>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetWithProfileAsync(request.UserId, cancellationToken);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", request.UserId);
            throw;
        }
    }
}

// Get User by Email Query
public record GetUserByEmailQuery(string Email) : IRequest<UserDto?>;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserByEmailQueryHandler> _logger;

    public GetUserByEmailQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUserByEmailQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", request.Email);
            throw;
        }
    }
}

// Get All Active Users Query
public record GetAllActiveUsersQuery() : IRequest<IEnumerable<UserDto>>;

public class GetAllActiveUsersQueryHandler : IRequestHandler<GetAllActiveUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllActiveUsersQueryHandler> _logger;

    public GetAllActiveUsersQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetAllActiveUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllActiveUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _userRepository.GetActiveUsersAsync(cancellationToken);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all active users");
            throw;
        }
    }
}

// Get Users Paginated Query
public record GetUsersPaginatedQuery(int Page = 1, int PageSize = 10) : IRequest<PaginatedResultDto<UserDto>>;

public class GetUsersPaginatedQueryHandler : IRequestHandler<GetUsersPaginatedQuery, PaginatedResultDto<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUsersPaginatedQueryHandler> _logger;

    public GetUsersPaginatedQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUsersPaginatedQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResultDto<UserDto>> Handle(GetUsersPaginatedQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (users, totalCount) = await _userRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                predicate: u => u.IsActive,
                orderBy: u => u.CreatedAt,
                ascending: false,
                cancellationToken: cancellationToken);

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            return new PaginatedResultDto<UserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users paginated. Page: {Page}, PageSize: {PageSize}", request.Page, request.PageSize);
            throw;
        }
    }
}