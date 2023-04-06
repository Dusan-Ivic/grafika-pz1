using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG_PZ1.Commands
{
    public interface IDrawCommand
    {
        void Execute();
        void Undo();
        void Redo();
    }
}
