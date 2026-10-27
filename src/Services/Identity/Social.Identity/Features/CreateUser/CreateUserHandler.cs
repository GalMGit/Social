using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Social.Contracts.Events.Identity;
using Social.Identity.Application.Abstractions.Auth;
using Social.Identity.Application.Abstractions.Cache;
using Social.Identity.Application.Errors;
using Social.Identity.Domain;
using Social.Identity.Infrastructure.Persistence.Database.Context;
using Social.Shared.ResultType;
using Wolverine;

namespace Social.Identity.Features.CreateUser;

public sealed class CreateUserHandler(
    IdentityDbContext context,
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
        
        var emailExists = await context.Users
            .AnyAsync(x => 
                x.Email == command.Request.Email, ct);

        if (emailExists)
            return Result.Failure(
                UserErrors.EmailExists);

        var usernameExists = await context.Users
            .AnyAsync(x => 
                x.Username == command.Request.Username, ct);
        
        if (usernameExists)
            return Result.Failure(
                UserErrors.UsernameExists);
        
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