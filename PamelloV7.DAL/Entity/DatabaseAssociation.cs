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
    public class DatabaseAssociation
    {
        public string Association;
        public DatabaseSong Song { get; set; }

        public static string[] ReservedNames = ["current", "random"];
        public static char[] ReservedSymbols = [':', '[', ']'];

        public static bool IsReserved(string association) {
            if (ReservedNames.Contains(association)) return true;
            if (association.Any(c => ReservedSymbols.Contains(c))) return true;
            
            return false;
        }
        public static void EnsureNotReserved(string association) {
            if (ReservedNames.Contains(association)) throw new PamelloException($"Association \"{association}\" is reserved");
            if (association.Any(c => ReservedSymbols.Contains(c))) throw new PamelloException($"Association \"{association}\" contains reserved symbols");
        }
    }
}
