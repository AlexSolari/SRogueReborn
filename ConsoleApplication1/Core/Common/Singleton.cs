﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRogue.Core.Common
{
    public class Singleton<T>
    {
        private static T _instance;

        static Singleton()
        {

        }

        public static T Instance 
        {
            get
            {
                if (_instance == null)
                    _instance = Activator.CreateInstance<T>();

                return _instance;
            }
        }
    }
}
