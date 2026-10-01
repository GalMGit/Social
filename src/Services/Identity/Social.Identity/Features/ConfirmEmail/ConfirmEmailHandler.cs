using Microsoft.EntityFrameworkCore;
using Social.Contracts.Events.Identity;
using Social.Identity.Application.Abstractions.Cache;
using Social.Identity.Application.Errors;
using Social.Identity.Domain;
using Social.Identity.Infrastructure.Persistence.Database.Context;
using Social.Shared.Names;
using Social.Shared.ResultType;
using Wolverine;
using Wolverine.Attributes;

namespace Social.Identity.Features.ConfirmEmail;

[Transactional(typeof(IdentityDbContext))]
public sealed class ConfirmEmailHandler(
    IdentityDbContext context,
    ICacheService cacheService,
    IMessageBus bus)
{
    public async Task<Result> Handle(
        ConfirmEmailCommand command,
        CancellationToken ct)
    {
        var cacheKey = $"temp_user:{command.Request.Email}";
        
        var tempUser = await cacheService.GetAsync<TempUser>(
            cacheKey, ct);

        if (tempUser is null)
            return Result.Failure(
                UserErrors.ConfirmationCodeNotFound);

        if (tempUser.ConfirmationCode != command.Request.Code)
            return Result.Failure(
                UserErrors.ConfirmationCodeInvalid);

        if (tempUser.CodeExpiresAt < DateTime.UtcNow)
            return Result.Failure(
                UserErrors.ConfirmationCodeExpired);

        var emailExist = await context.Users
            .AnyAsync(x => 
                x.Email == tempUser.Email, ct);

        if (emailExist)
            return Result.Failure(
                UserErrors.EmailExists);
        
        var userRole = await context.Roles
            .SingleAsync(x =>
                x.Name == RoleNames.User, ct);

        var user = new User
        {
            Id = tempUser.Id,
            CreatedAt = tempUser.CreatedAt,
            Username = tempUser.Username,
            Email = tempUser.Email,
            PasswordHash = tempUser.PasswordHash,
            Roles = [userRole]
        };

        await context.Users.AddAsync(
            user, ct);

        await cacheService.RemoveAsync(
            cacheKey, ct);

        await bus.PublishAsync(
            new UserCreatedEvent(
                user.Id));

        return Result.Success();
    }
}