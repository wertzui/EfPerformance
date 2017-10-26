using System;

namespace PerformanceTest
{
    internal class Counter
    {
        public long Value { get; private set; }
        public bool outputIncrementToConsole { get; set; }

        public void Increment(string sqlStatement)
        {
            if (outputIncrementToConsole)
                Console.WriteLine(sqlStatement);

            if (sqlStatement.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                Value++;
        }
    }
}