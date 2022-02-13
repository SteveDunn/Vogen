using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Vogen;


[assembly: VogenDefaults(underlyingType: typeof(string), conversions: Conversions.None)]

var id = FooVo.From(new Bar(123, "aaa"));

Console.WriteLine($"{id.Value} ({id.Value.GetType()})");


public record struct Bar(int Age, string Name);

[ValueObject(conversions: Conversions.None, underlyingType: typeof(Bar))]
public partial class FooVo { }
