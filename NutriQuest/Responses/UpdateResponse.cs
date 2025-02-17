namespace DatabaseService.Responses;

public class UpdateResponse
{
    public required long MatchedCount { get; set; }

    public required long ModifiedCount { get; set; }
}
