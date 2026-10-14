using System;

namespace Social.Communities.Domain;

public sealed class Community
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid OwnerId { get; set; }
    public string? Description { get; set; }
    public ICollection<CommunityMember> Members { get; set; } = [];
}
