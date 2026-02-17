using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Attributes;
using PamelloV7.Core.Data;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Events.RestorePacks.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.History.Records;
using PamelloV7.Core.History.Services;

namespace PamelloV7.Module.Marsoau.Base.History;

public class HistoryService : IHistoryService
{
    private readonly IServiceProvider _services;

    private readonly IDatabaseAccessService _database;
    
    private readonly List<NestedPamelloEvent> _unfinished;
    private readonly List<HistoryRecord> _records;
    
    public HistoryService(IServiceProvider services) {
        _services = services;
        
        _database = services.GetRequiredService<IDatabaseAccessService>();

        _unfinished = [];
        _records = [];
    }
    
    private IDatabaseCollection<HistoryRecord> GetCollection() => _database.GetCollection<HistoryRecord>("history");

    public void Startup(IServiceProvider services) {
        Console.WriteLine("Stated up history service");
    }

    public void FullReset() {
        GetCollection().Drop();
        _records.Clear();
    }

    public void WriteAll() {
        var all = GetCollection().GetAll().ToList();
        Console.WriteLine($"All records: {all.Count}");

        foreach (var record in all) {
            Console.WriteLine($"Record {record.CreatedAt} by {record.Performer}: {record.Nested.Event.GetType().Name} with {record.Nested.NestedEvents.Count} nested events");
        }
    }

    private HistoryRecord Save(NestedPamelloEvent nested, IPamelloUser? scopeUser) {
        _unfinished.Remove(nested);
        
        var collection = GetCollection();
        var record = new HistoryRecord(nested, scopeUser);
        
        collection.Add(record);
        _records.Add(record);

        record.Nested.ActivateRestorePacks(_services);
        
        return record;
    }

    public HistoryRecord GetRequired(int id)
        => Get(id) ?? throw new PamelloException($"History record with id {id} not found");
    public HistoryRecord? Get(int id) {
        return _records.FirstOrDefault(record => record.Id == id);
    }
    
    public HistoryRecord Record(IPamelloEvent e, IPamelloUser? scopeUser) {
        Console.Write($"Record event: ");

        var nested = _unfinished.FirstOrDefault(record => record.Event == e) ?? new NestedPamelloEvent(e);
        var record = Save(nested, scopeUser);
        
        Write(record.Nested);
        
        return record;
    }

    public void Record(IPamelloEvent nestedEvent, IPamelloEvent parentEvent) {
        Console.WriteLine($"Record nested event: {nestedEvent.GetType().Name}; in parent event: {parentEvent.GetType().Name}");

        if (_unfinished.FirstOrDefault(record => record.Event == parentEvent) is { } unfinishedParent) {
            unfinishedParent.NestedEvents.Add(new NestedPamelloEvent(nestedEvent));
            
            Console.WriteLine($"Added nested {nestedEvent.GetType().Name} to {unfinishedParent.Event.GetType().Name}");
        }
        else if (_unfinished.FirstOrDefault(record => record.Event == nestedEvent) is { } unfinishedNested) {
            _unfinished.Remove(unfinishedNested);
            
            var record = new NestedPamelloEvent(parentEvent);
            record.NestedEvents.Add(unfinishedNested);

            Console.WriteLine($"Added new: {record.Event.GetType().Name} with {record.NestedEvents.First()}");
            
            _unfinished.Add(record);
        }
        else {
            var record = new NestedPamelloEvent(parentEvent);
            record.NestedEvents.Add(new NestedPamelloEvent(nestedEvent));
            _unfinished.Add(record);

            Console.WriteLine($"Added new: {record.Event.GetType().Name} with {record.NestedEvents.First()}");
        }
    }

    private void Write(NestedPamelloEvent nested) {
        Console.Write($"{nested.Event.GetType().Name} ({nested.NestedEvents.Count})");
        if (nested.NestedEvents.FirstOrDefault() is { } firstNested) {
            Console.Write(" -> ");
            Write(firstNested);
        }
        else {
            Console.WriteLine();
        }
    }
}
