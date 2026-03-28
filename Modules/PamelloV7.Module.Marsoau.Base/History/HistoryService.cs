using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Data;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.RestorePacks.Base;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.History.Records;
using PamelloV7.Framework.History.Services;
using PamelloV7.Framework.Logging;

namespace PamelloV7.Module.Marsoau.Base.History;

public class HistoryService : IHistoryService
{
    private readonly IServiceProvider _services;

    private readonly IDatabaseAccessService _database;
    
    private readonly List<NestedPamelloEvent> _unfinished;
    private readonly List<IHistoryRecord> _records;
    
    public HistoryService(IServiceProvider services) {
        _services = services;
        
        _database = services.GetRequiredService<IDatabaseAccessService>();

        _unfinished = [];
        _records = [];
    }
    
    private IDatabaseCollection<HistoryRecord> GetCollection() => _database.GetCollection<HistoryRecord>("history");

    public void Startup(IServiceProvider services) {
        GetCollection().GetAll().ToList().ForEach(databaseRecord => Load(databaseRecord));
    }

    public void FullReset() {
        GetCollection().Drop();
        _records.Clear();
    }

    public void WriteAll() {
        Output.Write($"All records: {_records.Count}");

        foreach (var record in _records) {
            Output.Write($"Record {record.CreatedAt} by {record.Performer}: {record.Nested.Event.GetType().Name} with {record.Nested.NestedEvents.Count} nested events");
        }
    }

    private IHistoryRecord Load(HistoryRecord databaseRecord) {
        _records.Insert(0, databaseRecord);
        
        databaseRecord.Nested.ActivateRestorePacks(_services);
        
        return databaseRecord;
    }

    private IHistoryRecord Save(NestedPamelloEvent nested, IPamelloUser? scopeUser) {
        _unfinished.Remove(nested);
        
        var collection = GetCollection();
        var databaseRecord = new HistoryRecord(nested, scopeUser);
        collection.Add(databaseRecord);
        
        return Load(databaseRecord);
    }

    public IHistoryRecord? Get(IPamelloUser scopeUser, int id) {
        return Get(id);
    }

    public IEnumerable<IHistoryRecord> GetAll(IPamelloUser scopeUser) {
        return _records.ToList();
    }

    public IEnumerable<IHistoryRecord> GetLast(IPamelloUser scopeUser, int count = 1) {
        return _records.Take(count).ToList();
    }

    public IHistoryRecord? GetRequired(int id)
        => Get(id) ?? throw new PamelloException($"History record with id {id} not found");
    public IHistoryRecord? Get(int id) {
        return _records.FirstOrDefault(record => record.Id == id);
    }
    
    public IHistoryRecord Record(IPamelloEvent e, IPamelloUser? scopeUser) {
        Debug.Write($"Record event: ");

        var nested = _unfinished.FirstOrDefault(record => record.Event == e) ?? new NestedPamelloEvent(e);
        var record = Save(nested, scopeUser);
        
        //Write(record.Nested);
        
        return record;
    }

    public void Record(IPamelloEvent nestedEvent, IPamelloEvent parentEvent) {
        Debug.WriteLine($"Record nested event: {nestedEvent.GetType().Name}; in parent event: {parentEvent.GetType().Name}");

        if (_unfinished.FirstOrDefault(record => record.Event == parentEvent) is { } unfinishedParent) {
            unfinishedParent.NestedEvents.Add(new NestedPamelloEvent(nestedEvent));
        }
        else if (_unfinished.FirstOrDefault(record => record.Event == nestedEvent) is { } unfinishedNested) {
            _unfinished.Remove(unfinishedNested);
            
            var record = new NestedPamelloEvent(parentEvent);
            record.NestedEvents.Add(unfinishedNested);
            
            _unfinished.Add(record);
        }
        else {
            var record = new NestedPamelloEvent(parentEvent);
            record.NestedEvents.Add(new NestedPamelloEvent(nestedEvent));
            _unfinished.Add(record);
        }
    }

    private void Write(NestedPamelloEvent nested) {
        Output.Write($"{nested.Event.GetType().Name} ({nested.NestedEvents.Count})");
        if (nested.NestedEvents.FirstOrDefault() is { } firstNested) {
            Output.Write(" -> ");
            Write(firstNested);
        }
        else {
            Output.Write();
        }
    }
}
