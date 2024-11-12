using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.DAL.Entity
{
    public class DatabaseAssociacion
    {
        public string Associacion;
        public DatabaseSong Song { get; set; }
    }
}
