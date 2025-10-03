# Alumni Backend - Schema Migration & Integration Specification
**Spec ID**: 003-schema-migration-dev-staging  
**Created**: October 3, 2025  
**Status**: Planning  
**Priority**: High  

## 🎯 Overview

This specification outlines the implementation strategy for migrating from our current schema to the backoffice team's unified schema, ensuring seamless integration between development, staging, and production environments while maintaining data integrity and supporting both authentication systems and social features.

## 📋 Problem Statement

### Current Situation
- **Our Current Schema**: Uses separate `Name` field, `Role` enum, includes `IsActive`, `LastLoginAt`, etc.
- **Backoffice Schema**: Uses `Firstname`/`Lastname`, `RoleId` foreign key, includes member-specific fields
- **Integration Challenge**: Staging database already has `Users` and `Roles` tables with existing user data

### Key Challenges
1. **Schema Mismatch**: Different field structures between our entities and staging database
2. **Data Preservation**: Must not lose existing user data in staging
3. **Social Features**: Need to create additional tables for Posts, Comments, Likes, etc.
4. **Environment Consistency**: Development, staging, and production must work seamlessly
5. **Migration Strategy**: Different approaches needed for different environments

## 🎯 Goals

### Primary Objectives
1. **Seamless Integration**: Connect to staging database without data loss
2. **Schema Alignment**: Update our entities to match backoffice schema
3. **Selective Migration**: Apply migrations only where needed
4. **Environment Flexibility**: Support dev (fresh DB) and staging (existing data)
5. **Social Features**: Ensure all social platform features work with new schema

### Success Criteria
- ✅ Authentication works with existing staging users
- ✅ Social features (posts, comments, likes) function correctly
- ✅ No data loss in staging environment
- ✅ Development environment works with new schema
- ✅ All Application layer code compiles and functions

## 📊 Schema Mapping

### Current Schema → New Schema (Backoffice)

| Current Field | New Field | Type | Notes |
|---------------|-----------|------|-------|
| `Name` | `Firstname` + `Lastname` | Split into two fields | |
| `Role` (enum) | `RoleId` + `Role` (navigation) | Foreign Key | References Roles table |
| `IsActive` | *Removed* | - | No longer needed |
| `LastLoginAt` | *Removed* | - | No longer tracked |
| `Provider`/`ProviderId` | *Removed* | - | OAuth not in current scope |
| - | `MemberID` | string? | New: Member identification |
| - | `NameInYearbook` | string? | New: Name as appears in yearbook |
| - | `GroupID` | string? | New: Member group classification |
| - | `IsDefaultAdmin` | bool | New: Default admin flag |

### Database Tables Status

#### ✋ **DO NOT MIGRATE** (Exist in Staging)
- `Users` - Contains existing user data with BCrypt passwords
- `Roles` - Contains predefined roles (Member=1, Admin=2)

#### ✅ **MUST MIGRATE** (Missing in Staging)
- `AlumniProfiles` - Extended alumni information
- `Posts` - Social media posts
- `Comments` - Post comments and replies
- `Likes` - User engagement tracking
- `Reports` - Content moderation
- `AuditLogs` - System audit trail

## 🏗️ Implementation Strategy

### Phase 1: Entity Restructuring
#### 1.1 Update Domain Entities (2 hours)
- [x] Create `Role` entity with proper relationships
- [x] Update `User` entity to match backoffice schema
- [x] Maintain social feature entities (Post, Comment, Like, etc.)
- [x] Update navigation properties and relationships

#### 1.2 Configuration Updates (1 hour)
- [x] Create `RoleConfiguration` with seed data
- [x] Update `UserConfiguration` for new schema
- [x] Add proper indexes and constraints
- [x] Configure foreign key relationships

### Phase 2: Infrastructure Updates
#### 2.1 Repository Updates (2 hours)
- [x] Update `UserRepository` for new fields
- [x] Add role-based query methods
- [x] Update search functionality
- [ ] Fix interface compatibility issues

#### 2.2 Database Context (1 hour)
- [x] Add `DbSet<Role>` to AppDbContext
- [x] Update OnModelCreating configuration
- [x] Remove enum conversions

### Phase 3: Application Layer Migration
#### 3.1 Service Layer Updates (4 hours)
- [ ] Fix `AuthenticationService` compilation errors
- [ ] Update `UserService` for new schema
- [ ] Fix `ReportService`, `PostService`, `CommentService`
- [ ] Update mapping profiles (AutoMapper)

#### 3.2 DTOs and Interfaces (2 hours)
- [ ] Update `UserDto` and related DTOs
- [ ] Fix `IUserRepository` interface
- [ ] Update validation rules
- [ ] Fix controller endpoints

### Phase 4: Migration Strategy Implementation
#### 4.1 Environment-Specific Logic (2 hours)
- [x] Create conditional migration in `Program.cs`
- [x] Implement staging detection
- [x] Create missing tables SQL script

