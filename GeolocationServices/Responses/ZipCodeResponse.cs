using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ZipCodeResponse
{
    [JsonPropertyName("results")]
    public ZipCodeResult[] Results { get; set; }
}

public class ZipCodeResult
{
    [JsonPropertyName("geometry")]
    public ZipCodeGeometry Geometry { get; set; }
}

public class ZipCodeGeometry
{
    [JsonPropertyName("location")]
    public ZipCodeLocation Location { get; set; }
}

public class ZipCodeLocation
{
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    [JsonPropertyName("lng")]
    public double Longitude { get; set; }
}