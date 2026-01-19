using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using PamelloV7.Core.Attributes;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Model.Entities.Base;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Model;
using PamelloV7.Server.Exceptions;

namespace PamelloV7.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataController : PamelloControllerBase
    {
        public readonly IEntityQueryService _peql;
        
        public readonly IAssemblyTypeResolver _typeResolver;
        
        public readonly IPamelloLogger _logger;

        public DataController(IServiceProvider services) : base(services) {
            _peql = services.GetRequiredService<IEntityQueryService>();
            
            _typeResolver = services.GetRequiredService<IAssemblyTypeResolver>();
            
            _logger = services.GetRequiredService<IPamelloLogger>();
        }

        [HttpGet("{*query}")]
        public IActionResult Get(string query) {
            RequireUser();

            var view = EDtoView.Normal;
            var type = typeof(IPamelloEntity);
            var single = false;

            if (Request.Query.TryGetValue("type", out var typeStrValues)) {
                type = _typeResolver.GetTypeByName(typeStrValues.FirstOrDefault() ?? "");
                
                if (!type?.IsAssignableTo(typeof(IPamelloEntity)) ?? false)
                    throw new PamelloControllerException(BadRequest("type must be assignable to IPamelloEntity"));
            }
            if (Request.Query.TryGetValue("single", out var singleStrValues)) {
                if (!bool.TryParse(singleStrValues.FirstOrDefault(), out var s))
                    throw new PamelloControllerException(BadRequest("single must be boolean"));
                
                single = s;
            }
            if (Request.Query.TryGetValue("view", out var viewStrValues)) {
                if (!Enum.TryParse(viewStrValues.FirstOrDefault(), out EDtoView v))
                    throw new PamelloControllerException(BadRequest($"view must be one of: {string.Join(", ", Enum.GetNames<EDtoView>())}"));
                
                view = v;
            }

            _logger.Log($"User: {User} Query: {query} [type: {type?.Name}, view: {view}, single: {single}]");
            
            IEnumerable<IPamelloEntity> results;

            if (type is not null) {
                var method = typeof(IEntityQueryService)
                    .GetMethods()
                    .FirstOrDefault(m => m is { Name: "Get", IsGenericMethod: true })
                    ?.MakeGenericMethod(type);
                Debug.Assert(method is not null);
                
                results = method.Invoke(_peql, [query, User]) as IEnumerable<IPamelloEntity> ?? [];
            }
            else {
                results = _peql.Get(query, User);
            }

            if (single) {
                return Ok(results.FirstOrDefault()?.GetDto() ?? throw new PamelloControllerException(NotFound()));
            }

            switch (view) {
                case EDtoView.Normal:
                    return single ?
                        Ok(results.FirstOrDefault()?.GetDto() ?? throw new PamelloControllerException(NotFound($"Could not find single entity by \"{query}\" query"))) :
                        Ok(results.Select(object (x) => x.GetDto()));
                case EDtoView.Ids:
                    return single ?
                        Ok(results.FirstOrDefault()?.Id ?? throw new PamelloControllerException(NotFound($"Could not find single entity by \"{query}\" query"))) :
                        Ok(IPamelloEntity.GetIds(results));
                case EDtoView.Detailed: {
                    if (!single) Ok(results.Select((x) => new DtoDescription(x)));
                    
                    var first = results.FirstOrDefault();
                    if (first is null) throw new PamelloControllerException(NotFound($"Could not find single entity by \"{query}\" query"));

                    return Ok(new DtoDescription(first));
                }
                default:
                    Debug.Assert(false, "Impossible case");
                    return BadRequest();
            }
        }
    }
}
