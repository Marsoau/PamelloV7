using System.Reflection;
using LiteDB;
using PamelloV7.Framework.Containers;
using PamelloV7.Framework.Data;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;

namespace PamelloV7.Server.Database;

public class DatabaseAccessService : IDatabaseAccessService
{
    private readonly LiteDatabase _db;
    
    private readonly IAssemblyTypeResolver _typeResolver;
    private readonly IFileAccessService _files;

    public DatabaseAccessService(IServiceProvider services) {
        _typeResolver = services.GetRequiredService<IAssemblyTypeResolver>();
        _files = services.GetRequiredService<IFileAccessService>();
        
        _db = new LiteDatabase(_files.GetDatabaseFile().FullName, GetMapper());
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
                    Console.WriteLine($"Mapping: {stringData}");
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
                    
                    var safeType = typeof(SafeStoredEntities<>).MakeGenericType(entitiesType);
                    var safeEntities = Activator.CreateInstance(safeType, entitiesIds);
                    
                    return safeEntities;
                }
                
                if (entityString.IndexOf('^') is { } idPosition && idPosition != -1) {
                    var typeName = entityString[..idPosition];
                    
                    var entityType = _typeResolver.GetByFullName(typeName);
                    if (entityType is null) return null;
                    
                    var entityId = int.Parse(entityString[(idPosition + 1)..]);
                    
                    var safeType = typeof(SafeStoredEntity<>).MakeGenericType(entityType);
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