#### 4.2 Migration Scripts (2 hours)
- [x] `create_missing_tables.sql` - For staging deployment
- [ ] Development migration cleanup
- [ ] Production deployment scripts

### Phase 5: Testing & Validation
#### 5.1 Development Testing (2 hours)
- [ ] Test fresh database creation
- [ ] Verify all entity mappings
- [ ] Test authentication flow
- [ ] Validate social features

#### 5.2 Staging Integration (3 hours)
- [ ] Connect to staging database
- [ ] Run missing tables script
- [ ] Test with existing user data
- [ ] Verify BCrypt password compatibility
- [ ] End-to-end feature testing

## 📋 Technical Requirements

### Environment Configuration
```json
// appsettings.Staging.json
{
  "Database": {
    "UsePostgreSQL": true
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=staging-host;Port=5432;Database=staging_db;Username=staging_user;Password=***"
  },
  "Migration": {
    "Strategy": "SelectiveOnly",  // Don't migrate Users/Roles
    "CheckExistingTables": true   // Verify existing schema
  }
}
```

### Deployment Strategy
1. **Development Environment**:
   - Clean slate migrations
   - Full schema creation
   - Test data seeding

2. **Staging Environment**:
   - Connect to existing database
   - Run `create_missing_tables.sql`
   - Verify table compatibility
   - No Users/Roles migration

3. **Production Environment**:
   - Follow staging model
   - Additional backup procedures
   - Rollback strategy

## 🔧 Migration Scripts

### For Staging (Selective Migration)
```sql
-- create_missing_tables.sql
-- Creates only missing social feature tables
-- Assumes Users and Roles tables exist with data
```

### For Development (Full Migration)
```bash
# Clean migrations and recreate
dotnet ef migrations remove --force
dotnet ef migrations add UnifiedSchemaImplementation
dotnet ef database update
```

## 🧪 Testing Strategy

### Unit Tests
- [ ] Entity mapping validations
- [ ] Repository method compatibility
- [ ] Service layer functionality
- [ ] Authentication with new schema

### Integration Tests
- [ ] Database connection and queries
- [ ] API endpoint functionality
- [ ] Cross-table relationships
- [ ] Performance with new indexes

### Staging Tests
- [ ] Existing user authentication
- [ ] Social features with real data
- [ ] Admin functionality
- [ ] Member profile management

## 📊 Validation Checklist

### Pre-Migration
- [ ] Backup existing migrations
- [ ] Document current schema
- [ ] Test connection to staging DB
- [ ] Verify existing data structure

### Post-Migration
- [ ] All entities compile successfully
- [ ] Database schema matches expectations  
- [ ] Authentication works with existing users
- [ ] Social features are functional
- [ ] Admin panel accessibility
- [ ] API endpoints return correct data

### Staging Verification
- [ ] Connect without errors
- [ ] Login with existing admin account
- [ ] Create and view posts
- [ ] Member profile functionality
- [ ] Data integrity maintained

## 🚨 Risk Mitigation

### High Risks
1. **Data Loss in Staging**
   - Mitigation: Never run full migrations on staging
   - Use selective table creation only

2. **Authentication Failure**
   - Mitigation: Verify BCrypt compatibility
   - Test with known user credentials

3. **Schema Incompatibility**
   - Mitigation: Thorough entity mapping validation
   - Cross-reference with backoffice team

### Medium Risks
1. **Performance Issues**
   - Mitigation: Proper indexing strategy
   - Query optimization testing

2. **Feature Regression**
   - Mitigation: Comprehensive testing suite
   - Rollback procedures

## 📈 Success Metrics

### Technical Metrics
- Zero compilation errors
- All tests passing
- Database connection success rate: 100%
- API response time < 200ms

### Functional Metrics  
- User authentication success rate: 100%
- Social feature functionality: 100%
- Data integrity: No data corruption
- Admin operations: Full functionality

## 🗓️ Timeline

| Phase | Duration | Dependencies |
|-------|----------|--------------|
| Phase 1: Entity Updates | 3 hours | None |
| Phase 2: Infrastructure | 3 hours | Phase 1 complete |
| Phase 3: Application Layer | 6 hours | Phase 2 complete |
| Phase 4: Migration Strategy | 4 hours | Phase 3 complete |
| Phase 5: Testing | 5 hours | Phase 4 complete |
| **Total** | **21 hours** | **3 days** |

## 🔗 Related Documents
- [001-alumni-backend-api/spec.md](../001-alumni-backend-api/spec.md) - Original backend specification
- [002-mobile-login-external-data/spec.md](../002-mobile-login-external-data/spec.md) - External data integration
- [STAGING_DEPLOYMENT_GUIDE.md](../../STAGING_DEPLOYMENT_GUIDE.md) - Deployment procedures

## 👥 Stakeholders
- **Development Team**: Implementation and testing
- **Backoffice Team**: Schema coordination and staging access
- **DevOps Team**: Deployment and environment management

---

**Next Steps**: Begin Phase 3 (Application Layer Migration) to resolve compilation errors and ensure full system functionality.