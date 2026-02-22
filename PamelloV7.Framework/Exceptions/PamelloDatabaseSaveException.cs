using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Framework.Exceptions
{
    public class PamelloDatabaseSaveException : PamelloException
    {
        public PamelloDatabaseSaveException() : base("Null reference database save exception") { }
    }
}
