﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Entities.Interfaces
{
    public interface ILootable
    {
        bool DroppedLoot { get; set; }
        void DropLoot();
    }
}
