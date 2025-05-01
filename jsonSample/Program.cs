using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// Middleware Pipeline - ORDER MATTERS!
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll"); // Apply CORS policy

app.UseHttpsRedirection();




var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
// Serialize
var person = new Person
{
    Name = "John Doe",
    Age = 30,
    BirthDate = new DateTime(1993, 5, 15),
    Hobbies = new List<string> { "Reading", "Hiking" }
};

//-----Controllers-----------

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");


//Basic Serialization/Deserialization

app.MapGet("/getperson", () =>
{
    string json = JsonSerializer.Serialize(person, new JsonSerializerOptions
        {
            WriteIndented = true, // Pretty print
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase // camelCase property names
        });

    return json;
})
.WithName("getperson");


// /Advanced Options
app.MapGet("/getadvperson", () =>
{
     var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        // Async serialization/deserialization
         using var stream = new MemoryStream();
         JsonSerializer.Serialize(stream, person, options);

         stream.Position = 0;
       var asyncDeserializedPerson = JsonSerializer.Deserialize<Person>(stream);

    return asyncDeserializedPerson;
})
.WithName("getadvperson");

// Custom Converters
app.MapGet("/getcustomeperson", () =>
{
     // Usage
        var customOptions = new JsonSerializerOptions
        {
            Converters = { new DateTimeOffsetConverter() }
        };

    return customOptions;
})
.WithName("getcustomeperson");



app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTimeOffset.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:sszzz"));
    }
}