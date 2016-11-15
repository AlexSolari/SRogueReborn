using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                return this == (Point)obj;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return X + Y + X ^ Y;
        }

        public static bool operator!=(Point p1, Point p2)
        {
            return !(p1==p2);
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return (p1.X == p2.X && p1.Y == p2.Y);
        }
    }
}
