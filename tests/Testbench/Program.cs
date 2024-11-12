using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using MessagePack;
using N1;
using N2;
using Vogen;
// ReSharper disable UnusedVariable

namespace Testbench;

[MessagePack<MyInt>()]
[MessagePack<MyString>()]
[EfCoreConverter<MyInt>]
public partial class MyMarkers;

public static class Program
{
    public static void Main()
    {
    }
}