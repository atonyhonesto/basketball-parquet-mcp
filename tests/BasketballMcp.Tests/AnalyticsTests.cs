using BasketballMcp;
using Parquet.Serialization;
using Xunit;

public class AnalyticsTests : IDisposable
{
    readonly string directory = Path.Combine(Path.GetTempPath(), "basketball-tests-" + Guid.NewGuid());
    public AnalyticsTests() => Directory.CreateDirectory(directory);
    public void Dispose() => Directory.Delete(directory, true);

    [Fact] public async Task AggregatesAllGamesWithWeightedPercentage()
    {
        await SampleData.WriteAsync(directory);
        var result = await new BasketballStore(directory).FreeThrowsAsync(10);
        var first = result.First();
        Assert.Equal("Morgan Lane", first.Player);
        Assert.Equal(19, first.Made);
        Assert.Equal(22, first.Attempts);
        Assert.Equal(86.36m, first.Percentage);
        Assert.Equal(3, first.Games);
        Assert.DoesNotContain(result, x => x.PlayerId == "p4");
    }
    [Fact] public async Task RejectsDuplicatePlayersAcrossFiles()
    {
        await ParquetSerializer.SerializeAsync(SampleData.Rows(1), Path.Combine(directory,"one.parquet"));
        await ParquetSerializer.SerializeAsync(SampleData.Rows(1), Path.Combine(directory,"two.parquet"));
        await Assert.ThrowsAsync<InvalidDataException>(() => new BasketballStore(directory).ReadAsync());
    }
    [Fact] public async Task AppliesMinimumAttemptsToSeriesTotal()
    {
        await SampleData.WriteAsync(directory);
        var result = await new BasketballStore(directory).FreeThrowsAsync(16);
        Assert.Single(result);
        Assert.Equal("p1",result[0].PlayerId);
    }
    [Fact] public void RejectsImpossibleShootingCounts()
    {
        var row = SampleData.Rows(1)[0];
        row.FreeThrowsMade = row.FreeThrowsAttempted + 1;
        Assert.Throws<InvalidDataException>(() => BasketballStore.Validate(row));
    }
    [Fact] public async Task RejectsUnrecognizedParquetSchema()
    {
        await ParquetSerializer.SerializeAsync(new[] { new WrongSchema { Value = 1 } }, Path.Combine(directory,"wrong.parquet"));
        await Assert.ThrowsAsync<InvalidDataException>(() => new BasketballStore(directory).ReadAsync());
    }
    [Fact] public async Task EmptyDirectoryReturnsNoInventedResults() => Assert.Empty(await new BasketballStore(directory).FreeThrowsAsync(1));
    [Fact] public async Task RejectsInvalidThreshold() => await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => new BasketballStore(directory).FreeThrowsAsync(0));
    [Fact] public async Task PagingReportsTotalCoverage()
    {
        await SampleData.WriteAsync(directory);
        using var json = System.Text.Json.JsonDocument.Parse(await BasketballTools.QueryPlayerGames(new BasketballStore(directory), 10, 2));
        Assert.Equal(12,json.RootElement.GetProperty("total_rows").GetInt32());
        Assert.Equal(2,json.RootElement.GetProperty("rows").GetArrayLength());
    }
    public class WrongSchema { public int Value { get; set; } }
}
