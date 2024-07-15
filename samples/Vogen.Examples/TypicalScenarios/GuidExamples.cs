using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vogen.Examples.TypicalScenarios.GuidExamples
{
    /// <summary>
    ///     Generates sequential <see cref="Guid" /> values optimized for use in Microsoft SQL server clustered
    ///     keys or indexes, yielding better performance than random values. This is the default generator for
    ///     SQL Server <see cref="Guid" /> columns which are set to be generated on add.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Although this generator achieves the same goals as SQL Server's NEWSEQUENTIALID, the algorithm used
    ///         to generate the GUIDs is different. See
    ///         <see href="https://docs.microsoft.com/sql/t-sql/functions/newsequentialid-transact-sql">NEWSEQUENTIALID</see>
    ///         for more information on the advantages of sequential GUIDs.
    ///     </para>
    /// </remarks>
    public static class GuidFactory<TSelf> where TSelf : IVogen<TSelf, Guid>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static long _counter = DateTime.UtcNow.Ticks;

        public static TSelf NewSequential()
        {
            var guidBytes = Guid.NewGuid().ToByteArray();
            var counterBytes = BitConverter.GetBytes(Interlocked.Increment(ref _counter));

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


    
    [ValueObject<Guid>]
    public partial class CustomerId;

    [ValueObject<Guid>]
    public partial class SupplierId;

    [UsedImplicitly]
    internal class GuidExample : IScenario
    {
        public async Task Run()
        {
            var id1 = GuidFactory<CustomerId>.NewSequential();
            var id2 = GuidFactory<CustomerId>.NewSequential();

            await Task.Delay(TimeSpan.FromMicroseconds(50));
            
            var id3 = GuidFactory<SupplierId>.NewSequential();
            var id4 = GuidFactory<SupplierId>.NewSequential();
            
            Console.WriteLine($"Customer ID 1 = {id1}");
            Console.WriteLine($"Customer ID 2 = {id2}");

            Console.WriteLine($"Supplied ID 1 = {id3}");
            Console.WriteLine($"Supplied ID 2 = {id4}");
            
        }
    }
}