using UnityEngine;
using Unity.FPS.Game;

namespace Unity.FPS.UI
{
    // Simple test helper: press H in Play mode to simulate dealing damage and show the hitmarker
    public class HitmarkerTester : MonoBehaviour
    {
        public float TestDamage = 10f;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                // broadcast damage with this GameObject as the source
                DamageEvents.OnAnyDamage?.Invoke(TestDamage, gameObject);
            }
        }
    }
}
