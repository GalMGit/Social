using System;

namespace Social.Communities.Domain;

public sealed class CommunityMember
{
    public Guid Id { get; set; }

    public Guid CommunityId { get; set; }
    public Community Community { get; set; } = null!;

    public Guid UserId { get; set; }

    public CommunityRole Role { get; set; }
}