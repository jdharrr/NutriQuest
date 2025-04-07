namespace NutriQuestServices.UserServices.Responses;

public class SavedCartResponse
{
    public string CartId { get; set; }
    
    public DateTime Date { get; set; }
    
    public double TotalPrice { get; set; }
    
    public int NumberOfProducts { get; set; }
}