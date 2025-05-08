using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.DAL.Entity
{
    public class DatabaseAssociacion
    {
        public string Associacion;
        public DatabaseSong Song { get; set; }

        public static string[] ReservedNames = ["current", "random"];
        public static char[] ReservedSymbols = [':', '[', ']'];

        public static bool IsReserved(string associacion) {
            if (ReservedNames.Contains(associacion)) return true;
            if (associacion.Any(c => ReservedSymbols.Contains(c))) return true;
            
            return false;
        }
        public static void EnsureNotReserved(string associacion) {
            if (ReservedNames.Contains(associacion)) throw new PamelloException($"Association \"{associacion}\" is reserved");
            if (associacion.Any(c => ReservedSymbols.Contains(c))) throw new PamelloException($"Association \"{associacion}\" contains reserved symbols");
        }
    }
}
