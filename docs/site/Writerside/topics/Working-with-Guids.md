# Working with IDs

This tutorial takes you through working with value objects which represent IDs. You should follow this tutorial if you want to customize how IDs are generated. It describes how to work with sequential Guids which are optimised for use in SQL server.  

**1. Create a new value object based on a Guid**
```c#
[ValueObject<Guid>]
public partial struct CustomerId;

[ValueObject<Guid>]
public partial struct SupplierId;
```

**2. Tell Vogen to generate the `IVogen` interface**

Add this assembly level attribute:

```c#
[assembly: VogenDefaults(
   staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon)]
```

`MostCommon` specifies the generation of equal operators, explicit casts, and the `From` and `TryFrom` methods. [See the source code](https://github.com/SteveDunn/Vogen/blob/main/src/Vogen.SharedTypes/StaticAbstractsGeneration.cs) for other options.

This interface that is generated has static abstract methods (which are part of C# 11), and allows us to hook into the creation of IDs.

The interface looks something like this:
```c#
public interface IVogen<TSelf, TPrimitive> where TSelf 
   : IVogen<TSelf, TPrimitive>
{
    static abstract explicit operator TSelf(TPrimitive value);
    static abstract explicit operator TPrimitive(TSelf value);

    static abstract bool operator ==(TSelf left, TSelf right);
    static abstract bool operator !=(TSelf left, TSelf right);

    static abstract bool operator ==(TSelf left, TPrimitive right);
    static abstract bool operator !=(TSelf left, TPrimitive right);

    static abstract bool operator ==(TPrimitive left, TSelf right);
    static abstract bool operator !=(TPrimitive left, TSelf right);

    static abstract TSelf From(TPrimitive value);
    static abstract bool TryFrom(TPrimitive value, out TSelf vo);
}
```

**3. Create a factory that generates sequential Guids**

Add the following code. It is a factory that creates sequential Guids on value objects that wrap `System.Guid`:

```c#
 public static class GuidFactory<TSelf> 
    where TSelf : IVogen<TSelf, Guid>
 {
     // ReSharper disable once StaticMemberInGenericType
     private static long _counter = DateTime.UtcNow.Ticks;

     static TSelf NewSequential()
     {
         var guidBytes = Guid.NewGuid().ToByteArray();
         
         var counterBytes = BitConverter.GetBytes(
            Interlocked.Increment(ref _counter));

         if (!BitConverter.IsLittleEndian)
         {
             Array.Reverse(counterBytes);
         }

         guidBytes[08] = counterBytes[1];
         guidBytes[09] = counterBytes[0];
         guidBytes[10] = counterBytes[7];
         guidBytes[11] = counterBytes[6];
         guidBytes[12] = counterBytes[5];
         guidBytes[13] = counterBytes[4];
         guidBytes[14] = counterBytes[3];
         guidBytes[15] = counterBytes[2];

         return TSelf.From(new Guid(guidBytes));
     }
 }
```

> _the code to generate these sequential Guids was borrowed from the EFCore repository_

This factory generates value objects, but only value objects that wrap Guids. It doesn't know any specific value object type, just that they wrap a Guid.

**4. Call the factory to create sequential Guids**

The code below demonstrates how to use this factory. It creates two IDs, pauses (to demonstrate the time-based nature of sequential Guids), and then creates two more:

```c#
var id1 = GuidFactory<CustomerId>.NewSequential();
var id2 = GuidFactory<CustomerId>.NewSequential();

await Task.Delay(TimeSpan.FromMicroseconds(50));

var id3 = GuidFactory<SupplierId>.NewSequential();
var id4 = GuidFactory<SupplierId>.NewSequential();

Console.WriteLine($"Customer ID 1 = {id1}");
Console.WriteLine($"Customer ID 2 = {id2}");

Console.WriteLine($"Supplied ID 1 = {id3}");
Console.WriteLine($"Supplied ID 2 = {id4}");
```

This outputs something like:

```Bash
Customer ID 1 = 2930454c-9889-4c52-161d-08dca48c5b35
Customer ID 2 = 40cbec7e-280f-4bf7-161e-08dca48c5b35
Supplied ID 1 = 9684f1f5-2246-4b9c-20ff-08dca48c5b35
Supplied ID 2 = 92b19132-141d-4245-2100-08dca48c5b35
```

Notice where the delay happens in the generation.

## What you've learned {id="what-learned"}

We've learned how to tell Vogen to create the `IVogen` interface. We've seen that the interface has static abstract methods, and how a factory can use those methods to create value objects without knowing the exact type.

This source for this tutorial is the [samples project](https://github.com/SteveDunn/Vogen/blob/main/samples/Vogen.Examples/TypicalScenarios/GuidExamples.cs).