using Social.Users.Application.DTOs;
using Social.Users.Domain;

namespace Social.Users.Application.Mappers;

public static class ProfileMapper
{
    public static ProfileResponse ToProfileResponse(
        this UserProfile profile,
        string? imageUrl)
        => new (
            profile.Id, 
            profile.Username,
            imageUrl);
}