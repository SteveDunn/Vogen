using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.Mapping;
using Microsoft.Data.Sqlite;
using Vogen.Examples.Types;

namespace Vogen.Examples.SerializationAndConversion
{
	public class LinqToDbExamples : IScenario
	{
		public Task Run()
		{
			LinqToDbValueConverterUsesValueConverter();
			return Task.CompletedTask;
		}

		private void LinqToDbValueConverterUsesValueConverter()
		{
			var connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();

			var original = new TestEntity { Id = LinqToDbStringVo.From("foo!") };
			using (var context = new TestDbContext(connection))
			{
				context.CreateTable<TestEntity>();
				context.Insert(original);
			}
			using (var context = new TestDbContext(connection))
			{
				var all = context.Entities.ToList();
				var retrieved = all.Single();

				Console.WriteLine(retrieved);
			}
		}

		public class TestDbContext : DataConnection
		{
			public ITable<TestEntity> Entities => GetTable<TestEntity>();

			public TestDbContext(SqliteConnection connection)
				: base(
					  SQLiteTools.GetDataProvider("SQLite.MS"),
					  connection,
					  disposeConnection: false)
			{ }
		}

		public class TestEntity
		{
			[PrimaryKey]
			[Column(DataType = DataType.VarChar)]
			[ValueConverter(ConverterType = typeof(LinqToDbStringVo.LinqToDbValueConverter))]
			public LinqToDbStringVo Id { get; set; }
		}
	}
}