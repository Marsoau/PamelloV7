using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Core.Exceptions
{
    public class PamelloDatabaseSaveException : PamelloException
    {
        public PamelloDatabaseSaveException() : base("Null reference database save exception") { }
    }
}
