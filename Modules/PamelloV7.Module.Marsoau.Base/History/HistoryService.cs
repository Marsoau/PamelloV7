using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Data;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Events.RestorePacks.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.History.Records;
using PamelloV7.Core.History.Services;
using PamelloV7.Module.Marsoau.Base.History.Records;

namespace PamelloV7.Module.Marsoau.Base.History;

public class HistoryService : IHistoryService
{
    private readonly IServiceProvider _services;

    private readonly IDatabaseAccessService _database;
    
    private readonly List<IHistoryRecord> _unfinished;
    
    public HistoryService(IServiceProvider services) {
        _services = services;
        
        _database = services.GetRequiredService<IDatabaseAccessService>();

        _unfinished = [];
    }
    
    private IDatabaseCollection<HistoryRecord> GetCollection() => _database.GetCollection<HistoryRecord>("history");

    private void Save(IHistoryRecord record) {
        var collection = GetCollection();
        
        collection.Drop();
        collection.Add((HistoryRecord)record);
    }

    public IHistoryRecord GetRequired(int id)
        => Get(id) ?? throw new PamelloException($"History record with id {id} not found");
    public IHistoryRecord? Get(int id) {
        var collection = GetCollection();
        
        var record = collection.Get(id);
        if (record?.Event is not RevertiblePamelloEvent revertibleEvent) return record;
        
        if (typeof(RevertPack).GetField("Services") is { } servicesProperty) {
            servicesProperty.SetValue(revertibleEvent.RevertPack, _services);
        }

        return record;
    }
    
    public void Record(IPamelloEvent e) {
        Console.Write($"Record event: ");

        var record = _unfinished.FirstOrDefault(record => record.Event == e) ?? new HistoryRecord(e);
        
        Write(record);
        Save(record);
    }

    public void Record(IPamelloEvent nestedEvent, IPamelloEvent parentEvent) {
        Console.WriteLine($"Record nested event: {nestedEvent.GetType().Name}; in parent event: {parentEvent.GetType().Name}");

        if (_unfinished.FirstOrDefault(record => record.Event == parentEvent) is { } unfinishedParent) {
            unfinishedParent.NestedRecords.Add(new HistoryRecord(nestedEvent));
            
            Console.WriteLine($"Added nested {nestedEvent.GetType().Name} to {unfinishedParent.Event.GetType().Name}");
        }
        else if (_unfinished.FirstOrDefault(record => record.Event == nestedEvent) is { } unfinishedNested) {
            _unfinished.Remove(unfinishedNested);
            
            var record = new HistoryRecord(parentEvent);
            record.NestedRecords.Add(unfinishedNested);

            Console.WriteLine($"Added new: {record.Event.GetType().Name} with {record.NestedRecords.First()}");
            
            _unfinished.Add(record);
        }
        else {
            var record = new HistoryRecord(parentEvent);
            record.NestedRecords.Add(new HistoryRecord(nestedEvent));
            _unfinished.Add(record);

            Console.WriteLine($"Added new: {record.Event.GetType().Name} with {record.NestedRecords.First()}");
        }
    }

    private void Write(IHistoryRecord record) {
        _unfinished.Remove(record);

        Console.Write($"{record.Event.GetType().Name} ({record.NestedRecords.Count})");
        if (record.NestedRecords.FirstOrDefault() is { } firstNested) {
            Console.Write(" -> ");
            Write(firstNested);
        }
        else {
            Console.WriteLine();
        }
    }
}
