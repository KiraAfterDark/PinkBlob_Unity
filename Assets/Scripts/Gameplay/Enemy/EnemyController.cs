using System;
using System.Collections.Generic;
using DG.Tweening;
using PinkBlob.Gameplay.Ability;
using PinkBlob.Gameplay.Ai.StateMachine;
using PinkBlob.Gameplay.Suck;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob.Gameplay.Enemy
{
    [RequireComponent(typeof(StateMachine))]
    public class EnemyController : MonoBehaviour, ISuckable
    {
        public event Action<AbilityType> CompleteSucking;

        private Rigidbody rigidbody;
        
        public StateMachine StateMachine => stateMachine;
        private StateMachine stateMachine;
        
        // Suckable
        private bool isInhaled = false;
        
        private const float SuckTime = 0.2f;
        private float suckTimer = 0;

        [Title("Suckable Properties")]

        [Min(0)]
        [SerializeField]
        private int suckHealth = 5;
        private int currentSuckHealth;
        
        [SerializeField]
        private AbilityType abilityType = AbilityType.Normal;

        private bool isSucking;

        [Min(0)]
        [SerializeField]
        private float inhaleTime = 0.25f;

        [SerializeField]
        private Ease inhaleEase = Ease.Linear;
        
        [Title("Shake")]
        
        [Min(0)]
        [SerializeField]
        private float intensity = 0.1f;

        [Min(0)]
        [SerializeField]
        private float speed = 10f;

        private Vector3 shakeSeed;

        [Title("Visuals")]

        [SerializeField]
        private Transform visual;

        [Title("Colliders")]
        
        [SerializeField]
        private List<Collider> colliders = new List<Collider>();

        private void Awake()
        {
            stateMachine = GetComponent<StateMachine>();
            rigidbody = GetComponent<Rigidbody>();
            
            isSucking = false;
        }

        private void Start()
        {
            currentSuckHealth = suckHealth;
            GameplayController.Instance.SpawnEnemy(this);
        }

        public void EnterSucking()
        {
            isSucking = true;
        }

        public void UpdateSucking(Vector3 source)
        {
            if (isInhaled)
            {
                return;
            }

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
                    CompleteSuck(source);
                }
            }
        }

        public void ExitSucking()
        {
            isSucking = false;
            currentSuckHealth = suckHealth;
            visual.position = Vector3.zero;
        }
        
        private void CompleteSuck(Vector3 destination)
        {
            isInhaled = true;

            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }

            transform.DOMove(destination, inhaleTime)
                     .SetEase(inhaleEase)
                     .OnComplete(() =>
                                 {
                                     CompleteSucking?.Invoke(abilityType);
                                     Destroy(gameObject);
                                 });
            
            transform.DOScale(Vector3.one * 0.1f, inhaleTime).SetEase(inhaleEase);
        }

        public bool IsSucking()
        {
            return isSucking;
        }
    }
}
