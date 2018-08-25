/*
    Repro program demonstrating EF Core issue with conversion of C# DateTime to SQL smalldatetime.
    In cases of '.Contains' usage or usage with constants DateTime values will not be correctly 
    translated to strings which SQL can parse as smalldatetime. 
    In some cases this works correctly.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFCoreBugRepro
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var reproContext = new ReproContext())
            {
                reproContext.Database.Migrate();
                var testDate = new DateTime(2018, 10, 07);

                // just to assert regular query is working fine
                {
                    var findASingleRecord = reproContext.ReproEntity
                        .FirstOrDefault(a => a.MyTime == testDate);
                    Console.WriteLine($"Found record is null: {findASingleRecord == null}.");
                }

                // now lets test the problematic query
                var testDateList = new List<DateTime>() { testDate };
                try
                {
                    // this will fail
                    var findRecordsWithDateInList = reproContext.ReproEntity
                        .Where(a => testDateList.Contains(a.MyTime))
                        .ToList();

                    Console.WriteLine($"Loaded {findRecordsWithDateInList.Count} entities from ReproEntity.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\nBUG? Query on ReproEntity failed: {e}\n");
                }

                // just to be sure that the problem is in 'smalldatetime' column type, we run the same code vs another entity with regular column type
                try
                {
                    // this won't fail
                    var findRecordsWithDateInList = reproContext.WorkingEntity
                        .Where(a =>testDateList.Contains(a.MyTime))
                        .ToList();

                    Console.WriteLine($"Loaded {findRecordsWithDateInList.Count} entities from WorkingEntity.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\nQuery on ReproEntity failed: {e}\n"); // this won't happen
                }

                // not sure why, but using constant DateTime in any kind of query will also trigger the same error
                // this is the same query as above but with constant parameter (no query parametrization)
                try
                {
                    var findASingleRecord = reproContext.ReproEntity
                        .FirstOrDefault(a => a.MyTime == new DateTime(2018, 10, 07));
                    Console.WriteLine($"Found record is null: {findASingleRecord == null}.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\nQuery which worked above also failed: {e}\n");
                }
            }
        }
    }
}
