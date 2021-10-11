using System;
using DG.Tweening;
using PinkBlob.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PinkBlob.Gameplay.Suck
{
    public class Suckable : MonoBehaviour
    {
        private const float SuckTime = 0.2f;
        private const float ResetTime = 1f;

        private float suckTimer = 0;
        private float resetTimer = 0;
        
        [SerializeField]
        private Transform visual = default;
        
        [Min(0)]
        [SerializeField]
        private float suckHealth = 3f;

        private float currentSuckHealth;

        [Min(0)]
        [SerializeField]
        private float inhaleTime = 0.2f;

        [SerializeField]
        private Ease inhaleEase = Ease.InQuint;

        private bool isInhaled = false;

        [SerializeField]
        private SuckObject suckObject = default;

        [Title("Shake")]

        [Min(0)]
        [SerializeField]
        private float intensity = 1f;

        [Min(0)]
        [SerializeField]
        private float speed = 1f;

        private Vector3 shakeSeed;

        private void Awake()
        {
            currentSuckHealth = suckHealth;
            shakeSeed = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
        }

        private void Update()
        {
            if (resetTimer > 0)
            {
                resetTimer -= Time.deltaTime;

                if (resetTimer <= 0)
                {
                    currentSuckHealth = suckHealth;
                    resetTimer = 0;

                    visual.localPosition = Vector3.zero;
                }
            }
        }

        public void Suck(Vector3 mouth, Action<SuckObject> inhaledCallback)
        {
            if (isInhaled)
            {
                return;
            }

            resetTimer = ResetTime;
            suckTimer -= Time.deltaTime;

            float randX = Mathf.Lerp(-1, 1, Mathf.PerlinNoise((Time.time * speed) + shakeSeed.x, (Time.deltaTime * speed) + shakeSeed.x));
            float randY = Mathf.Lerp(-1, 1, Mathf.PerlinNoise((Time.time * speed) + shakeSeed.y, (Time.deltaTime * speed) + shakeSeed.y));
            float randZ = Mathf.Lerp(-1, 1, Mathf.PerlinNoise((Time.time * speed) + shakeSeed.z, (Time.deltaTime * speed) + shakeSeed.z));

            var shake = new Vector3(randX * intensity, randY * intensity, randZ * intensity);

            visual.localPosition = shake;

            if (suckTimer <= 0)
            {
                suckTimer = SuckTime;
                currentSuckHealth--;

                if (currentSuckHealth <= 0)
                {
                    isInhaled = true;

                    transform.DOMove(mouth, inhaleTime)
                             .SetEase(inhaleEase)
                             .OnComplete(() =>
                                         {
                                             Destroy(gameObject);
                                             inhaledCallback?.Invoke(suckObject);
                                         });
                }
            }
        }
    }
}
