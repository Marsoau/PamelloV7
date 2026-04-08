using PamelloV7.Framework.Difference;
using Xunit.Abstractions;

namespace PamelloV7.Tests.Difference;

public class DifferenceResultPositionTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    public DifferenceResultPositionTests(ITestOutputHelper testOutputHelper) {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void A1() {
        var result = DifferenceResult<int>.From([1, 100, 3, 4], [1, 3, 4, 100, 11]);
        Write(result);
    }
    
    [Fact]
    public void A2() {
        var result = DifferenceResult<int>.From([1, 2, 3, 4, 100, 5, 6], [11, 12, 3, 4, 100, 13, 14]);
        Write(result);
    }
    
    [Fact]
    public void A3() {
        var result = DifferenceResult<int>.From([100, 2, 3, 4], [2, 3, 4, 100, 11]);
        Write(result);
    }
    
    [Fact]
    public void A4() {
        var result = DifferenceResult<int>.From([1, 2, 3, 4, 100, 5], [2, 100, 3, 4, 1, 5]);
        Write(result);
    }
    
    [Fact]
    public void Apply1() {
        var pos100 = 0;
        var value100 = 1;
        var targetPos = 8;
        List<int> source = [1, 2, 3, 4, 100, 5];
        List<int> target = [2, 8, 3, 5, 6, 7, 100, 4, 1, 5];
        var result = DifferenceResult<int>.From(source, target);
        
        Write(result);
        
        result.Apply(source,
            onAdd: (i, v) => {
                _testOutputHelper.WriteLine($"Add: {i} = {v}; {pos100}");
                if (pos100 == -1 && v == value100) pos100 = i;
                else if (i <= pos100) pos100++;
            },
            onDelete: (i, _) => {
                _testOutputHelper.WriteLine($"Delete: {i}; {pos100}");
                if (i == pos100) pos100 = -1;
                else if (i < pos100) pos100--;
            }
        );
        
        Write(source, "Source");
        Write(target, "Target");
        
        _testOutputHelper.WriteLine($"Final: {pos100}");
        
        Assert.Equal(source, target);
        Assert.Equal(targetPos, pos100);
    }

    public void Write<T>(DifferenceResult<T> result) {
        Write(result.Deleted, "Deleted");
        Write(result.Added, "Added");
        Write(result.Moved, "Moved");
    }

    public void Write<TA, TB>(Dictionary<TA, TB> dictionary, string message = "") {
        if (dictionary.Count == 0) {
            _testOutputHelper.WriteLine($"{message}: [] //None");
            return;
        }
        _testOutputHelper.WriteLine($"{message}: [\n    {string.Join("\n    ", dictionary.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}\n]");
    }
    
    public void Write<TA>(List<TA> list, string message = "") {
        if (list.Count == 0) {
            _testOutputHelper.WriteLine($"{message}: [] //None");
            return;
        }
        _testOutputHelper.WriteLine($"{message}: [\n    {string.Join("\n    ", list)}\n]");
    }
}
