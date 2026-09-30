using Microsoft.EntityFrameworkCore;
using Social.Identity.Application.Abstractions.Auth;
using Social.Identity.Application.Errors;
using Social.Identity.Infrastructure.Persistence.Database.Context;
using Social.Shared.ResultType;

namespace Social.Identity.Features.Login;

public sealed class LoginHandler(
    IdentityDbContext context,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider)
{
    public async Task<Result<LoginResponse>> Handle(
        LoginCommand command,
        CancellationToken ct)
    {
        var user = await context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => 
                x.Email == command.Request.Email, ct);

        if (user is null || !passwordHasher.VerifyHash(
                command.Request.Password, user.PasswordHash))
            return Result<LoginResponse>.Failure(
                UserErrors.NotFound);

        var token = jwtProvider.GenerateToken(user);

        return Result<LoginResponse>.Success(
            new LoginResponse(token));
    }
}