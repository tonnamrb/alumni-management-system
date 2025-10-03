# Implementation Tasks: Schema Migration & Integration

## 🎯 Overview
This document provides detailed task breakdown for migrating from current schema to backoffice unified schema while maintaining both development and staging environment compatibility.

---

## 📋 Phase 3: Application Layer Migration (CURRENT PRIORITY)

### Task 3.1: AuthenticationService Fixes
**Priority**: 🔥 Critical  
**Estimated Time**: 2 hours  
**Status**: Pending  

#### Files to Fix:
- `src/Application/Services/AuthenticationService.cs`

#### Specific Changes:

1. **Remove IsActive Logic (Lines 51, 135, 260)**
   ```csharp
   // ❌ REMOVE these checks:
   if (!user.IsActive)
   {
       return Result<LoginResponseDto>.Failure("Account is deactivated");
   }
   
   // ✅ All users are active in new schema - remove these blocks
   ```

2. **Update LastLoginAt Usage (Lines 65, 149, 256)**
   ```csharp
   // ❌ REMOVE these calls:
   user.UpdateLastLogin();
   user.LastLoginAt = DateTime.UtcNow;
   
   // ✅ Login tracking removed from new schema
   ```

3. **Fix Name Property Usage (Lines 97, 186, 239)**
   ```csharp
   // ❌ BEFORE:
   Name = user.Name,
   
   // ✅ AFTER:  
   Name = user.FullName, // or $"{user.Firstname} {user.Lastname}"
   ```

4. **Update Role Assignments (Lines 100, 190, 243)**
   ```csharp
   // ❌ BEFORE:
   Role = role,  // UserRole enum
   
   // ✅ AFTER:
   RoleId = user.RoleId,
   RoleName = user.Role?.Name ?? "Unknown"
   ```

#### Acceptance Criteria:
- [ ] AuthenticationService compiles without errors
- [ ] Login functionality works with new User schema
- [ ] Role-based access control functions properly
- [ ] No references to removed fields (IsActive, LastLoginAt)

---

### Task 3.2: User Commands & Queries Updates
**Priority**: 🔥 Critical  
**Estimated Time**: 1.5 hours  
**Status**: Pending  

#### Files to Fix:
- `src/Application/Commands/Users/UserCommands.cs`
- `src/Application/Commands/Users/AuthCommands.cs`  
- `src/Application/Queries/Users/UserQueries.cs`

#### Specific Changes:

1. **UserCommands.cs (Lines 132, 148, 196)**
   ```csharp
   // ❌ BEFORE:
   user.Name = request.Name;
   
   // ✅ AFTER:
   user.Firstname = request.Firstname;
   user.Lastname = request.Lastname;
   ```

2. **AuthCommands.cs (Lines 48, 69)**
   ```csharp
   // ❌ REMOVE:
   if (!user.IsActive) { ... }
   user.LastLoginAt = DateTime.UtcNow;
   
   // ✅ Clean up - remove these blocks entirely
   ```

#### Acceptance Criteria:
- [ ] All user command handlers compile
- [ ] User creation/update uses new field structure
- [ ] Auth commands work without removed fields

---

### Task 3.3: AutoMapper Profile Updates
**Priority**: 🔥 Critical  
**Estimated Time**: 1 hour  
**Status**: Pending  

#### Files to Fix:
- `src/Application/Mappings/UserMappingProfile.cs`

#### Specific Changes:

1. **Update User → UserDto Mapping**
   ```csharp
   // ❌ REMOVE these mappings:
   .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
   .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider))
   .ForMember(dest => dest.LastLoginAt, opt => opt.MapFrom(src => src.LastLoginAt))
   
   // ✅ ADD these mappings:
   .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.Firstname))
   .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.Lastname))
   .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
   .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
   .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
   ```

2. **Update CreateUserDto → User Mapping**
   ```csharp
   // Update to use new field structure
   .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.Firstname))
   .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.Lastname))
   .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
   ```

#### Acceptance Criteria:
- [ ] AutoMapper profiles compile without errors
- [ ] All field mappings use new schema
- [ ] No mappings to removed fields

---

### Task 3.4: Service Layer Updates
**Priority**: 🔴 High  
**Estimated Time**: 2 hours  
**Status**: Pending  

