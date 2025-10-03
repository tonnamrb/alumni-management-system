-- ============================================================================
-- SQL Script สำหรับสร้าง Tables ที่ขาดหายไปใน Staging Database
-- ใช้เมื่อ Staging มี Users และ Roles แล้ว แต่ขาด social features tables
-- ============================================================================

-- ตรวจสอบว่า Users และ Roles tables มีอยู่แล้วหรือไม่
DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'Users') THEN
        RAISE EXCEPTION 'Users table ไม่พบ - กรุณาตรวจสอบ connection กับ staging database';
    END IF;
    
    IF NOT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'Roles') THEN
        RAISE EXCEPTION 'Roles table ไม่พบ - กรุณาตรวจสอบ connection กับ staging database';
    END IF;
    
    RAISE NOTICE '✅ Users และ Roles tables พบแล้ว - เริ่มสร้าง missing tables';
END $$;

-- ============================================================================
-- 1. AlumniProfiles Table
-- ============================================================================
CREATE TABLE IF NOT EXISTS "AlumniProfiles" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "ExternalMemberID" TEXT,
    "ExternalSystemId" TEXT,
    "ExternalDataLastSync" TIMESTAMP WITH TIME ZONE,
    "NameInYearbook" TEXT,
    "TitleCode" TEXT,
    "GroupCode" TEXT,
    "Bio" VARCHAR(1000),
    "ProfilePictureUrl" VARCHAR(500),
    "GraduationYear" VARCHAR(10),
    "Major" VARCHAR(255),
    "CurrentJobTitle" VARCHAR(255),
    "CurrentCompany" VARCHAR(255),
    "PhoneNumber" VARCHAR(20),
    "LinkedInProfile" VARCHAR(500),
    "IsProfilePublic" BOOLEAN NOT NULL DEFAULT false,
    "Email" TEXT,
    "Address" TEXT,
    "District" TEXT,
    "Province" TEXT,
    "Country" TEXT,
    "ZipCode" TEXT,
    "MobilePhone" TEXT,
    "NickName" TEXT,
    "Phone" TEXT,
    "LineID" TEXT,
    "Facebook" TEXT,
    "DateOfBirth" TIMESTAMP WITH TIME ZONE,
    "MaritalStatus" TEXT,
    "SpouseName" TEXT,
    "JobTitle" TEXT,
    "CompanyName" TEXT,
    "WorkAddress" TEXT,
    "IsVerified" BOOLEAN NOT NULL DEFAULT false,
    "ProfileCompletedAt" TIMESTAMP WITH TIME ZONE,
    "Status" TEXT,
    "Comment" TEXT,
    "ClassName" TEXT,
    "Firstname" TEXT,
    "Lastname" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Key to Users table
    CONSTRAINT "FK_AlumniProfiles_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

-- Indexes for AlumniProfiles
CREATE INDEX IF NOT EXISTS "IX_AlumniProfiles_GraduationYear" ON "AlumniProfiles"("GraduationYear");
CREATE INDEX IF NOT EXISTS "IX_AlumniProfiles_Major" ON "AlumniProfiles"("Major");
CREATE INDEX IF NOT EXISTS "IX_AlumniProfiles_IsProfilePublic" ON "AlumniProfiles"("IsProfilePublic");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_AlumniProfiles_UserId" ON "AlumniProfiles"("UserId");
CREATE INDEX IF NOT EXISTS "IX_AlumniProfiles_GraduationYear_Major" ON "AlumniProfiles"("GraduationYear", "Major");

-- ============================================================================
-- 2. Posts Table
-- ============================================================================
CREATE TABLE IF NOT EXISTS "Posts" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "Content" VARCHAR(2000) NOT NULL,
    "ImageUrl" VARCHAR(500),
    "Type" INTEGER NOT NULL DEFAULT 1,
    "MediaUrls" TEXT,
    "IsPinned" BOOLEAN NOT NULL DEFAULT false,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Key to Users table
    CONSTRAINT "FK_Posts_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

