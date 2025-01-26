using System.Collections;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.FPS.AI
{
    [RequireComponent(typeof(EnemyController))]
    public class EnemyMobile : MonoBehaviour
    {

        public Animator Animator;

        [Tooltip("Fraction of the enemy's attack range at which it will stop moving towards target while attacking")]
        [Range(0f, 1f)]
        public float AttackStopDistanceRatio = 0.5f;

        [Tooltip("Registers attack for the attack cooldown timer")]
        public float LastAttack;

        [Tooltip("Captures the time the enemy was spawned in")]
        public float spawnTime;
        
        [Tooltip("The random hit damage effects")]
        public ParticleSystem[] RandomHitSparks;

        public ParticleSystem[] OnDetectVfx;
        public AudioClip OnDetectSfx;

        [Header("Sound")] public AudioClip MovementSound;
        public MinMaxFloat PitchDistortionMovementSpeed;
        
        EnemyController m_EnemyController;
        AudioSource m_AudioSource;
        private EnemyManager m_EnemyManager;

        const string k_AnimMoveSpeedParameter = "MoveSpeed";
        const string k_AnimAttackParameter = "Attack";
        const string k_AnimAlertedParameter = "Alerted";
        const string k_AnimOnDamagedParameter = "OnDamaged";
        
        // manages movement state
        private string direction = "";

        void Start()
        {
            m_EnemyController = GetComponent<EnemyController>();
            DebugUtility.HandleErrorIfNullGetComponent<EnemyController, EnemyMobile>(m_EnemyController, this,
                gameObject);

            m_EnemyController.onAttack += OnAttack;
            m_EnemyController.onDamaged += OnDamaged;
            

            // adding a audio source to play the movement sound on it
            m_AudioSource = GetComponent<AudioSource>();
            DebugUtility.HandleErrorIfNullGetComponent<AudioSource, EnemyMobile>(m_AudioSource, this, gameObject);
            m_AudioSource.clip = MovementSound;
            m_AudioSource.Play();
            
            // detects initial position of the entity
            Vector3 initialPosition = transform.position;
            direction = m_EnemyController.MoveBasedOnAxisPosition(initialPosition);
        }

        void Update()
        {
            // Perform movement pattern
            m_EnemyController.MoveEnemy(direction);
            // perform attack manoeuvre 
            float currentTime = LastAttack += Time.deltaTime;
            if (m_EnemyController.AttackDelay(currentTime))
            {
                LastAttack = 0f;
                m_EnemyController.TryAtack();
            }
            
        }
        

        void OnAttack()
        {
            Animator.SetTrigger(k_AnimAttackParameter);
            
        }
        

        void OnDamaged()
        {
            if (RandomHitSparks.Length > 0)
            {
                int n = Random.Range(0, RandomHitSparks.Length - 1);
                RandomHitSparks[n].Play();
            }

            Animator.SetTrigger(k_AnimOnDamagedParameter);
        }
    }
}