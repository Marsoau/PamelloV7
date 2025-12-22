using PamelloV7.Core.Attributes;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Core.Model.PEQL;

namespace PamelloV7.Module.Marsoau.PEQL.Operators;

[EntityOperator("Multiplication", '*')]
public class MultiplicationOperator : EntityOperator
{
    public MultiplicationOperator(IServiceProvider services) : base(services) { }

    public override IEnumerable<IPamelloEntity> Execute(IPamelloUser scopeUser, string query, string value) {
        if (int.TryParse(value, out var multiplier) == false) throw new Exception($"Value {value} is not a number");

        var results = new List<IPamelloEntity>();

        for (var i = 0; i < multiplier; i++) {
            results.AddRange(_peql.Get(query, scopeUser));
        }

        return results;
    }
}
