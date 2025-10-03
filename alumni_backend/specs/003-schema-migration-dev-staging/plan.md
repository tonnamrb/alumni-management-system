# Implementation Plan: Schema Migration & Integration

## 🎯 Current Status

### ✅ **Completed (Phase 1-2)**
- [x] Created `Role` entity with proper relationships
- [x] Updated `User` entity to match backoffice schema  
- [x] Created `RoleConfiguration` and updated `UserConfiguration`
- [x] Updated `AppDbContext` and `UserRepository`
- [x] Implemented environment-specific migration logic
- [x] Created `create_missing_tables.sql` script

### ⚠️ **In Progress (Phase 3)**
**57 Compilation Errors** in Application Layer need fixing:
- `AuthenticationService.cs`
- `UserService.cs` 
- `ReportService.cs`
- `PostService.cs`
- `CommentService.cs`
- `UserMappingProfile.cs`
- Various other services and DTOs

### 🎯 **Next Steps (Phase 3-5)**
1. Fix Application Layer compilation errors
2. Update DTOs and interfaces
3. Test development environment
4. Test staging integration
5. Documentation and deployment guides

---

## 📋 Phase 3: Application Layer Migration (Current Focus)

### 🔨 Task 3.1: Authentication Service Updates (Priority: High)

#### Issues to Fix:
```csharp
// AuthenticationService.cs - Line 48, 51, 65, etc.
// ❌ Current (broken):
user.IsActive
user.LastLoginAt  
user.UpdateLastLogin()
user.Name

// ✅ Target (new schema):
// Remove IsActive checks (not in new schema)
// Remove LastLoginAt usage
// Use user.Firstname + user.Lastname instead of user.Name
// Update Role checks to use RoleId instead of Role enum
```

#### Implementation Plan:
1. **Remove IsActive Logic**
   - All users are considered active in new schema
   - Remove `user.IsActive` checks

2. **Update Login Tracking**  
   - Remove `LastLoginAt` and `UpdateLastLogin()` calls
   - Consider alternative tracking if needed

3. **Fix Name Usage**
   ```csharp
   // Before: user.Name  
   // After: $"{user.Firstname} {user.Lastname}"
   // Or use: user.FullName (computed property)
   ```

4. **Update Role Checking**
   ```csharp
   // Before: user.Role == UserRole.Alumni
   // After: user.RoleId == 1 || user.IsAlumni()
   ```

### 🔨 Task 3.2: User Service Updates

#### Issues to Fix:
- Update user creation/update logic
- Fix name field handling
- Update role assignment
- Fix search functionality

### 🔨 Task 3.3: Mapping Profile Updates

#### Issues to Fix:
```csharp
// UserMappingProfile.cs
// ❌ Current:
.ForMember(dest => dest.IsActive, ...)
.ForMember(dest => dest.Provider, ...)
.ForMember(dest => dest.LastLoginAt, ...)

// ✅ Target:
.ForMember(dest => dest.Firstname, ...)
.ForMember(dest => dest.Lastname, ...)
.ForMember(dest => dest.RoleId, ...)
```

### 🔨 Task 3.4: Other Services Updates

#### Services needing fixes:
- `ReportService.cs` - Role comparisons
- `PostService.cs` - User name displays  
- `CommentService.cs` - Role checks
- `ExternalDataIntegrationService.cs` - Field mappings

---

## 📋 Phase 4: Migration Scripts & Environment Setup

### 🔧 Task 4.1: Development Environment Setup

#### Clean Migration Strategy:
```bash
# 1. Remove existing migrations
cd src/Infrastructure
rm -rf Migrations/*

# 2. Create fresh migration
dotnet ef migrations add UnifiedSchemaInitial -s ../Api

# 3. Apply to development database  
dotnet ef database update -s ../Api
```

### 🔧 Task 4.2: Staging Deployment Preparation

#### Pre-Deployment Checklist:
1. **Database Connection Testing**
   ```bash
   # Test connection to staging
   psql -h staging-host -U staging_user -d staging_db -c "\dt"
   ```

2. **Verify Existing Tables**
   ```sql
   SELECT table_name, table_rows 
   FROM information_schema.tables 
   WHERE table_name IN ('Users', 'Roles');
   ```

3. **Run Missing Tables Script**
   ```bash
   psql -h staging-host -U staging_user -d staging_db -f create_missing_tables.sql
   ```

