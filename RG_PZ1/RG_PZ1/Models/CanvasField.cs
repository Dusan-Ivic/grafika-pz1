using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG_PZ1.Models
{
    public class CanvasField
    {
        public int IndexX { get; set; }
        public int IndexY { get; set; }
        public List<CanvasPowerEntity> Entities { get; set; }
    }
}