#### Files to Fix:
- `src/Application/Services/ReportService.cs`
- `src/Application/Services/PostService.cs`  
- `src/Application/Services/CommentService.cs`
- `src/Application/Services/ExternalDataIntegrationService.cs`

#### Specific Changes:

1. **ReportService.cs (Lines 187, 242)**
   ```csharp
   // ❌ BEFORE:
   if (user.Role != UserRole.Administrator)
   
   // ✅ AFTER:
   if (user.RoleId != 2) // or user.IsAdmin or !user.IsAdministrator()
   ```

2. **PostService.cs (Lines 255, 291, 348, 349)**
   ```csharp
   // ❌ BEFORE:
   if (user.Role != UserRole.Administrator)
   AuthorName = post.User.Name,
   AuthorPictureUrl = post.User.PictureUrl,
   
   // ✅ AFTER:
   if (user.RoleId != 2)
   AuthorName = post.User.FullName,
   AuthorPictureUrl = post.User.AlumniProfile?.ProfilePictureUrl,
   ```

3. **ExternalDataIntegrationService.cs (Multiple lines)**
   ```csharp
   // Update all field assignments to use new schema
   user.Firstname = externalData.Firstname;
   user.Lastname = externalData.Lastname;
   // Remove assignments to non-existent fields
   ```

#### Acceptance Criteria:
- [ ] All services compile without errors
- [ ] Role checking uses new RoleId system
- [ ] User name display uses new field structure
- [ ] No references to removed fields

---

### Task 3.5: DTOs and Interfaces Updates  
**Priority**: 🟡 Medium  
**Estimated Time**: 1.5 hours  
**Status**: Pending  

#### Files to Fix:
- Update `UserDto` class structure
- Fix `IUserRepository` interface methods
- Update validation rules
- Fix API controller endpoints

#### Specific Changes:

1. **UserDto Updates**
   ```csharp
   public class UserDto
   {
       // Remove obsolete fields
       // public bool IsActive { get; set; }
       // public DateTime? LastLoginAt { get; set; }
       
       // Add new fields
       public string Firstname { get; set; }
       public string Lastname { get; set; }
       public string FullName { get; set; }
       public int RoleId { get; set; }
       public string? RoleName { get; set; }
       
       // Member-specific fields (when RoleId = 1)
       public string? MemberID { get; set; }
       public string? GroupID { get; set; }
       // ... other member fields
   }
   ```

#### Acceptance Criteria:
- [ ] All DTOs match new entity structure
- [ ] Interface methods compile correctly
- [ ] Validation rules updated for new fields

---

## 📋 Phase 4: Development & Staging Migration

### Task 4.1: Development Environment Migration
**Priority**: 🟡 Medium  
**Estimated Time**: 1 hour  
**Status**: Pending  

#### Steps:
1. **Clean Migration Reset**
   ```bash
   # Remove existing migrations
   rm -rf src/Infrastructure/Migrations/*
   
   # Create new unified migration
   dotnet ef migrations add UnifiedSchemaInitial -p src/Infrastructure -s src/Api
   
   # Test migration
   dotnet ef database drop -f -p src/Infrastructure -s src/Api
   dotnet ef database update -p src/Infrastructure -s src/Api
   ```

2. **Verification Tests**
   ```bash
   # Build and test
   dotnet build
   dotnet test
   
   # Run application
   dotnet run --project src/Api
   ```

#### Acceptance Criteria:
- [ ] Clean migration creates all tables correctly
- [ ] Application starts without errors
- [ ] All endpoints accessible
- [ ] Authentication flow works

---

### Task 4.2: Staging Integration Testing
**Priority**: 🔴 High  
**Estimated Time**: 2 hours  
**Status**: Pending  

#### Prerequisites:
- [ ] Staging database connection string obtained
- [ ] Access credentials verified
- [ ] Existing Users/Roles tables confirmed

#### Steps:
1. **Pre-Deployment Verification**
   ```bash
   # Test database connection
   psql "Host=staging-host;Database=staging_db;Username=staging_user;Password=***" -c "\dt"
   
   # Verify existing data
   psql "connection_string" -c "SELECT COUNT(*) FROM \"Users\"; SELECT COUNT(*) FROM \"Roles\";"
   ```

2. **Deploy Missing Tables**
   ```bash
   # Run the prepared SQL script
   psql "connection_string" -f create_missing_tables.sql
   
   # Verify creation
   psql "connection_string" -c "SELECT table_name FROM information_schema.tables WHERE table_name IN ('Posts', 'AlumniProfiles', 'Comments', 'Likes', 'Reports', 'AuditLogs');"
   ```

