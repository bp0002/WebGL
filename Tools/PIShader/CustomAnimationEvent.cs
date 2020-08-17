using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PIShader
{
    public class CustomAnimationEvent : MonoBehaviour
    {
        public void func(string key)
        {
            Debug.LogWarning(key);
        }
    }
}
