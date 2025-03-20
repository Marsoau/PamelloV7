using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Client.Pages.Refreshes
{
    public interface IRefrashable
    {
        public Task Update();
        public void Refresh();
    }
}
