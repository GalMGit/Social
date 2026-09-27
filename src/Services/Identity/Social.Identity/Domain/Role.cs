namespace Social.Identity.Domain;

public sealed class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public ICollection<Permission> Permissions { get; set; } = [];
}