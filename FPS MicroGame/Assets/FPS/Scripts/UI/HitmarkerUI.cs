using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.FPS.Game;

namespace Unity.FPS.UI
{
    public class HitmarkerUI : MonoBehaviour
    {
        public Image HitmarkerImage;
        public float FlashDuration = 0.15f;
        public float MaxAlpha = 1f;

        Coroutine m_FlashCoroutine;

        void Awake()
        {
            // try to auto-assign the image if the designer didn't set it in the inspector
            if (HitmarkerImage == null)
                HitmarkerImage = GetComponentInChildren<Image>();

            // make sure the image is hidden at start
            SetAlpha(0f);
        }

        void OnEnable()
        {
            DamageEvents.OnAnyDamage += OnAnyDamage;
        }

        void OnDisable()
        {
            DamageEvents.OnAnyDamage -= OnAnyDamage;
            // ensure the hitmarker is hidden when this component is disabled
            SetAlpha(0f);
        }

        void OnAnyDamage(float damage, GameObject damageSource)
        {
            // Only show hitmarker when this client (player) is the damage source
            // The projectile/weapon system sets the Owner as the shooter GameObject
            GameObject player = FindLocalPlayer();
            if (damageSource == player)
            {
                if (m_FlashCoroutine != null)
                    StopCoroutine(m_FlashCoroutine);

                m_FlashCoroutine = StartCoroutine(Flash());
            }
        }

        IEnumerator Flash()
        {
            if (HitmarkerImage == null)
                yield break;

            float t = 0f;
            Color c = HitmarkerImage.color;
            c.a = MaxAlpha;
            HitmarkerImage.color = c;

            while (t < FlashDuration)
            {
                t += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(MaxAlpha, 0f, t / FlashDuration);
                c.a = alpha;
                HitmarkerImage.color = c;
                yield return null;
            }

            c.a = 0f;
            HitmarkerImage.color = c;
            m_FlashCoroutine = null;
        }

        void SetAlpha(float a)
        {
            if (HitmarkerImage == null)
                return;

            Color c = HitmarkerImage.color;
            c.a = a;
            HitmarkerImage.color = c;
        }

        GameObject FindLocalPlayer()
        {
            // Try to find a PlayerWeaponsManager and return its GameObject
            var pwm = FindObjectOfType<Unity.FPS.Gameplay.PlayerWeaponsManager>();
            if (pwm)
                return pwm.gameObject;

            return null;
        }
    }
}
