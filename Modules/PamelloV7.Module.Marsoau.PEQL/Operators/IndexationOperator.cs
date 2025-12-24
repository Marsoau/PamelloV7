using PamelloV7.Core.Attributes;
using PamelloV7.Core.Extensions;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Core.Model.PEQL;

namespace PamelloV7.Module.Marsoau.PEQL.Operators;

[EntityOperator("Indexation", ':')]
public class IndexationOperator : EntityOperator
{
    public IndexationOperator(IServiceProvider services) : base(services) { }
    
    public override IEnumerable<IPamelloEntity> Execute(IPamelloUser scopeUser, string query, string value) {
        var results = _peql.Get(query, scopeUser).ToArray();

        var position = 0;

        var separatorAt = value.IndexOf('-');
        if (separatorAt == 0) {
            separatorAt = value.IndexOf('-', 1);
        }

        if (separatorAt == -1) {
            position = results.TranslateValueIndex(value);
            var result = results.ElementAtOrDefault(position);
            
            return result is not null ? [result] : [];
        }
        
        position = results.TranslateValueIndex(value[..separatorAt]);
        var end = results.TranslateValueIndex(value[(separatorAt + 1)..]);

        if (end < position) {
            (position, end) = (end, position);
        }
        
        if (end > results.Length - 1) end = results.Length - 1;
        if (position < 0) position = 0;
        
        return results.Skip(position).Take(end - position + 1);
    }
}
