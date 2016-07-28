using SRogue.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Entities.Interfaces
{
    public interface IMovable
    {
        void Move(Direction direction);
        void Move(Direction direction, int count);
        void MoveInstantly(int x, int y);
    }
}
