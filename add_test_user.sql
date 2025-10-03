-- Add test user for registration testing
INSERT INTO "Users" (
    "Email", 
    "Firstname", 
    "Lastname", 
    "MobilePhone", 
    "PasswordHash",
    "RoleId", 
    "CreatedAt", 
    "UpdatedAt",
    "IsDefaultAdmin"
) VALUES (
    'test@example.com',
    'Test',
    'User', 
    '+66891234567',
    '',
    2,
    NOW(),
    NOW(),
    false
);