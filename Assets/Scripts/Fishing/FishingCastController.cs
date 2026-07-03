using System;
using System.Collections;
using UnityEngine;

namespace AfterBlue.Fishing
{
    public sealed class FishingCastController : MonoBehaviour
    {
        [SerializeField] private float castDistance = 2.2f;
        [SerializeField] private float castHeight = 0.8f;
        [SerializeField] private float castDuration = 0.45f;
        [SerializeField] private float waterHeight = 0.22f;
        [SerializeField] private Vector3 rodTipLocalPosition = new Vector3(0.42f, 0.72f, 0.62f);

        private Bobber activeBobber;
        private Coroutine castRoutine;

        public Bobber ActiveBobber => activeBobber;
        public Transform ActiveBobberTransform => activeBobber != null ? activeBobber.transform : null;

        public Transform EnsureRodTip(Transform playerBoat)
        {
            Transform existing = playerBoat.Find("RodTip");
            if (existing != null)
            {
                return existing;
            }

            GameObject rodTip = new GameObject("RodTip");
            rodTip.transform.SetParent(playerBoat, false);
            rodTip.transform.localPosition = rodTipLocalPosition;
            return rodTip.transform;
        }

        public void Cast(Transform playerBoat, Transform rodTip, Action<Bobber> onLanded)
        {
            if (castRoutine != null)
            {
                StopCoroutine(castRoutine);
            }

            Retrieve();
            activeBobber = Bobber.CreateDefault("Bobber");
            activeBobber.transform.position = rodTip.position;

            Vector3 castDirection = playerBoat.forward;
            castDirection.y = 0f;
            if (castDirection.sqrMagnitude < 0.001f)
            {
                castDirection = Vector3.forward;
            }

            castDirection.Normalize();
            Vector3 target = playerBoat.position + castDirection * castDistance;
            target.y = waterHeight;
            castRoutine = StartCoroutine(CastRoutine(rodTip.position, target, onLanded));
        }

        public void Retrieve()
        {
            if (castRoutine != null)
            {
                StopCoroutine(castRoutine);
                castRoutine = null;
            }

            if (activeBobber != null)
            {
                Destroy(activeBobber.gameObject);
                activeBobber = null;
            }
        }

        private IEnumerator CastRoutine(Vector3 start, Vector3 target, Action<Bobber> onLanded)
        {
            float elapsed = 0f;

            while (elapsed < castDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / castDuration);
                Vector3 position = Vector3.Lerp(start, target, t);
                position.y += Mathf.Sin(t * Mathf.PI) * castHeight;
                activeBobber.transform.position = position;
                yield return null;
            }

            activeBobber.SetWaterPosition(target);
            castRoutine = null;
            onLanded?.Invoke(activeBobber);
        }
    }
}
