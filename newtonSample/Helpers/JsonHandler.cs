using Newtonsoft.Json;

public class JsonHandler
{
    public static string SerializeWithCustomConverter(DateTime date)
    {
        return JsonConvert.SerializeObject(
            new { Date = date },
            new CustomDateTimeConverter()
        );
    }

    public static string SerializeProduct(Product product)
    {
        return JsonConvert.SerializeObject(product);
    }

    public static string SerializeAnimal(Animal animal)
    {
        return JsonConvert.SerializeObject(animal);
    }

    public static Animal DeserializeAnimal(string json)
    {
        return JsonConvert.DeserializeObject<Animal>(json);
    }
}