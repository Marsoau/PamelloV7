using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using PamelloV7.Core.Services;
using PamelloV7.Server.Controllers.Base;
using PamelloV7.Server.Exceptions;

namespace PamelloV7.Server.Controllers;

[ApiController]
public class FilesController : PamelloControllerBase
{
    private readonly IFileAccessService _files;

    public FilesController(IServiceProvider services) : base(services) {
        _files = services.GetRequiredService<IFileAccessService>();
    }

    [HttpGet("Files/{*path}")]
    public async Task<IActionResult> GetFile(string path) {
        if (!path.StartsWith("Public")) RequireUser();
        
        var file = _files.GetFile($"/{path}");
        if (file is null || !file.Exists) throw new PamelloControllerException(NotFound($"File \"{path}\" not found"));

        var typeProvider = new FileExtensionContentTypeProvider();
        if (!typeProvider.TryGetContentType(file.Name, out var contentType)) {
            contentType = "application/octet-stream"; 
        }

        var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        
        return File(fs, contentType);
    }
}
