using System.Security.Cryptography;
using Social.Contracts.Events.Identity;
using Social.Identity.Application.Abstractions.Auth;
using Social.Identity.Application.Abstractions.Cache;
using Social.Identity.Application.Errors;
using Social.Identity.Domain;
using Social.Shared.ResultType;
using Wolverine;

namespace Social.Identity.Features.CreateUser;

public sealed class CreateUserHandler(
    ICacheService cacheService,
    IMessageBus bus,
    IPasswordHasher passwordHasher)
{
    public async Task<Result> Handle(
        CreateUserCommand command,
        CancellationToken ct)
    {
        var cacheKey = $"temp_user:{command.Request.Email}";
        
        if (await cacheService.ExistsAsync(
                cacheKey, ct))
            return Result.Failure(
                UserErrors.RegistrationPending);
        
        var confirmationCode =
            RandomNumberGenerator
                .GetInt32(10000, 100000)
                .ToString();
        
        var tempUser = new TempUser
        {
            Id = Guid.CreateVersion7(),
            Username = command.Request.Username,
            Email = command.Request.Email,
            PasswordHash = passwordHasher.GenerateHash(
                command.Request.Password),
            CreatedAt = DateTime.UtcNow,
            ConfirmationCode = confirmationCode,
            CodeExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };
        
        await cacheService.SetAsync(
            cacheKey,
            tempUser,
            TimeSpan.FromMinutes(5),
            ct);
        
        await bus.PublishAsync(
            new UserStartRegistrationEvent(
                tempUser.Email,
                tempUser.Username,
                tempUser.ConfirmationCode));
        
        return Result.Success();
    }
}