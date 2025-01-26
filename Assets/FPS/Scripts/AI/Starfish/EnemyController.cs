using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Unity.FPS.AI
{
    [RequireComponent(typeof(Health), typeof(Actor), typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        [System.Serializable]
        public struct RendererIndexData
        {
            public Renderer thisRenderer;
            [FormerlySerializedAs("MaterialIndex")] public int materialIndex;

            public RendererIndexData(Renderer renderer, int index)
            {
                thisRenderer = renderer;
                materialIndex = index;
            }
        }

        [Header("Parameters")]
        [Tooltip("The Y height at which the enemy will be automatically killed (if it falls off of the level)")]
        public float selfDestructYHeight = -20f;

        [Tooltip("The X distance at which the enemy will despawn")]
        public float xLimit = 2f;

        [Tooltip("The Z distance at which the enemy will despawn")]
        public float zLimit = 1f;
        
        [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
        public float pathReachingRadius = 2f;

        [Tooltip("The damage dealt to player when self destructs")]
        public float collisionDamage = 10f;
        
        [Tooltip("The speed in which the enemy traverses")]
        public float traverseSpeed = 9f;
        
        [Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
        public float deathDuration = 0f;
        
        [Header("Weapons Parameters")] [Tooltip("Allow weapon swapping for this enemy")]
        public bool swapToNextWeapon = false;
        
        [Tooltip("Time delay before this entity performs traversal sequence")]
        public float delayBeforeTraversal = 2.5f;

        [Tooltip("Time delay between a weapon swap and the next attack")]
        public float delayAfterWeaponSwap = 0f;

        [Tooltip("Time delay between enemy attacks")]
        public float delayBetweenAttacks = 2f;

        [Header("Flash on hit")] [Tooltip("The material used for the body of the entity")]
        public Material bodyMaterial;

        [Tooltip("The gradient representing the color of the flash on hit")] [GradientUsageAttribute(true)]
        public Gradient onHitBodyGradient;

        [Tooltip("The duration of the flash on hit")]
        public float flashOnHitDuration = 0.5f;

        [Header("Sounds")] [Tooltip("Sound played when recieving damages")]
        public AudioClip damageTick;

        [Tooltip("The VFX prefab spawned when the enemy dies")]
        public GameObject deathVfx;

        [Tooltip("The point at which the death VFX is spawned")]
        public Transform deathVfxSpawnPoint;

        [Header("Loot")] [Tooltip("The object this enemy can drop when dying")]
        public GameObject lootPrefab;

        [Tooltip("The chance the object has to drop")] [Range(0, 1)]
        public float dropRate = 1f;
        

        public UnityAction onAttack;
        
        public UnityAction onDamaged;

        List<RendererIndexData> m_BodyRenderers = new List<RendererIndexData>();
        MaterialPropertyBlock m_BodyFlashMaterialPropertyBlock;
        float m_LastTimeDamaged = float.NegativeInfinity;
        
        MaterialPropertyBlock m_EyeColorMaterialPropertyBlock;

        int m_PathDestinationNodeIndex;
        private Vector3 initialPosition;
        EnemyManager m_EnemyManager;
        ActorsManager m_ActorsManager;
        Health m_Health;
        Actor m_Actor;
        Collider[] m_SelfColliders;
        GameFlowManager m_GameFlowManager;
        bool m_WasDamagedThisFrame;
        float m_LastTimeWeaponSwapped = Mathf.NegativeInfinity;
        private float m_TimeSinceLastAttack = 0;
        int m_CurrentWeaponIndex;
        WeaponController m_CurrentWeapon;
        WeaponController[] m_Weapons;
        NavigationModule m_NavigationModule;
        private IEnumerator traversalRoutine;

        void Start()
        {
            m_EnemyManager = FindAnyObjectByType<EnemyManager>();
            DebugUtility.HandleErrorIfNullFindObject<EnemyManager, EnemyController>(m_EnemyManager, this);

            m_ActorsManager = FindAnyObjectByType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, EnemyController>(m_ActorsManager, this);

            m_EnemyManager.RegisterEnemy(this);

            m_Health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, EnemyController>(m_Health, this, gameObject);

            m_GameFlowManager = FindAnyObjectByType<GameFlowManager>();
            DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, EnemyController>(m_GameFlowManager, this);

            // Subscribe to damage & death actions
            m_Health.OnDie += OnDie;
            m_Health.OnDamaged += OnDamaged;
            
            // get entity's initial position
            initialPosition = transform.position;
            
            foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {

                    if (renderer.sharedMaterials[i] == bodyMaterial)
                    {
                        m_BodyRenderers.Add(new RendererIndexData(renderer, i));
                    }
                }
            }

            m_BodyFlashMaterialPropertyBlock = new MaterialPropertyBlock();
        }

        void Update()
        {
            EnsureWithinVerticalBounds();
            DespawnIfOutsideXZBounds();
            
            Color currentColor = onHitBodyGradient.Evaluate((Time.time - m_LastTimeDamaged) / flashOnHitDuration);
            m_BodyFlashMaterialPropertyBlock.SetColor("_EmissionColor", currentColor);
            foreach (var data in m_BodyRenderers)
            {
                data.thisRenderer.SetPropertyBlock(m_BodyFlashMaterialPropertyBlock, data.materialIndex);
            }
            m_WasDamagedThisFrame = false;
        }

        void EnsureWithinVerticalBounds()
        {
            // at every frame, this tests for conditions to kill the enemy
            if (transform.position.y < selfDestructYHeight)
            {
                Destroy(gameObject);
            }
        }

        void DespawnIfOutsideXZBounds()
        {
            // at every frame tests if the enemy is within the game boundaries.
            if (transform.position.x > xLimit)
            {
                m_EnemyManager.UnregisterDespawnedEnemy(this);
            }

            if (transform.position.x < -xLimit)
            {
                m_EnemyManager.UnregisterDespawnedEnemy(this);
            }

            if (transform.position.z > zLimit)
            {
                m_EnemyManager.UnregisterDespawnedEnemy(this);
            }

            if (transform.position.z < -zLimit)
            {
                m_EnemyManager.UnregisterDespawnedEnemy(this);
            }
        }
        
        void OnDamaged(float damage, GameObject damageSource)
        {
            // test if the damage source is the player
            if (damageSource && !damageSource.GetComponent<EnemyController>())
            {
                onDamaged?.Invoke();
                m_LastTimeDamaged = Time.time;
            
                // play the damage tick sound
                if (damageTick && !m_WasDamagedThisFrame)
                    AudioUtility.CreateSFX(damageTick, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);
            
                m_WasDamagedThisFrame = true;
            }
        }

        void OnDie()
        {
            // spawn a particle system when dying
            var vfx = Instantiate(deathVfx, deathVfxSpawnPoint.position, Quaternion.identity);
            Destroy(vfx, 5f);

            // tells the game flow manager to handle the enemy destuction
            m_EnemyManager.UnregisterDefeatedEnemy(this);

            // loot an object
            if (TryDropItem())
            {
                Instantiate(lootPrefab, transform.position, Quaternion.identity);
            }

            // this will call the OnDestroy function
            Destroy(gameObject, deathDuration);
        }

        public string MoveBasedOnAxisPosition()
        {
            Vector3 origin = new Vector3(0f, 0f, 0f);
            if (initialPosition.x > origin.x)
            {
                return "left";
            }

            return "right";
        }

        public void MoveEnemy(string direction)
        {
            if (direction == "left")
            {
                transform.Translate(Vector3.left * (traverseSpeed * Time.deltaTime));
            }

            if (direction == "right")
            {
                transform.Translate(Vector3.right * (traverseSpeed * Time.deltaTime));
            }
        }
        
        
        public bool TryAtack()
        {
            bool didFire = false;
            if (m_GameFlowManager.GameIsEnding)
                return false;

            if ((m_LastTimeWeaponSwapped + delayAfterWeaponSwap) >= Time.time)
                return false;

            didFire = GetCurrentWeapon().HandleShootInputs(false, true, false);

            if (didFire && onAttack != null)
            {
                onAttack.Invoke();

                if (swapToNextWeapon && m_Weapons.Length > 1)
                {
                    int nextWeaponIndex = (m_CurrentWeaponIndex + 1) % m_Weapons.Length;
                    SetCurrentWeapon(nextWeaponIndex);
                }
            }
            
            return didFire;
        }
        
        public bool AttackDelay(float timeSinceLastAttack){
            Debug.Log($"{timeSinceLastAttack}");
            if (timeSinceLastAttack >= delayBetweenAttacks)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        

        public bool TryDropItem()
        {
            if (dropRate == 0 || lootPrefab == null)
                return false;
            else if (dropRate == 1)
                return true;
            else
                return (Random.value <= dropRate);
        }

        void FindAndInitializeAllWeapons()
        {
            // Check if we already found and initialized the weapons
            if (m_Weapons == null)
            {
                m_Weapons = GetComponentsInChildren<WeaponController>();
                DebugUtility.HandleErrorIfNoComponentFound<WeaponController, EnemyController>(m_Weapons.Length, this,
                    gameObject);

                for (int i = 0; i < m_Weapons.Length; i++)
                {
                    m_Weapons[i].Owner = gameObject;
                }
            }
        }

        public WeaponController GetCurrentWeapon()
        {
            FindAndInitializeAllWeapons();
            // Check if no weapon is currently selected
            if (m_CurrentWeapon == null)
            {
                // Set the first weapon of the weapons list as the current weapon
                SetCurrentWeapon(0);
            }

            DebugUtility.HandleErrorIfNullGetComponent<WeaponController, EnemyController>(m_CurrentWeapon, this,
                gameObject);

            return m_CurrentWeapon;
        }

        void SetCurrentWeapon(int index)
        {
            m_CurrentWeaponIndex = index;
            m_CurrentWeapon = m_Weapons[m_CurrentWeaponIndex];
            if (swapToNextWeapon)
            {
                m_LastTimeWeaponSwapped = Time.time;
            }
            else
            {
                m_LastTimeWeaponSwapped = Mathf.NegativeInfinity;
            }
        }
    }
}