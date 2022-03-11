using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Todos.Utils.Data;
using Todos.Utils.Query;
using Xunit;

namespace Todos.Utils.Tests.Data;

public class ReadWriteRepositoryTests
{
    public class TestRecord : IDataRecord
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }
    }
    
    private class TestContext : DbContext
    {
        public DbSet<TestRecord> TestRecords { get; set; }
        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        {
        }
    }


    private (ReadWriteRepository<TestRecord>, TestContext) SetupData()
    {
        var records = new List<TestRecord>();
        records.Add(new TestRecord{Id = 1, CreatedAt = DateTime.Now.AddDays(-6), ArchivedAt = null});
        records.Add(new TestRecord{Id = 2, CreatedAt = DateTime.Now.AddDays(-6), ArchivedAt = DateTime.Now.AddDays(-2)});
        records.Add(new TestRecord{Id = 3, CreatedAt = DateTime.Now.AddDays(-2), ArchivedAt = DateTime.Now.AddDays(-6)});
        records.Add(new TestRecord{Id = 5, CreatedAt = DateTime.Now.AddDays(-1), ArchivedAt = null});
        records.Add(new TestRecord{Id = 4, CreatedAt = DateTime.Now.AddDays(-10), ArchivedAt = DateTime.Now});


        var options = new DbContextOptionsBuilder<TestContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging(true)
            .Options;

        var context = new TestContext(options);

        foreach (var sr in records)
        {
            context.Add(sr);
            context.SaveChanges();
        }


        return (new ReadWriteRepository<TestRecord>(context), context);
    }
    
    private static IEnumerable<object[]> GetAll_Data
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    new QueryOptions(),
                    new List<int>{1,2,3,4,5}
                },
            };
        }
    }
    
    [Theory]
    [InlineData(null, null, null, false, true, new int[] {1,2,3,5,4})]
    [InlineData(2, null, null, false, true, new int[] {1,2})]
    [InlineData(1, 2, null, false, true, new int[] {3})]
    [InlineData(null, null, "Id", false, true, new int[] {1,2,3,4,5})]
    [InlineData(null, null, "Id", true, true, new int[] {5,4,3,2,1})]
    [InlineData(null, null, "ArchivedAt", false, true, new int[] {1,5,3,2,4})]
    [InlineData(null, null, null, false, false, new int[] {1,5})]
    public void GetAll(int? limit, int? offset, string? order, bool isDescending, bool includeArchived, int[] expectedIds)
    {
        var query = new QueryOptions
        {
            Limit = limit,
            Offset = offset,
            Order = order,
            IsDescending = isDescending,
            IncludeArchived = includeArchived
        };
        var repo = this.SetupData().Item1;
        var results = repo.GetAll(query).ToList();
        Assert.InRange(results.Count, expectedIds.Length, expectedIds.Length);
        for (var i = 0; i < expectedIds.Length; i++)
        {
            Assert.Equal(expectedIds[i], results[i].Id);
        }
    }
    
    [Fact]
    public void GetAll_Null()
    {
        var expectedIds = new int[] {1, 2, 3, 5, 4};
        var repo = this.SetupData().Item1;
        var results = repo.GetAll(null).ToList();
        Assert.InRange(results.Count, expectedIds.Length, expectedIds.Length);
        for (var i = 0; i < expectedIds.Length; i++)
        {
            Assert.Equal(expectedIds[i], results[i].Id);
        }
    }
    
    [Fact]
    public void Add()
    {
        var (repo, ctx) = this.SetupData();
        var record = new TestRecord();
        var beforeAction = DateTime.Now;
        var result = repo.Add(record);
        Assert.NotNull(result);
        Assert.InRange(ctx.TestRecords.Count(), 6, 6);
        Assert.Equal(6, result.Id);
        Assert.True(result.CreatedAt < DateTime.Now);
        Assert.True(result.CreatedAt > beforeAction);
        Assert.Null(result.ArchivedAt);
    }
    
    [Fact]
    public void Update()
    {
        var (repo, ctx) = this.SetupData();
        var record = ctx.TestRecords.FirstOrDefault();
        var createdAt = record.CreatedAt;
        var archivedAt = record.ArchivedAt;
        var beforeAction = DateTime.Now;
        var result = repo.Update(record);
        Assert.NotNull(result);
        Assert.InRange(ctx.TestRecords.Count(), 5, 5);
        Assert.Equal(1, result.Id);
        Assert.Equal(archivedAt, result.ArchivedAt);
        Assert.Equal(createdAt, result.CreatedAt);
    }
    
        
    [Fact]
    public void Archive()
    {
        var (repo, ctx) = this.SetupData();
        var beforeAction = DateTime.Now;
        var result = repo.Archive(1);
        Assert.NotNull(result);
        Assert.InRange(ctx.TestRecords.Count(), 5, 5);
        Assert.Equal(1, result.Id);
        Assert.True(result.ArchivedAt < DateTime.Now);
        Assert.True(result.ArchivedAt > beforeAction);
    }
    
}