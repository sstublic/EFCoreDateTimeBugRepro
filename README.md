## Under some circumstances, query parameter of type System.DateTime is not correctly converted to match the column type that is defined as SQL 'smalldatetime'.

This results in **System.Data.SqlClient.SqlException (0x80131904): Conversion failed when converting character string to smalldatetime data type.**

## Two cases in which this is detected to happen are:
1. using List\<DateTime\>.Contains inside query
2. using DateTime constant inside query

In these cases DateTime value in actual SQL query is converted to e.g. '2018-10-07T00:00:00.0000000' instead of '2018-10-07 00:00:00', causing SQL server to fail to convert it to smalldatetime and throw exception.

## Trivial example:
Given 
```
public class ReproEntity
{
    public Guid Id { get; set; }
    public DateTime MyTime { get; set; }
}
```
and
```
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<ReproEntity>(e =>
        e.Property("MyTime").HasColumnType("smalldatetime"));
}
```

following query will fail:
```
var testDateList = new List<DateTime>() { new DateTime(2018, 10, 07) };
var findRecordsWithDateInList = reproContext.ReproEntity
    .Where(a => testDateList.Contains(a.MyTime))
    .ToList();
```
with exception:
```
System.Data.SqlClient.SqlException (0x80131904): Conversion failed when converting character string to smalldatetime data type.
   at System.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at System.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at System.Data.SqlClient.SqlDataReader.TryHasMoreRows(Boolean& moreRows)
   at System.Data.SqlClient.SqlDataReader.TryReadInternal(Boolean setTimeout, Boolean& more)
   at System.Data.SqlClient.SqlDataReader.Read()
   at Microsoft.EntityFrameworkCore.Query.Internal.QueryingEnumerable`1.Enumerator.BufferlessMoveNext(DbContext _, Boolean buffer)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.Execute[TState,TResult](TState state, Func`3 operation, Func`3 verifySucceeded)
   at Microsoft.EntityFrameworkCore.Query.Internal.QueryingEnumerable`1.Enumerator.MoveNext()
   at Microsoft.EntityFrameworkCore.Query.Internal.LinqOperatorProvider._TrackEntities[TOut,TIn](IEnumerable`1 results, QueryContext queryContext, IList`1 entityTrackingInfos, IList`1 entityAccessors)+MoveNext()
   at Microsoft.EntityFrameworkCore.Query.Internal.LinqOperatorProvider.ExceptionInterceptor`1.EnumeratorExceptionInterceptor.MoveNext()
   at System.Collections.Generic.List`1.AddEnumerable(IEnumerable`1 enumerable)
   at System.Linq.Enumerable.ToList[TSource](IEnumerable`1 source)
```

Observed on:
* netcoreapp2.1
* Microsoft.EntityFrameworkCore.SqlServer, 2.1.2
* Microsoft.EntityFrameworkCore.Design, 2.1.2
