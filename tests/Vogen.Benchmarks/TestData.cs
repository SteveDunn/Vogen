using System;

namespace Vogen.Benchmarks
{
    public class TestData
    {
        private static Random _random = new();

        public static string GetRandomString()
        {
            var c = new char[10];

            for (int i = 0; i < 10; i++)
            {
                c[i] = (char)_random.Next('a', 'z');
            }

            return new string(c.AsSpan());
        }

        public static int RandomNumberBetween(int s, int e) => _random.Next(s, e);
    }
}