-- Indexes for Posts
CREATE INDEX IF NOT EXISTS "IX_Posts_CreatedAt" ON "Posts"("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Posts_IsPinned" ON "Posts"("IsPinned");
CREATE INDEX IF NOT EXISTS "IX_Posts_Type" ON "Posts"("Type");
CREATE INDEX IF NOT EXISTS "IX_Posts_UserId" ON "Posts"("UserId");
CREATE INDEX IF NOT EXISTS "IX_Posts_IsPinned_CreatedAt" ON "Posts"("IsPinned", "CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Posts_Type_CreatedAt" ON "Posts"("Type", "CreatedAt");

-- ============================================================================
-- 3. Comments Table
-- ============================================================================
CREATE TABLE IF NOT EXISTS "Comments" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "PostId" INTEGER NOT NULL,
    "ParentCommentId" INTEGER,
    "Content" VARCHAR(1000) NOT NULL,
    "MentionedUserIds" VARCHAR(1000),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Keys
    CONSTRAINT "FK_Comments_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Comments_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Comments_Comments_ParentCommentId" FOREIGN KEY ("ParentCommentId") REFERENCES "Comments"("Id") ON DELETE CASCADE
);

-- Indexes for Comments
CREATE INDEX IF NOT EXISTS "IX_Comments_CreatedAt" ON "Comments"("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Comments_ParentCommentId" ON "Comments"("ParentCommentId");
CREATE INDEX IF NOT EXISTS "IX_Comments_PostId" ON "Comments"("PostId");
CREATE INDEX IF NOT EXISTS "IX_Comments_UserId" ON "Comments"("UserId");

-- ============================================================================
-- 4. Likes Table
-- ============================================================================
CREATE TABLE IF NOT EXISTS "Likes" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "PostId" INTEGER NOT NULL,
    "CommentId" INTEGER,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Keys
    CONSTRAINT "FK_Likes_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Likes_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Likes_Comments_CommentId" FOREIGN KEY ("CommentId") REFERENCES "Comments"("Id") ON DELETE CASCADE
);

-- Indexes for Likes
CREATE INDEX IF NOT EXISTS "IX_Likes_CreatedAt" ON "Likes"("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Likes_PostId" ON "Likes"("PostId");
CREATE INDEX IF NOT EXISTS "IX_Likes_UserId" ON "Likes"("UserId");
CREATE INDEX IF NOT EXISTS "IX_Likes_CommentId" ON "Likes"("CommentId");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Likes_UserId_PostId" ON "Likes"("UserId", "PostId");

-- ============================================================================
-- 5. Reports Table  
-- ============================================================================
CREATE TABLE IF NOT EXISTS "Reports" (
    "Id" SERIAL PRIMARY KEY,
    "ReporterId" INTEGER NOT NULL,
    "Type" TEXT NOT NULL,
    "PostId" INTEGER,
    "CommentId" INTEGER,
    "Reason" VARCHAR(255) NOT NULL,
    "AdditionalDetails" VARCHAR(1000),
    "Status" TEXT NOT NULL,
    "ResolvedByUserId" INTEGER,
    "ResolutionNote" VARCHAR(1000),
    "ResolvedAt" TIMESTAMP WITH TIME ZONE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Keys
    CONSTRAINT "FK_Reports_Users_ReporterId" FOREIGN KEY ("ReporterId") REFERENCES "Users"("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Reports_Users_ResolvedByUserId" FOREIGN KEY ("ResolvedByUserId") REFERENCES "Users"("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Reports_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Reports_Comments_CommentId" FOREIGN KEY ("CommentId") REFERENCES "Comments"("Id") ON DELETE CASCADE
);

-- Indexes for Reports
CREATE INDEX IF NOT EXISTS "IX_Reports_CommentId" ON "Reports"("CommentId");
CREATE INDEX IF NOT EXISTS "IX_Reports_CreatedAt" ON "Reports"("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Reports_PostId" ON "Reports"("PostId");
CREATE INDEX IF NOT EXISTS "IX_Reports_ReporterId" ON "Reports"("ReporterId");
CREATE INDEX IF NOT EXISTS "IX_Reports_ResolvedByUserId" ON "Reports"("ResolvedByUserId");
CREATE INDEX IF NOT EXISTS "IX_Reports_Status" ON "Reports"("Status");
CREATE INDEX IF NOT EXISTS "IX_Reports_Type" ON "Reports"("Type");
CREATE INDEX IF NOT EXISTS "IX_Reports_Status_CreatedAt" ON "Reports"("Status", "CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Reports_Type_Status" ON "Reports"("Type", "Status");

-- ============================================================================
-- 6. AuditLogs Table
-- ============================================================================
CREATE TABLE IF NOT EXISTS "AuditLogs" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER,
    "Action" TEXT NOT NULL,
    "EntityType" VARCHAR(100) NOT NULL,
    "EntityId" INTEGER NOT NULL,
    "OldValues" JSONB,
    "NewValues" JSONB,
    "Timestamp" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "IpAddress" VARCHAR(45),
    "UserAgent" VARCHAR(1000),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign Key
    CONSTRAINT "FK_AuditLogs_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE SET NULL
);

-- Indexes for AuditLogs
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_Action" ON "AuditLogs"("Action");
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_EntityId" ON "AuditLogs"("EntityId");
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_EntityType" ON "AuditLogs"("EntityType");
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_Timestamp" ON "AuditLogs"("Timestamp");
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_UserId" ON "AuditLogs"("UserId");
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_Action_Timestamp" ON "AuditLogs"("Action", "Timestamp");
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_EntityType_EntityId" ON "AuditLogs"("EntityType", "EntityId");
CREATE INDEX IF NOT EXISTS "IX_AuditLogs_UserId_Timestamp" ON "AuditLogs"("UserId", "Timestamp");

-- ============================================================================
-- 7. Auto-update timestamp triggers
-- ============================================================================
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW."UpdatedAt" = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Apply triggers to all tables
CREATE TRIGGER IF NOT EXISTS update_alumniprofiles_updated_at 
    BEFORE UPDATE ON "AlumniProfiles" 
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER IF NOT EXISTS update_posts_updated_at 
    BEFORE UPDATE ON "Posts" 
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER IF NOT EXISTS update_comments_updated_at 
    BEFORE UPDATE ON "Comments" 
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER IF NOT EXISTS update_likes_updated_at 
    BEFORE UPDATE ON "Likes" 
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER IF NOT EXISTS update_reports_updated_at 
    BEFORE UPDATE ON "Reports" 
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER IF NOT EXISTS update_auditlogs_updated_at 
    BEFORE UPDATE ON "AuditLogs" 
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- ============================================================================
-- สิ้นสุด Script
-- ============================================================================
SELECT 
    'Missing tables created successfully!' as status,
    count(*) as tables_created
FROM information_schema.tables 
WHERE table_name IN ('AlumniProfiles', 'Posts', 'Comments', 'Likes', 'Reports', 'AuditLogs');