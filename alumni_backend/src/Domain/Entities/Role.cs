namespace Domain.Entities;

/// <summary>
/// Represents user roles in the system
/// Maps to the "Roles" table created by the backoffice team
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation Properties
    public ICollection<User> Users { get; set; } = new List<User>();

    // Constants for predefined roles
    public static class Names
    {
        public const string Member = "Member";
        public const string Admin = "Admin";
    }

    // Domain Methods
    public bool IsMember() => Name == Names.Member;
    public bool IsAdmin() => Name == Names.Admin;
}