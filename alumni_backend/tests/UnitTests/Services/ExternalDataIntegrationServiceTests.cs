using Application.DTOs.ExternalData;
using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace UnitTests.Services;

/// <summary>
/// Unit tests สำหรับ ExternalDataIntegrationService
/// </summary>
public class ExternalDataIntegrationServiceTests : IDisposable
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IAlumniProfileRepository> _mockProfileRepository;
    private readonly Mock<ILogger<ExternalDataIntegrationService>> _mockLogger;
    private readonly ExternalDataIntegrationService _service;

    public ExternalDataIntegrationServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockProfileRepository = new Mock<IAlumniProfileRepository>();
        _mockLogger = new Mock<ILogger<ExternalDataIntegrationService>>();
        
        _service = new ExternalDataIntegrationService(
            _mockUserRepository.Object,
            _mockProfileRepository.Object,
            _mockLogger.Object);
    }

    #region Data Validation Tests

    [Fact]
    public void ValidateAlumniData_WithValidData_ReturnsTrue()
    {
        // Arrange
        var alumniData = CreateValidAlumniData();
        var result = new ImportResult();

        // Act
        var isValid = _service.ValidateAlumniData(alumniData, result);

        // Assert
        Assert.True(isValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateAlumniData_WithEmptyMemberID_ReturnsFalse()
    {
        // Arrange
        var alumniData = CreateValidAlumniData();
        alumniData.MemberID = "";
        var result = new ImportResult();

        // Act
        var isValid = _service.ValidateAlumniData(alumniData, result);

        // Assert
        Assert.False(isValid);
        Assert.Contains(result.Errors, e => e.Field == "MemberID" && e.ErrorCode == "REQUIRED_FIELD");
    }

    [Fact]
    public void ValidateAlumniData_WithNoNameFields_ReturnsFalse()
    {
        // Arrange
        var alumniData = CreateValidAlumniData();
        alumniData.NameInYearbook = "";
        alumniData.Firstname = "";
        var result = new ImportResult();

        // Act
        var isValid = _service.ValidateAlumniData(alumniData, result);

        // Assert
        Assert.False(isValid);
        Assert.Contains(result.Errors, e => e.Field == "Name" && e.ErrorCode == "REQUIRED_FIELD");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@test.com")]
    [InlineData("test.com")]
    public void ValidateAlumniData_WithInvalidEmail_ReturnsFalse(string invalidEmail)
    {
        // Arrange
        var alumniData = CreateValidAlumniData();
        alumniData.Email = invalidEmail;
        var result = new ImportResult();

        // Act
        var isValid = _service.ValidateAlumniData(alumniData, result);

        // Assert
        Assert.False(isValid);
        Assert.Contains(result.Errors, e => e.Field == "Email" && e.ErrorCode == "INVALID_FORMAT");
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.th")]
    [InlineData("")]
    [InlineData(null)]
    public void ValidateAlumniData_WithValidEmail_ReturnsTrue(string? validEmail)
    {
        // Arrange
        var alumniData = CreateValidAlumniData();
        alumniData.Email = validEmail;
        var result = new ImportResult();

        // Act
        var isValid = _service.ValidateAlumniData(alumniData, result);

        // Assert
        Assert.True(isValid);
        Assert.DoesNotContain(result.Errors, e => e.Field == "Email");
    }

    #endregion

    #region Phone Number Validation Tests

    [Theory]
    [InlineData("0812345678", "66812345678")]
    [InlineData("66812345678", "66812345678")]
    [InlineData("+66812345678", "66812345678")]
    [InlineData("081-234-5678", "66812345678")]
    [InlineData("081 234 5678", "66812345678")]
    public void ValidateAndNormalizePhoneNumber_WithValidPhone_ReturnsNormalized(string input, string expected)
    {
        // Arrange
        var result = new ImportResult();

        // Act
        var normalized = _service.ValidateAndNormalizePhoneNumber(input, "TEST001", result);

        // Assert
        Assert.Equal(expected, normalized);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("0712345678")] // Invalid prefix
    [InlineData("081234567")] // Too short
    [InlineData("08123456789")] // Too long
    [InlineData("abcdefghij")] // Invalid format
    public void ValidateAndNormalizePhoneNumber_WithInvalidPhone_ReturnsNull(string invalidPhone)
    {
        // Arrange
        var result = new ImportResult();

        // Act
        var normalized = _service.ValidateAndNormalizePhoneNumber(invalidPhone, "TEST001", result);

        // Assert
        Assert.Null(normalized);
        Assert.Contains(result.Errors, e => e.Field == "MobilePhone" && e.ErrorCode == "INVALID_PHONE_FORMAT");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ValidateAndNormalizePhoneNumber_WithEmptyPhone_ReturnsNull(string? emptyPhone)
    {
        // Arrange
        var result = new ImportResult();

        // Act
        var normalized = _service.ValidateAndNormalizePhoneNumber(emptyPhone, "TEST001", result);

        // Assert
        Assert.Null(normalized);
        Assert.Empty(result.Errors);
    }

    #endregion

    #region Email Validation Tests

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user.name@domain.co.th", true)]
    [InlineData("test+tag@example.org", true)]
    [InlineData("", true)] // Empty is valid (optional)
    [InlineData(null, true)] // Null is valid (optional)
    [InlineData("invalid-email", false)]
    [InlineData("test@", false)]
    [InlineData("@test.com", false)]
    [InlineData("test.com", false)]
    public void IsValidEmail_WithVariousEmails_ReturnsExpected(string? email, bool expected)
    {
        // Act
        var result = _service.IsValidEmail(email);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Duplicate Detection Tests

    [Fact]
    public void DetectAndHandleDuplicates_WithNoDuplicates_ReturnsAllRecords()
    {
        // Arrange
        var data = new List<ExternalAlumniData>
        {
            CreateValidAlumniData("001", "0812345678"),
            CreateValidAlumniData("002", "0823456789"),
            CreateValidAlumniData("003", "0834567890")
        };
        var result = new ImportResult();

        // Act
        var cleanedData = _service.DetectAndHandleDuplicates(data, result);

        // Assert
        Assert.Equal(3, cleanedData.Count);
        Assert.Equal(0, result.SkippedRecords);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void DetectAndHandleDuplicates_WithDuplicateMemberIDs_RemovesDuplicates()
    {
        // Arrange
        var data = new List<ExternalAlumniData>
        {
            CreateValidAlumniData("001", "0812345678"),
            CreateValidAlumniData("001", "0823456789"), // Duplicate MemberID
            CreateValidAlumniData("002", "0834567890")
        };
        var result = new ImportResult();

        // Act
        var cleanedData = _service.DetectAndHandleDuplicates(data, result);

        // Assert
        Assert.Equal(2, cleanedData.Count);
        Assert.Equal(1, result.SkippedRecords);
        Assert.Contains(result.Warnings, w => w.Field == "MemberID");
    }

    [Fact]
    public void DetectAndHandleDuplicates_WithDuplicatePhones_RemovesDuplicates()
    {
        // Arrange
        var data = new List<ExternalAlumniData>
        {
            CreateValidAlumniData("001", "0812345678"),
            CreateValidAlumniData("002", "081-234-5678"), // Same phone, different format
            CreateValidAlumniData("003", "0823456789")
        };
        var result = new ImportResult();

        // Act
        var cleanedData = _service.DetectAndHandleDuplicates(data, result);

        // Assert
        Assert.Equal(2, cleanedData.Count);
        Assert.Equal(1, result.SkippedRecords);
        Assert.Contains(result.Warnings, w => w.Field == "MobilePhone");
    }

    #endregion

    #region Bulk Processing Tests

    [Fact]
    public async Task ProcessBulkDataAsync_WithValidationOnly_ValidatesWithoutProcessing()
    {
        // Arrange
        var request = new BulkImportRequest
        {
            Alumni = new List<ExternalAlumniData> { CreateValidAlumniData() },
            ExternalSystemId = "TEST_SYSTEM",
            ValidateOnly = true
        };

        // Act
        var result = await _service.ProcessBulkDataAsync(request);

        // Assert
        Assert.Equal(1, result.TotalRecords);
        Assert.Equal(0, result.SuccessfulImports);
        Assert.Empty(result.Errors);
        
        // Verify no database operations were called
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockProfileRepository.Verify(x => x.AddAsync(It.IsAny<AlumniProfile>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ValidateDataAsync_WithMixedValidInvalidData_ReturnsCorrectCounts()
    {
        // Arrange
        var data = new List<ExternalAlumniData>
        {
            CreateValidAlumniData("001"),
            CreateInvalidAlumniData("002"), // Missing required fields
            CreateValidAlumniData("003")
        };

        // Act
        var result = await _service.ValidateDataAsync(data, "TEST_SYSTEM");

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.ValidRecords);
        Assert.Equal(1, result.InvalidRecords);
        Assert.NotEmpty(result.Errors);
    }

    #endregion

    #region Single Record Processing Tests

    [Fact]
    public async Task SyncSingleRecordAsync_WithValidData_ReturnsTrue()
    {
        // Arrange
        var alumniData = CreateValidAlumniData();
        
        _mockUserRepository
            .Setup(x => x.GetByExternalMemberIDAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        _mockUserRepository
            .Setup(x => x.GetByMobilePhoneAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestUser());

        // Act
        var result = await _service.SyncSingleRecordAsync(alumniData, "TEST_SYSTEM");

        // Assert
        Assert.True(result);
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockProfileRepository.Verify(x => x.AddAsync(It.IsAny<AlumniProfile>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SyncSingleRecordAsync_WithInvalidData_ReturnsFalse()
    {
        // Arrange
        var alumniData = CreateInvalidAlumniData();

        // Act
        var result = await _service.SyncSingleRecordAsync(alumniData, "TEST_SYSTEM");

        // Assert
        Assert.False(result);
        
        // Verify no database operations were called
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockProfileRepository.Verify(x => x.AddAsync(It.IsAny<AlumniProfile>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region User Management Tests

    [Fact]
    public async Task FindExistingUserAsync_WithExternalMemberID_FindsUser()
    {
        // Arrange
        var expectedUser = CreateTestUser();
        _mockUserRepository
            .Setup(x => x.GetByExternalMemberIDAsync("TEST001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _service.FindExistingUserAsync("TEST001", "66812345678");

        // Assert
        Assert.Equal(expectedUser, result);
        _mockUserRepository.Verify(x => x.GetByExternalMemberIDAsync("TEST001", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FindExistingUserAsync_WithMobilePhone_FindsUser()
    {
        // Arrange
        var expectedUser = CreateTestUser();
        _mockUserRepository
            .Setup(x => x.GetByExternalMemberIDAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        _mockUserRepository
            .Setup(x => x.GetByMobilePhoneAsync("66812345678", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _service.FindExistingUserAsync("TEST001", "66812345678");

        // Assert
        Assert.Equal(expectedUser, result);
        _mockUserRepository.Verify(x => x.GetByMobilePhoneAsync("66812345678", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateNewUserWithProfileAsync_CreatesUserAndProfile()
    {
        // Arrange
        var alumniData = CreateValidAlumniData();
        var createdUser = CreateTestUser();
        
        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _service.CreateNewUserWithProfileAsync(
            alumniData, "TEST_SYSTEM", "66812345678");

        // Assert
        Assert.Equal(createdUser, result);
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProfileRepository.Verify(x => x.AddAsync(It.IsAny<AlumniProfile>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockProfileRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Helper Methods

    private static ExternalAlumniData CreateValidAlumniData(string memberID = "TEST001", string? mobilePhone = "0812345678")
    {
        return new ExternalAlumniData
        {
            MemberID = memberID,
            NameInYearbook = "Test User",
            Firstname = "Test",
            Lastname = "User",
            MobilePhone = mobilePhone,
            Email = "test@example.com",
            GraduationYear = 2020,
            DateOfBirth = new DateTime(1998, 1, 1)
        };
    }

    private static ExternalAlumniData CreateInvalidAlumniData(string memberID = "")
    {
        return new ExternalAlumniData
        {
            MemberID = memberID, // Invalid: empty
            NameInYearbook = "", // Invalid: empty
            Firstname = "", // Invalid: empty when NameInYearbook is also empty
            Email = "invalid-email", // Invalid format
            MobilePhone = "0712345678" // Invalid prefix
        };
    }

    private static User CreateTestUser()
    {
        return new User
        {
            Id = 1,
            MobilePhone = "66812345678",
            Email = "test@example.com",
            Name = "Test User",
            ExternalMemberID = "TEST001",
            ExternalSystemId = "TEST_SYSTEM",
            Role = UserRole.Alumni,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    #endregion

    public void Dispose()
    {
        // Clean up resources if needed
        GC.SuppressFinalize(this);
    }
}