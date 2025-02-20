namespace GeolocationServices.Requests;

public class NearbyStoresRequest
{
    public List<string> IncludedTypes { get; } = ["grocery_store"];

    public int MaxResultCount { get; } = 10;
    
    public required LocationRestriction LocationRestriction { get; set; }
}

public class LocationRestriction
{
    public required LocationCircle Circle { get; set; }
}

public class LocationCircle
{
    public required LocationCenter Center { get; set; }

    public double Radius { get; } = 24140.1;
}

public class LocationCenter
{
    public required double Latitude { get; set; }
    
    public required double Longitude { get; set; }
}