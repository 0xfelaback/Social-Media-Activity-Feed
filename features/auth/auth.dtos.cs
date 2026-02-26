using System.ComponentModel.DataAnnotations;

public record AuthResponse
{
    public LoginResult loginResult { get; set; } = null!;
    public string accessToken { get; set; } = null!;
};

public record LoginRequest
{
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string ProvidedPassword { get; set; } = null!;
}

public record LoginResult
{
    public long userID;
    public string userName { get; set; } = null!;
    public string firstName { get; set; } = null!;
    public string lastName { get; set; } = null!;
    public string email { get; set; } = null!;
    [Phone]
    public string phoneNumber { get; set; } = null!;
    [Url]
    public string profileImage_MediaUrl { get; set; } = null!;
}

public record RegisterRequest
{
    public long userID;
    public string userName { get; set; } = null!;
    public string lastName { get; set; } = null!;
    public string firstName { get; set; } = null!;
    public string passwordHash { get; set; } = null!;
    public string email { get; set; } = null!;
    [Phone]
    public string phoneNumber { get; set; } = null!;
    [Url]
    public string profileImage_MediaUrl { get; set; } = null!;
}

public record EditUserDTO
{
    public string? userName { get; set; }
    public string? providedOldPassword { get; set; }
    public string? providedNewPassword { get; set; }
    public string? lastName { get; set; }
    public string? firstName { get; set; }
    public string? passwordHash { get; set; }
    public string? email { get; set; }
    [Phone]
    public string? phoneNumber { get; set; }
    [Url]
    public string? profileImage_MediaUrl { get; set; }
    [MaxLength(20)]
    public string? Gender { get; set; }
    public bool? PushNotifications { get; set; }
    public bool? AccountPrivacy { get; set; }
    public bool? Verified { get; set; }
    [Url(ErrorMessage = "Invalid URL Format")]
    public string? Website { get; set; }
}