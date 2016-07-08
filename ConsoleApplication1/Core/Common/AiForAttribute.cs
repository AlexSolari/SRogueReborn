using SRogue.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true, Inherited=true)]
    public class AiForAttribute : Attribute
    {
        public Type Target { get; private set; }

        public AiForAttribute(Type target)
        {
            Target = target;
        }
    }
}
