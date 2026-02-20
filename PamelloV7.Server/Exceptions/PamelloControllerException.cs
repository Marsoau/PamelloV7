using Microsoft.AspNetCore.Mvc;
using PamelloV7.Framework.Exceptions;

namespace PamelloV7.Server.Exceptions
{
    public class PamelloControllerException : Exception
    {
        public IActionResult Result;

        public PamelloControllerException(IActionResult result) {
            Result = result;
        }
    }
}
