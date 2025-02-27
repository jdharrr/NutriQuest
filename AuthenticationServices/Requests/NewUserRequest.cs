namespace AuthenticationServices.Requests;

public class NewUserRequest
{
    public string? Name { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }
}
