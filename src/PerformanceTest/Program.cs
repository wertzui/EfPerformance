using EfPerformance;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PerformanceTest
{
    internal class Program
    {
        private static void Main()
        {
            Init();

            _1_IQueryable();
            //_2_IEnumerable();
            //_3_ToList();
            //_4_Include();
            //_5_Multiple();
            //_6_MultipleCorrect();
            //_7_OwnContext();
            //_8_Async().Wait();
            //_9_ParallelAsync().Wait();
            //_10_CorrectParallelAsync().Wait();

            Console.ReadLine();
        }

        private static async Task DoEpicShit()
        {
            var sw1 = new Stopwatch();
            var sw2 = new Stopwatch();
            var counter = CreateCounter();

            using (var context = GetContext(counter))
            {
                // Get Data
                sw1.Start();
                for (int i = 0; i < 10; i++)
                {

                var customersTask = GetFullSetAsync<Customer>(counter, c => c.CustomerID);
                var headersTasks = GetFullSetAsync<SalesOrderHeader>(counter, h => h.SalesOrderID);
                var detailsTask = GetFullSetAsync<SalesOrderDetail>(counter, d => d.SalesOrderDetailID);
                var productsTask = GetFullSetAsync<Product>(counter, p => p.ProductID);

                await Task.WhenAll(customersTask, headersTasks, detailsTask, productsTask).ConfigureAwait(false);

                var customers = customersTask.Result;
                var headers = headersTasks.Result;
                var details = detailsTask.Result;
                var products = productsTask.Result;

                RelationshipFixup(customers, headers, details, products);
                }
                sw1.Stop();

                // Output Data
                sw2.Start();
                //WriteSumToConsole(customers.Values);
                sw2.Stop();

                // Output Statistics
                WriteStatisticsToConsole(sw1, sw2, counter);
            }

        }

        private static void _1_IQueryable()
        {
            Console.WriteLine(nameof(_1_IQueryable));
            Measure(counter =>
                GetContext(counter).Customer,
                WriteSumToConsole);
        }

        private static void _2_IEnumerable()
        {
            Console.WriteLine(nameof(_2_IEnumerable));
            Measure(counter =>
                GetContext(counter).Customer
                .AsEnumerable(),
                WriteSumToConsole);
        }

        private static void _3_ToList()
        {
            Console.WriteLine(nameof(_3_ToList));
            Measure(counter =>
                GetContext(counter).Customer
                .ToList(),
                WriteSumToConsole);
        }

        private static void _4_Include()
        {
            Console.WriteLine(nameof(_4_Include));
            Measure(counter =>
                GetContext(counter).Customer
                .Include(c => c.SalesOrderHeader.Select(h => h.SalesOrderDetail.Select(d => d.Product)))
                .ToList(),
                WriteSumToConsole);
        }

        private static void _5_Multiple()
        {
            Console.WriteLine(nameof(_5_Multiple));
            Measure(counter =>
                {
                    var context = GetContext(counter);
                    var customers = GetFullSet<Customer>(context, c => c.CustomerID);
                    var headers = GetFullSet<SalesOrderHeader>(context, h => h.SalesOrderID);
                    var details = GetFullSet<SalesOrderDetail>(context, d => d.SalesOrderDetailID);
                    var products = GetFullSet<Product>(context, p => p.ProductID);

                    RelationshipFixup(customers, headers, details, products);

                    return customers.Values;
                },
                WriteSumToConsole);
        }

        private static void _6_MultipleCorrect()
        {
            Console.WriteLine(nameof(_6_MultipleCorrect));
            Measure(counter =>
            {
                using (var context = GetContextForGet(counter))
                {
                    var customers = GetFullSet<Customer>(context, c => c.CustomerID);
                    var headers = GetFullSet<SalesOrderHeader>(context, h => h.SalesOrderID);
                    var details = GetFullSet<SalesOrderDetail>(context, d => d.SalesOrderDetailID);
                    var products = GetFullSet<Product>(context, p => p.ProductID);

                    RelationshipFixup(customers, headers, details, products);

                    return customers.Values;
                }
            },
                WriteSumToConsole);
        }

        private static void _7_OwnContext()
        {
            Console.WriteLine(nameof(_7_OwnContext));
            Measure(counter =>
            {
                var customers = GetFullSet<Customer>(counter, c => c.CustomerID);
                var headers = GetFullSet<SalesOrderHeader>(counter, h => h.SalesOrderID);
                var details = GetFullSet<SalesOrderDetail>(counter, d => d.SalesOrderDetailID);
                var products = GetFullSet<Product>(counter, p => p.ProductID);

                RelationshipFixup(customers, headers, details, products);

                return customers.Values;
            },
                WriteSumToConsole);
        }

        private static async Task _8_Async()
        {
            Console.WriteLine(nameof(_8_Async));
            await MeasureAsync(async counter =>
            {
                var customers = await GetFullSetAsync<Customer>(counter, c => c.CustomerID);
                var headers = await GetFullSetAsync<SalesOrderHeader>(counter, h => h.SalesOrderID);
                var details = await GetFullSetAsync<SalesOrderDetail>(counter, d => d.SalesOrderDetailID);
                var products = await GetFullSetAsync<Product>(counter, p => p.ProductID);

                RelationshipFixup(customers, headers, details, products);

                return customers.Values;
            },
                WriteSumToConsole);
        }

        private static async Task _9_ParallelAsync()
        {
            Console.WriteLine(nameof(_9_ParallelAsync));
            await MeasureAsync(async counter =>
            {
                var customersTask = GetFullSetAsync<Customer>(counter, c => c.CustomerID);
                var headersTask = GetFullSetAsync<SalesOrderHeader>(counter, h => h.SalesOrderID);
                var detailsTask = GetFullSetAsync<SalesOrderDetail>(counter, d => d.SalesOrderDetailID);
                var productsTask = GetFullSetAsync<Product>(counter, p => p.ProductID);

                await Task.WhenAll(customersTask, headersTask, detailsTask, productsTask);

                var customers = customersTask.Result;
                var headers = headersTask.Result;
                var details = detailsTask.Result;
                var products = productsTask.Result;

                RelationshipFixup(customers, headers, details, products);

                return customers.Values;
            },
                WriteSumToConsole);
        }

        private static Task _10_CorrectParallelAsync()
        {
            Console.WriteLine(nameof(_10_CorrectParallelAsync));
            return MeasureAsync(async counter =>
            {
                var customersTask = GetFullSetAsync<Customer>(counter, c => c.CustomerID);
                var headersTask = GetFullSetAsync<SalesOrderHeader>(counter, h => h.SalesOrderID);
                var detailsTask = GetFullSetAsync<SalesOrderDetail>(counter, d => d.SalesOrderDetailID);
                var productsTask = GetFullSetAsync<Product>(counter, p => p.ProductID);

                await Task.WhenAll(customersTask, headersTask, detailsTask, productsTask).ConfigureAwait(false);

                var customers = customersTask.Result;
                var headers = headersTask.Result;
                var details = detailsTask.Result;
                var products = productsTask.Result;

                RelationshipFixup(customers, headers, details, products);

                return customers.Values;
            },
                WriteSumToConsole);
        }

        #region Getter

        private static IDictionary<int, TEntity> GetFullSet<TEntity>(EfPerformanceContext context, Func<TEntity, int> idSelector)
            where TEntity : class
        {
            return context.Set<TEntity>()
                .ToDictionary(idSelector);
        }

        private static IDictionary<int, TEntity> GetFullSet<TEntity>(Counter counter, Func<TEntity, int> idSelector)
            where TEntity : class
        {
            using (var context = GetContextForGet(counter))
            {
                return context.Set<TEntity>()
                    .ToDictionary(idSelector);
            }
        }

        private static async Task<IDictionary<int, TEntity>> GetFullSetAsync<TEntity>(Counter counter, Func<TEntity, int> idSelector)
            where TEntity : class
        {
            using (var context = GetContextForGet(counter))
            {
                return await context.Set<TEntity>()
                    .ToDictionaryAsync(idSelector);
            }
        }

        #endregion Getter

        #region Helpers

        private static void Measure<TEntity>(Func<Counter, TEntity> getFunction, Action<TEntity> outputFunction)
        {
            if (getFunction == null)
                throw new ArgumentNullException(nameof(getFunction));
            if (outputFunction == null)
                throw new ArgumentNullException(nameof(outputFunction));

            var sw1 = new System.Diagnostics.Stopwatch();
            var sw2 = new System.Diagnostics.Stopwatch();
            var counter = CreateCounter();

            sw1.Start();
            var value = getFunction(counter);
            sw1.Stop();

            sw2.Start();
            outputFunction(value);
            sw2.Stop();

            WriteStatisticsToConsole(sw1, sw2, counter);
        }

        private static async Task MeasureAsync<TEntity>(Func<Counter, Task<TEntity>> getFunction, Action<TEntity> outputFunction)
        {
            if (getFunction == null)
                throw new ArgumentNullException(nameof(getFunction));
            if (outputFunction == null)
                throw new ArgumentNullException(nameof(outputFunction));

            var sw1 = new System.Diagnostics.Stopwatch();
            var sw2 = new System.Diagnostics.Stopwatch();
            var counter = CreateCounter();

            sw1.Start();
            var value = await getFunction(counter).ConfigureAwait(false);
            sw1.Stop();

            sw2.Start();
            outputFunction(value);
            sw2.Stop();

            WriteStatisticsToConsole(sw1, sw2, counter);
        }

        private static Counter CreateCounter()
        {
            return new Counter { outputIncrementToConsole = false };
        }

        private static void WriteStatisticsToConsole(System.Diagnostics.Stopwatch sw1, System.Diagnostics.Stopwatch sw2, Counter counter)
        {
            Console.WriteLine($"Used {counter.Value} SQL SELECT Statements");
            Console.WriteLine($"Get took {sw1.ElapsedMilliseconds / 10}ms");
            Console.WriteLine($"Output took {sw2.ElapsedMilliseconds}ms");
            Console.WriteLine();
            Console.WriteLine();
        }

        private static EfPerformanceContext GetContext(Counter c)
        {
            var context = new EfPerformanceContext();

            context.Database.Log = c.Increment;

            return context;
        }

        private static EfPerformanceContext GetContextForGet(Counter c)
        {
            var context = GetContext(c);

            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.ProxyCreationEnabled = false;

            return context;
        }

        private static void Init()
        {
            using (var context = new EfPerformanceContext())
            {
                context.ProductCategory.Count();
            }
        }

        private static void RelationshipFixup(IDictionary<int, Customer> customers, IDictionary<int, SalesOrderHeader> headers, IDictionary<int, SalesOrderDetail> details, IDictionary<int, Product> products)
        {
            foreach (var header in headers.Values)
            {
                header.Customer = customers[header.CustomerID];
                header.Customer.SalesOrderHeader.Add(header);
            }

            foreach (var detail in details.Values)
            {
                detail.SalesOrderHeader = headers[detail.SalesOrderID];
                detail.SalesOrderHeader.SalesOrderDetail.Add(detail);

                detail.Product = products[detail.ProductID];
                detail.Product.SalesOrderDetail.Add(detail);
            }
        }

        private static void WriteToConsole(IQueryable<Customer> customers)
        {
            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.FirstName} {customer.LastName}");
                foreach (var salesOrderHeader in customer.SalesOrderHeader)
                {
                    Console.WriteLine($"- OrderNumber {salesOrderHeader.PurchaseOrderNumber}");
                    foreach (var detail in salesOrderHeader.SalesOrderDetail)
                    {
                        Console.WriteLine($"-- Product {detail.Product.Name}");
                    }
                }
            }
        }

        private static void WriteToConsole(IEnumerable<Customer> customers)
        {
            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.FirstName} {customer.LastName}");
                foreach (var salesOrderHeader in customer.SalesOrderHeader)
                {
                    Console.WriteLine($"- OrderNumber {salesOrderHeader.PurchaseOrderNumber}");
                    foreach (var detail in salesOrderHeader.SalesOrderDetail)
                    {
                        Console.WriteLine($"-- Product {detail.Product.Name}");
                    }
                }
            }
        }

        private static void WriteSumToConsole(IQueryable<Customer> customers)
        {
            var sum = 0m;
            foreach (var customer in customers)
            {
                foreach (var salesOrderHeader in customer.SalesOrderHeader)
                {
                    foreach (var detail in salesOrderHeader.SalesOrderDetail)
                    {
                        sum += detail.Product.ListPrice;
                    }
                }
            }
            Console.WriteLine($"Sum is {sum:N2}");
        }

        private static void WriteSumToConsole(IEnumerable<Customer> customers)
        {
            var sum = 0m;
            foreach (var customer in customers)
            {
                foreach (var salesOrderHeader in customer.SalesOrderHeader)
                {
                    foreach (var detail in salesOrderHeader.SalesOrderDetail)
                    {
                        sum += detail.Product.ListPrice;
                    }
                }
            }
            Console.WriteLine($"Sum is {sum:N2}");
        }

        #endregion Helpers
    }
}