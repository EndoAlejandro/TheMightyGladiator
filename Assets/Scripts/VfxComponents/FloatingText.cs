using System;
using System.Collections;
using Pooling;
using TMPro;
using UnityEngine;

namespace VfxComponents
{
    public class FloatingText : PooledMonoBehaviour
    {
        [SerializeField] private float animationVelocity = 0.25f;
        [SerializeField] private float fadeVelocity = 0.5f;

        [SerializeField] private Color normalColor;
        [SerializeField] private Color criticalColor;

        private TMP_Text _text;

        private void Awake() => _text = GetComponentInChildren<TMP_Text>();

        private void Start()
        {
            StartCoroutine(Sample());
        }

        private IEnumerator Sample()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);
                transform.position = Vector3.zero;
                Setup("666", false);
            }
        }

        public void Setup(string text, bool isCritical)
        {
            _text.color = isCritical ? criticalColor : normalColor;
            _text.SetText(text);
            StartCoroutine(HitTextAnimationSequence());
        }

        private IEnumerator HitTextAnimationSequence()
        {
            yield return FloatingAnimation();
            yield return FadeAnimation();
            ReturnToPool();
        }

        private IEnumerator FloatingAnimation()
        {
            var t = _text.transform;
            t.localScale = Vector3.zero;

            var timeElapsed = 0f;
            var normalizedTime = 0f;
            var targetPosition = transform.position + Vector3.up * .25f;
            while (timeElapsed < animationVelocity)
            {
                timeElapsed += Time.deltaTime;
                normalizedTime = timeElapsed / animationVelocity;
                t.localScale = Vector3.Lerp(t.localScale, Vector3.one, normalizedTime);

                transform.position = Vector3.Lerp(transform.position, targetPosition, normalizedTime);
                yield return null;
            }
        }

        private IEnumerator FadeAnimation()
        {
            var timeElapsed = 0f;
            var normalizedTime = 0f;
            while (timeElapsed < fadeVelocity)
            {
                timeElapsed += Time.deltaTime;
                normalizedTime = timeElapsed / fadeVelocity;
                _text.alpha = Mathf.Lerp(_text.alpha, 0f, normalizedTime);
                yield return null;
            }
        }
    }
}