using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Threading;

namespace PerformanceTest
{
    public class EfSlowPerformanceSimulatorInterceptor : IDbCommandInterceptor
    {
        private const int latencyInMilliseconds = 50;

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            Thread.Sleep(latencyInMilliseconds);
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            Thread.Sleep(latencyInMilliseconds);
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            Thread.Sleep(latencyInMilliseconds);
        }
    }
}