### 🔧 Task 4.3: Application Configuration

#### Environment-Specific Settings:
```json
// appsettings.Staging.json
{
  "Migration": {
    "SkipUsersTables": true,
    "CreateSocialTables": true,
    "VerifyExistingSchema": true
  }
}
```

---

## 📋 Phase 5: Testing & Validation

### 🧪 Task 5.1: Unit Testing Updates

#### Test Categories:
1. **Entity Tests**
   - User entity property mappings
   - Role entity relationships  
   - Navigation property functionality

2. **Repository Tests**
   - CRUD operations with new schema
   - Search functionality
   - Role-based queries

3. **Service Tests**
   - Authentication with new fields
   - User management operations
   - Social feature integrations

### 🧪 Task 5.2: Integration Testing

#### Development Environment:
```bash
# 1. Fresh database test
dotnet ef database drop -f -s src/Api
dotnet ef database update -s src/Api

# 2. Seed test data
dotnet run --project src/Api --seed-data

# 3. Run integration tests
dotnet test --filter Category=Integration
```

#### Staging Environment:
```bash
# 1. Deploy application
ASPNETCORE_ENVIRONMENT=Staging dotnet run --project src/Api

# 2. Test authentication
curl -X POST https://staging-api/auth/login \
  -d '{"email":"admin@example.com","password":"12345678"}'

# 3. Test social features
curl -X POST https://staging-api/posts \
  -H "Authorization: Bearer {token}" \
  -d '{"content":"Test post","type":1}'
```

### 🧪 Task 5.3: User Acceptance Testing

#### Scenarios to Test:
1. **Authentication Flow**
   - Login with existing staging users
   - Admin vs Member access levels
   - Password validation (BCrypt)

2. **Profile Management**
   - View/edit alumni profiles
   - Upload profile pictures
   - Privacy settings

3. **Social Features**  
   - Create/edit posts
   - Comment on posts
   - Like functionality
   - Report inappropriate content

---

## 🚀 Deployment Strategy

### Development Deployment
1. **Clean Slate Approach**
   ```bash
   # Fresh migration with unified schema
   dotnet ef migrations add UnifiedSchema --output-dir Migrations
   dotnet ef database update
   ```

### Staging Deployment  
1. **Selective Migration Approach**
   ```bash
   # Don't run EF migrations - use SQL script instead
   ASPNETCORE_ENVIRONMENT=Staging
   # App will detect existing tables and skip Users/Roles migration
   ```

2. **Missing Tables Creation**
   ```bash
   # Run the prepared SQL script
   psql -f create_missing_tables.sql "connection_string"
   ```

### Production Deployment
1. **Follow Staging Model**
   - Coordinate with backoffice team
   - Use selective migration approach
   - Extensive testing before go-live

---

## 📊 Progress Tracking

### Completion Status
- [x] **Phase 1**: Entity Restructuring (100%)
- [x] **Phase 2**: Infrastructure Updates (100%) 
- [ ] **Phase 3**: Application Layer Migration (0% - Current Phase)
- [ ] **Phase 4**: Migration Scripts (75% - Scripts ready, testing pending)
- [ ] **Phase 5**: Testing & Validation (0%)

### Critical Path
```
Phase 3 (Fix Compilation) → Phase 5.1 (Unit Tests) → Phase 5.2 (Integration) → Deployment
```

### Estimated Time Remaining
- **Phase 3**: 6 hours (fixing 57 compilation errors)
- **Phase 4**: 2 hours (testing scripts)  
- **Phase 5**: 4 hours (comprehensive testing)
- **Total**: 12 hours (1.5 days)

---

## 🎯 Next Immediate Actions

### Priority 1: Fix AuthenticationService
1. Remove `IsActive` logic
2. Update `Name` → `Firstname`/`Lastname`  
3. Fix `Role` enum → `RoleId` comparisons
4. Remove `LastLoginAt` tracking

### Priority 2: Fix UserMappingProfile  
1. Update AutoMapper configurations
2. Add new field mappings
3. Remove obsolete field mappings

### Priority 3: Fix Other Services
1. Systematic fix of remaining 50+ errors
2. Update service interfaces
3. Fix DTOs and validation rules

**Goal**: Zero compilation errors within 6 hours, ready for testing phase.