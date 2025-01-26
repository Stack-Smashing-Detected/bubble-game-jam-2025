using System.Collections;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.FPS.AI
{
    [RequireComponent(typeof(EnemyController))]
    public class EnemyBehaviour : MonoBehaviour
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
        
        EnemyController _mEnemyController;
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
            _mEnemyController = GetComponent<EnemyController>();
            DebugUtility.HandleErrorIfNullGetComponent<EnemyController, EnemyBehaviour>(_mEnemyController, this,
                gameObject);

            _mEnemyController.onAttack += OnAttack;
            _mEnemyController.onDamaged += OnDamaged;
            

            // adding a audio source to play the movement sound on it
            m_AudioSource = GetComponent<AudioSource>();
            DebugUtility.HandleErrorIfNullGetComponent<AudioSource, EnemyBehaviour>(m_AudioSource, this, gameObject);
            m_AudioSource.clip = MovementSound;
            m_AudioSource.Play();
            
            // detects initial position of the entity
            Vector3 initialPosition = transform.position;
            direction = _mEnemyController.MoveBasedOnAxisPosition();
        }

        void Update()
        {
            // Perform movement pattern
            _mEnemyController.MoveEnemy(direction);
            // perform attack manoeuvre 
            float currentTime = LastAttack += Time.deltaTime;
            if (_mEnemyController.AttackDelay(currentTime))
            {
                LastAttack = 0f;
                _mEnemyController.TryAtack();
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