using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
namespace Leblebi
{
    public class Loader
    {
        private static GameObject loadObj;
        public static void Load()
        {
            loadObj = new GameObject();
            loadObj.AddComponent<Hacks>();
            UnityEngine.Object.DontDestroyOnLoad(loadObj);
        }

        public static void Unload()
        {
            _Unload();
        }

        public static void _Unload()
        {
            GameObject.Destroy(loadObj);
        }
    }
}
