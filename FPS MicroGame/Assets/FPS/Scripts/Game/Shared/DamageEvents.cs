using System;
using UnityEngine;

namespace Unity.FPS.Game
{
    // Global damage events so UI or other systems can react when any damage occurs
    public static class DamageEvents
    {
        // parameters: damage amount, damage source GameObject
        public static Action<float, GameObject> OnAnyDamage;
    }
}
