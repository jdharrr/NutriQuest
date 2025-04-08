namespace DatabaseServices.Responses;

public class BulkWriteResponse
{
    public required long MatchedCount { get; set; }

    public required long ModifiedCount { get; set; }
}
