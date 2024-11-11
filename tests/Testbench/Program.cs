using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using MessagePack;
using Vogen;
// ReSharper disable UnusedVariable

namespace Testbench;


[ValueObject<int>(conversions: Conversions.MessagePack)]
public partial class MyInt;

[MessagePack<MyInt>()]
public partial class MyMarkers;

public static class Program
{
    public static void Main()
    {
    }
}