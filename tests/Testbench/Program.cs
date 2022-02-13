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


[assembly: VogenDefaults]

Console.WriteLine("!!");
var id = MyId.From(123);


[ValueObject(typeof(int))]
public partial struct MyId {}