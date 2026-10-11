using System.Reflection;
using LiteDB;
using PamelloV7.Framework.Config;
using PamelloV7.Framework.Containers;
using PamelloV7.Framework.Data;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;
using PamelloV7.Server.Database;

namespace PamelloV7.Server.Services;

public class DatabaseAccessService : IDatabaseAccessService
{
    private readonly LiteDatabase _db;
    
    private readonly IAssemblyTypeResolver _typeResolver;
    private readonly IFileAccessService _files;

    public DatabaseAccessService(IServiceProvider services) {
        _typeResolver = services.GetRequiredService<IAssemblyTypeResolver>();
        _files = services.GetRequiredService<IFileAccessService>();
        
        _db = LoadDatabase();
    }

    private LiteDatabase LoadDatabase() {
        var outdatedDataFile = new FileInfo(_files.GetOutdatedDatabaseFile());
        var outdatedLogFile = new FileInfo(_files.GetOutdatedDatabaseLogFile());
        
        var dataFile = new FileInfo(_files.GetDatabaseFile());
        var logFile = new FileInfo(_files.GetDatabaseLogFile());

        dataFile = outdatedDataFile.Exists && !dataFile.Exists
            ? outdatedDataFile.CopyTo(_files.GetDatabaseFile(), true)
            : dataFile;
        logFile = outdatedLogFile.Exists && !logFile.Exists
            ? outdatedLogFile.CopyTo(_files.GetDatabaseLogFile(), true)
            : logFile;

        if (!ServerConfig.Root.DoNotSaveChanges) {
            Output.Write("Loading normal database");
            return new LiteDatabase(dataFile.FullName, GetMapper());
        }
        
        Output.Write("Loading temporary database");

        Console.WriteLine($"data: {dataFile.Length}");
        Console.WriteLine($"data-log: {logFile.Length}");
        
        Console.WriteLine($"data-TEMP: {new FileInfo(_files.GetTempDatabaseFile()).Length}");
        Console.WriteLine($"data-TEMP-log: {new FileInfo(_files.GetTempDatabaseLogFile()).Length}");
        
        var tempDatabaseFile = dataFile.Exists
            ? dataFile.CopyTo(_files.GetTempDatabaseFile(), true)
            : new FileInfo(_files.GetTempDatabaseFile());
        
        if (logFile.Exists) logFile.CopyTo(_files.GetTempDatabaseLogFile(), true);

        Console.WriteLine("After");
        
        Console.WriteLine($"data-TEMP: {new FileInfo(_files.GetTempDatabaseFile()).Length}");
        Console.WriteLine($"data-TEMP-log: {new FileInfo(_files.GetTempDatabaseLogFile()).Length}");
        
        return new LiteDatabase(tempDatabaseFile.FullName, GetMapper());
    }

    private BsonMapper GetMapper() {
        var mapper = new BsonMapper();

        mapper.ResolveMember = (parentType, memberInfo, memberMapper) => {
            FieldInfo? safeEntityField = null;
            PropertyInfo? safeEntitiesProperty = null;

            if (parentType.GetProperty(memberInfo.Name) is { } property) {
                if (memberMapper.DataType.IsAssignableTo(typeof(ISafeStoredEntities))) {
                    safeEntitiesProperty = property;
                }
            }

            if (safeEntitiesProperty is null) {
                var type = parentType;
                var currentField = type.GetField($"_safe{memberInfo.Name}");
                while (type.BaseType != null && currentField is null) {
                    type = type.BaseType;
                        
                    currentField = type.GetField($"_safe{memberInfo.Name}");
                }
                
                safeEntityField = currentField;
            }

            if (safeEntitiesProperty is null && safeEntityField is null) {
                if (!memberMapper.DataType.IsAssignableTo(typeof(IPamelloEntity))) return;
                
                memberMapper.IsIgnore = true;
                return;
            }
            
            if ((safeEntitiesProperty?.PropertyType.IsGenericType ?? false) && (
                    safeEntitiesProperty.PropertyType.GenericTypeArguments.FirstOrDefault() == typeof(IPamelloPlayer) ||
                    safeEntitiesProperty.PropertyType.GenericTypeArguments.FirstOrDefault() == typeof(IPamelloSpeaker)
                )) return;
            
            memberMapper.Getter = (obj) => {
                if (safeEntityField?.GetValue(obj) is ISafeStoredEntity { } entity) return $"{entity.EntityType.FullName}^{entity.Id}";
                if (safeEntitiesProperty?.GetValue(obj) is ISafeStoredEntities { } entities) return $"{entities.EntitiesType.FullName}|{string.Join(",", entities.InternalIds)}";

                return null;
            };
            memberMapper.Setter = (obj, value) => {
                if (value == null) return;

                if (safeEntitiesProperty is not null) {
                    safeEntitiesProperty.SetValue(obj, value);
                }
                else if (safeEntityField is not null) {
                    safeEntityField.SetValue(obj, value);
                }
            };
            
            memberMapper.Serialize = (getterValue, bm) => {
                if (getterValue is string stringData) {
                    return new BsonValue(stringData); 
                }
                return BsonValue.Null;
            };
        
            memberMapper.Deserialize = (bsonValue, m) => {
                if (bsonValue.IsNull || bsonValue.AsString is not { } entityString) return null;

                if (entityString.IndexOf('|') is {} idsPosition && idsPosition != -1) {
                    var typeName = entityString[..idsPosition];
                    
                    var entitiesType = _typeResolver.GetByFullName(typeName);
                    if (entitiesType is null) return null;
                    
                    var entitiesIds = entityString.Length == idsPosition + 1 ? [] :
                        entityString[(idsPosition + 1)..].Split(',').Select(int.Parse);
                    
                    var safeType = typeof(SafeList<>).MakeGenericType(entitiesType);
                    var safeEntities = Activator.CreateInstance(safeType, entitiesIds);
                    
                    return safeEntities;
                }
                
                if (entityString.IndexOf('^') is { } idPosition && idPosition != -1) {
                    var typeName = entityString[..idPosition];
                    
                    var entityType = _typeResolver.GetByFullName(typeName);
                    if (entityType is null) return null;
                    
                    var entityId = int.Parse(entityString[(idPosition + 1)..]);
                    
                    var safeType = typeof(Safe<>).MakeGenericType(entityType);
                    var safeEntity = Activator.CreateInstance(safeType, entityId);
                    
                    return safeEntity;
                }

                return null;
            };
        };
        
        
        return mapper;
    }

    public IDatabaseCollection<TType> GetCollection<TType>(string name) {
        var collection = _db.GetCollection<TType>(name);

        return new DatabaseCollection<TType>(collection);
    }
}
