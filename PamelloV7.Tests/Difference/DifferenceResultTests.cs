using PamelloV7.Core.Difference;

namespace PamelloV7.Tests.Difference;

public class DifferenceResultTests
{
    [Fact]
    public void Application() {
        var from = new List<int> {1, 2, 3, 4, 5, 6, 7, 8};
        var to = new List<int> {1, 2, 4, 5, 10, 7, 8, 6};
        
        var result = DifferenceResult<int>.From(from, to);
        
        result.Apply(from, a => from[a]);
        
        Assert.Equal(from, to);
    }
    
    [Fact]
    public void Addition() {
        var result = DifferenceResult<int>.From([1, 2, 3, 4], [1, 2, 11, 3, 4, 12, 13]);
        
        Assert.Equal(3, result.Added.Count);

        var expectedAdded = new Dictionary<int, int>() {
            [2] = 11,
            [5] = 12,
            [6] = 13
        };

        Assert.Equal(expectedAdded, result.Added);
    }
    
    [Fact]
    public void Deletion() {
        var result = DifferenceResult<int>.From([1, 2, 11, 3, 4, 12, 13], [1, 2, 3, 4]);
        
        Assert.Equal(3, result.Deleted.Count);

        var expectedDeleted = new Dictionary<int, int>() {
            [2] = 11,
            [5] = 12,
            [6] = 13
        };

        Assert.Equal(expectedDeleted, result.Deleted);
    }
    
    [Fact]
    public void Change() {
        var result = DifferenceResult<int>.From([1, 2, 3, 4, 5, 6], [1, 12, 13, 4, 15, 6]);
        
        Assert.Equal(3, result.Changed.Count);
    }
    
    [Fact]
    public void MovementAdditionDelition() {
        var result = DifferenceResult<int>.From([1, 2, 3, 4], [1, 3, 4, 2]);

        Assert.Equal(new Dictionary<int, int>() {
            [1] = 2
        }, result.Deleted);
        Assert.Equal(new Dictionary<int, int>() {
            [3] = 2
        }, result.Added);
    }

    [Fact]
    public void SimpleMovement() {
        var result = DifferenceResult<int>.From([1, 2, 3, 4], [1, 3, 4, 2], withMoved: true);
        
        Assert.Empty(result.Added);
        Assert.Empty(result.Deleted);
        Assert.Equal(new Dictionary<int, int>() {
            [2] = 1,
            [3] = 2
        }, result.Moved);
    }
}
