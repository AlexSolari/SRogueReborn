using SRogue.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Entities.Interfaces
{
    public interface IUnit : IPositionable, IDisplayable, IDamageble, IEntity, IMovable
    {
        float Health { get; set; }
        float HealthMax { get; set; }
        int Armor { get; set; }
        int MagicResist { get; set; }
        float DecreaseDamage(float pureDamage, DamageType type);
        int Attack { get; set; }

        int SummarizeAttack();
        int SummarizeArmor();
        int SummarizeResist();
    }
}
