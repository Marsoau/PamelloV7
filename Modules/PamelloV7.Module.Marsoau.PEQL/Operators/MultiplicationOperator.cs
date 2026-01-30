using PamelloV7.Core.Attributes;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.PEQL;

namespace PamelloV7.Module.Marsoau.PEQL.Operators;

[EntityOperator("Multiplication", '*')]
public class MultiplicationOperator : EntityOperator
{
    public MultiplicationOperator(IServiceProvider services) : base(services) { }

    public override async Task<IEnumerable<IPamelloEntity>> ExecuteAsync(IPamelloUser scopeUser, string query, string value) {
        if (!int.TryParse(value, out var multiplier)) throw new PamelloException($"Value {value} is not a number");

        var results = new List<IPamelloEntity>();
    
        for (var i = 0; i < multiplier; i++) {
            results.AddRange(await _peql.GetAsync(query, scopeUser));
        }

        return results;
    }
}
