using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class AuthExtension
{
    public static string GetUserEndpoint = "/api/users/{id}";

    public static void UseAuth(this WebApplication app)
    {
        app.MapPost("/api/register", async (RegisterRequest registerRequest, SocialMediaDataContext context, IPasswordHasher<string> passwordHasher) =>
                {
                    string passwordHash = passwordHasher.HashPassword(string.Empty, registerRequest.passwordHash);
                    User newUSer = new User
                    {
                        UserName = registerRequest.userName,
                        FirstName = registerRequest.firstName,
                        LastName = registerRequest.lastName,
                        PasswordHash = passwordHash,
                        Email = registerRequest.email,
                        PhoneNumber = registerRequest.phoneNumber,
                        FollowersCount = default,
                        FollowingCount = default,
                        AccountDeleted = default,
                        DeletedAt = null
                    };
                    newUSer.UserProfile = new UserProfile
                    {
                        User = newUSer,
                        PushNotifications = default,
                        AccountPrivacy = default,
                        Verified = default
                    };
                    context.Users.Add(newUSer);
                    await context.SaveChangesAsync();
                    //return Results.CreatedAtRoute(GetUserEndpoint, new { UserID = newUSer.UserID }, newUSer);
                    return Results.Created();
                });

        app.MapPost("/api/login", async (LoginRequest request, SocialMediaDataContext context, IPasswordHasher<string> passwordHasher, ITokenProvider tokenProvider) =>
        {
            User? user = await context.Users.Where(u => u.UserName == request.Username).FirstOrDefaultAsync();

            if (user is null) return Results.NotFound(new AuthResponse { loginResult = null, accessToken = string.Empty });
            var PasswordResult = passwordHasher.VerifyHashedPassword(string.Empty, user.PasswordHash, request.ProvidedPassword ?? string.Empty);
            bool isValid = PasswordResult != PasswordVerificationResult.Failed;
            if (!isValid) return Results.Unauthorized();

            string token = tokenProvider.Create(user);
            LoginResult resultData = new LoginResult { userID = user.UserID, userName = user.UserName, firstName = user.FirstName, lastName = user.LastName, email = user.Email ?? string.Empty, phoneNumber = user.PhoneNumber ?? string.Empty, profileImage_MediaUrl = user.ProfileImage_MediaUrl ?? string.Empty };
            AuthResponse responseData = new AuthResponse { loginResult = resultData, accessToken = token };
            return Results.Ok(responseData);
        });

        app.MapPut("/api/users/{username}", async (string username, EditUserDTO newUserDetails, SocialMediaDataContext context, IPasswordHasher<string> passwordHasher) =>
{
    User? User = await context.Users.Include(u => u.UserProfile).FirstOrDefaultAsync(u => u.UserName == username);
    if (User is null) return Results.NotFound("The user was not found");
    if (newUserDetails.userName != null && newUserDetails.userName != User.UserName)
    {
        bool userNameTaken = await context.Users.AnyAsync(u => u.UserName == newUserDetails.userName);
        if (userNameTaken) return Results.Conflict("Username is already taken");
        User.UserName = newUserDetails.userName;
    }
    if (newUserDetails.email != null && newUserDetails.email != User.Email)
    {
        bool emailTaken = await context.Users.AnyAsync(u => u.Email == newUserDetails.email);
        if (emailTaken) return Results.Conflict("Email is already taken");
        User.Email = newUserDetails.email;
    }
    if (newUserDetails.phoneNumber != null & newUserDetails.phoneNumber == User.PhoneNumber)
    {
        bool phoneNumberTaken = await context.Users.AnyAsync(u => u.PhoneNumber == newUserDetails.phoneNumber);
        if (phoneNumberTaken) return Results.Conflict("Phone Number is already taken");
        User.PhoneNumber = newUserDetails.phoneNumber;
    }
    if (newUserDetails.providedNewPassword != null)
    {
        var PasswordCheckResult = passwordHasher.VerifyHashedPassword(string.Empty, User.PasswordHash, newUserDetails.providedOldPassword ?? string.Empty);
        bool isValid = PasswordCheckResult != PasswordVerificationResult.Failed;
        if (!isValid) return Results.Unauthorized();
        string newPasswordhash = passwordHasher.HashPassword(string.Empty, newUserDetails.providedNewPassword);
        User.PasswordHash = newPasswordhash;
    }
    if (newUserDetails.lastName != null) User.LastName = newUserDetails.lastName;
    if (newUserDetails.firstName != null) User.FirstName = newUserDetails.firstName;
    if (newUserDetails.profileImage_MediaUrl != null) User.ProfileImage_MediaUrl = newUserDetails.profileImage_MediaUrl;
    if (User.UserProfile != null)
    {
        if (newUserDetails.Gender != null) User.UserProfile.Gender = newUserDetails.Gender;
        if (newUserDetails.PushNotifications != null) User.UserProfile.PushNotifications = newUserDetails.PushNotifications.GetValueOrDefault();
        if (newUserDetails.AccountPrivacy != null) User.UserProfile.AccountPrivacy = newUserDetails.AccountPrivacy.GetValueOrDefault();
        if (newUserDetails.Verified != null) User.UserProfile.Verified = (bool)newUserDetails.Verified;
        if (newUserDetails.Website != null) User.UserProfile.Website = newUserDetails.Website;
    }
    await context.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

        app.MapDelete("/api/users/{username}", async (string username, SocialMediaDataContext context) =>
{
    User? User = await context.Users.Where(u => u.UserName == username).FirstOrDefaultAsync();
    if (User is null) return Results.NotFound("The user was not found");

    context.Users.Remove(User);
    await context.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

    }
}