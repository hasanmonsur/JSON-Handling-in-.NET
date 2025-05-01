// Polymorphic serialization

using Newtonsoft.Json;
using JsonSubTypes; // Add this namespace

[JsonConverter(typeof(JsonSubtypes), "Type")]
[JsonSubtypes.KnownSubType(typeof(Cat), "cat")]
[JsonSubtypes.KnownSubType(typeof(Dog), "dog")]
public abstract class Animal
{
    public string Type { get; set; }
}

public class Cat : Animal { public bool LovesCatnip { get; set; } }
public class Dog : Animal { public bool ChasesTail { get; set; } }