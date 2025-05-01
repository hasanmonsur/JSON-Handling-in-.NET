using System.Text.Json.Serialization;

public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    
    [JsonIgnore]
    public bool IsExpensive => Price > 100;
    
    public bool ShouldSerializePrice() => Price > 0;
}
