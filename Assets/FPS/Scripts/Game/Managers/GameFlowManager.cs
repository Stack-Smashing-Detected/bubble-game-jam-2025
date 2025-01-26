using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
//using TMPro;


namespace Unity.FPS.Game
{
    public class GameFlowManager : MonoBehaviour
    {
        [Header("Parameters")] [Tooltip("Duration of the fade-to-black at the end of the game")]
        public float EndSceneLoadDelay = 3f;

        [Tooltip("The canvas group of the fade-to-black screen")]
        public CanvasGroup EndGameFadeCanvasGroup;

        [Header("Win")] [Tooltip("This string has to be the name of the scene you want to load when winning")]
        public string WinSceneName = "WinScene";

        [Tooltip("Duration of delay before the fade-to-black, if winning")]
        public float DelayBeforeFadeToBlack = 4f;

        [Tooltip("Win game message")]
        public string WinGameMessage;
        [Tooltip("Duration of delay before the win message")]
        public float DelayBeforeWinMessage = 2f;

        [Tooltip("Sound played on win")] public AudioClip VictorySound;

        [Header("Lose")] [Tooltip("This string has to be the name of the scene you want to load when losing")]
        public string LoseSceneName = "LoseScene";


        public bool GameIsEnding { get; private set; }

        float m_TimeLoadEndGameScene;
        string m_SceneToLoad;

        public GameObject player;
        public List<GameObject> enemies = new List<GameObject>();
        public List<GameObject> enemyPrefabs = new List<GameObject>();
        public float spawnDistance;
        public float spawnRange;
        public float spawnHeightRange;
        public float timeUntilSpawn = 0.5f;
        public float timeUntilSpawnDefault = 0.5f;
        public int gameScore;
        public GameObject floorChunk;
        public List<GameObject> floorChunks = new List<GameObject>();
        public float timeSinceStart;
        public int secondsUntilGameEnds;
        //public TextMeshProUGUI score;



        void Awake()
        {
            EventManager.AddListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventManager.AddListener<PlayerDeathEvent>(OnPlayerDeath);
        }

        void Start()
        {
            AudioUtility.SetMasterVolume(1);
        }

        void Update()
        {
            //score.text = gameScore.ToString();

            //for (int i = 0; i < )

            timeSinceStart += Time.deltaTime;

            if (timeSinceStart > secondsUntilGameEnds)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Victory");
            }
                
            if (GameIsEnding)
            {
                float timeRatio = 1 - (m_TimeLoadEndGameScene - Time.time) / EndSceneLoadDelay;
                EndGameFadeCanvasGroup.alpha = timeRatio;

                AudioUtility.SetMasterVolume(1 - timeRatio);

                // See if it's time to load the end scene (after the delay)
                if (Time.time >= m_TimeLoadEndGameScene)
                {
                    SceneManager.LoadScene(m_SceneToLoad);
                    GameIsEnding = false;
                }
            }

            if (timeUntilSpawn <= 0f)
            {
                GameObject enemyToSpawn = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)];
                GameObject temp = Instantiate(enemyToSpawn, new Vector3(UnityEngine.Random.Range(-spawnRange, spawnRange), UnityEngine.Random.Range(-3f, spawnHeightRange - 5f), player.transform.position.z + spawnDistance), Quaternion.identity);
                //Destroy(temp, 12f);
                enemies.Add(temp);
                timeUntilSpawn = timeUntilSpawnDefault - (90 - secondsUntilGameEnds) * 0.015f;
                //spawn random enemy
            }
            else
            {
                timeUntilSpawn -= Time.deltaTime;
                //reduce timeUntilSpawn
            }

            if (floorChunks.Count <= 1)
            {
                floorChunks.Add(Instantiate(floorChunk, new Vector3(floorChunks[0].transform.position.x, -5, floorChunks[0].transform.position.z + 90), Quaternion.identity));
                //add another 90 x in front
            }
            else
            {
                int removeIndex = -1;
                for (int i = 0; i < floorChunks.Count; i++)
                {
                    if (Mathf.Abs(floorChunks[i].transform.position.z - player.transform.position.z) > 100)
                    {
                        removeIndex = i;
                    }
                }
                if (removeIndex != -1)
                {
                    GameObject toRemove = floorChunks[removeIndex];
                    floorChunks.Remove(toRemove);
                    Destroy(toRemove);
                }
                //if distance from either floorChunk is far away remove it and delete object
            }

        }

        void OnAllObjectivesCompleted(AllObjectivesCompletedEvent evt) => EndGame(true);
        void OnPlayerDeath(PlayerDeathEvent evt) => EndGame(false);

        void EndGame(bool win)
        {
            // unlocks the cursor before leaving the scene, to be able to click buttons
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Remember that we need to load the appropriate end scene after a delay
            GameIsEnding = true;
            EndGameFadeCanvasGroup.gameObject.SetActive(true);
            if (win)
            {
                m_SceneToLoad = WinSceneName;
                m_TimeLoadEndGameScene = Time.time + EndSceneLoadDelay + DelayBeforeFadeToBlack;

                // play a sound on win
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = VictorySound;
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.HUDVictory);
                audioSource.PlayScheduled(AudioSettings.dspTime + DelayBeforeWinMessage);

                // create a game message
                //var message = Instantiate(WinGameMessagePrefab).GetComponent<DisplayMessage>();
                //if (message)
                //{
                //    message.delayBeforeShowing = delayBeforeWinMessage;
                //    message.GetComponent<Transform>().SetAsLastSibling();
                //}

                DisplayMessageEvent displayMessage = Events.DisplayMessageEvent;
                displayMessage.Message = WinGameMessage;
                displayMessage.DelayBeforeDisplay = DelayBeforeWinMessage;
                EventManager.Broadcast(displayMessage);
            }
            else
            {
                m_SceneToLoad = LoseSceneName;
                m_TimeLoadEndGameScene = Time.time + EndSceneLoadDelay;
            }
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventManager.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
        }
    }
}