3. **Application Deployment**
   ```bash
   # Set staging environment
   export ASPNETCORE_ENVIRONMENT=Staging
   
   # Deploy application
   dotnet run --project src/Api
   ```

4. **Integration Testing**
   ```bash
   # Test authentication with existing user
   curl -X POST https://staging-api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"email":"admin@example.com","password":"12345678"}'
   
   # Test social features
   curl -X GET https://staging-api/posts \
     -H "Authorization: Bearer {token}"
   ```

#### Acceptance Criteria:
- [ ] Staging database connection successful
- [ ] Missing tables created without conflicts
- [ ] Authentication works with existing users
- [ ] Social features functional
- [ ] No data corruption or loss

---

## 📋 Phase 5: Testing & Validation

### Task 5.1: Comprehensive Unit Testing
**Priority**: 🟡 Medium  
**Estimated Time**: 2 hours  
**Status**: Pending  

#### Test Categories:
1. **Entity Tests**
   - User entity validation
   - Role relationship tests
   - Navigation property functionality

2. **Repository Tests**
   - CRUD operations
   - Query methods
   - Role-based filtering

3. **Service Tests**  
   - Authentication service
   - User management
   - Social feature services

#### Acceptance Criteria:
- [ ] All unit tests pass
- [ ] Code coverage > 80%
- [ ] No test dependencies on removed fields

---

### Task 5.2: End-to-End Integration Testing
**Priority**: 🔴 High  
**Estimated Time**: 2 hours  
**Status**: Pending  

#### Test Scenarios:
1. **Authentication Flow**
   - User registration
   - Login/logout
   - Role-based access

2. **Alumni Features**
   - Profile creation/editing
   - Profile picture upload
   - Profile visibility settings

3. **Social Features**
   - Post creation/editing
   - Comments and replies
   - Like functionality
   - Content reporting

#### Acceptance Criteria:
- [ ] All user journeys complete successfully
- [ ] No errors in application logs
- [ ] Performance meets requirements
- [ ] Security features functional

---

## 🚀 Deployment Checklist

### Pre-Deployment
- [ ] All compilation errors resolved
- [ ] Unit tests passing
- [ ] Integration tests passing
- [ ] Database scripts tested
- [ ] Configuration files updated

### Deployment Process
- [ ] **Development**: Clean migration applied
- [ ] **Staging**: Selective migration completed
- [ ] **Production**: Deployment strategy confirmed

### Post-Deployment Validation
- [ ] Application health check passing
- [ ] Authentication functional
- [ ] Social features operational
- [ ] Admin access working
- [ ] Performance metrics acceptable

---

## 📊 Progress Tracking

### Current Status
| Task | Priority | Estimated | Status | Progress |
|------|----------|-----------|--------|----------|
| 3.1 AuthenticationService | 🔥 Critical | 2h | Pending | 0% |
| 3.2 Commands & Queries | 🔥 Critical | 1.5h | Pending | 0% |
| 3.3 AutoMapper Updates | 🔥 Critical | 1h | Pending | 0% |
| 3.4 Service Layer | 🔴 High | 2h | Pending | 0% |
| 3.5 DTOs & Interfaces | 🟡 Medium | 1.5h | Pending | 0% |
| 4.1 Dev Migration | 🟡 Medium | 1h | Pending | 0% |
| 4.2 Staging Integration | 🔴 High | 2h | Pending | 0% |
| 5.1 Unit Testing | 🟡 Medium | 2h | Pending | 0% |
| 5.2 E2E Testing | 🔴 High | 2h | Pending | 0% |

### Total Remaining Effort
- **Critical/High Priority**: 9.5 hours
- **Medium Priority**: 4.5 hours  
- **Total**: 14 hours (approximately 2 working days)

---

## 🎯 Next Immediate Steps

1. **Start with Task 3.1** (AuthenticationService) - highest impact
2. **Progress through Task 3.2-3.4** - resolve all compilation errors
3. **Test development environment** - verify basic functionality  
4. **Prepare staging integration** - coordinate with backoffice team
5. **Execute comprehensive testing** - ensure system reliability

**Goal**: Zero compilation errors and working development environment within 8 hours.