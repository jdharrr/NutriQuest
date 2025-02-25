namespace AuthenticationServices.Requests;

public class ChangePasswordRequest
{
    public required string UserId { get; set; }

    public required string CurrentPassword { get; set; }

    public required string NewPassword { get; set; }
}
