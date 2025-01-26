using NUnit.Framework.Constraints;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public delegate void AirChangedDelegate(float airAdjustment);
    public delegate void ScoreChangedDelegate(int scoreIncrease);

    public event AirChangedDelegate OnGainedOrLostAir;
    public event ScoreChangedDelegate OnGainedScore;
    
    private int m_Score = 0;
    private float m_BulletFrequencyPercent = 50f;
    private float m_BulletFrequencyAdjustment = 10f;
    private float m_MinBulletFrequencyPercent = 10f;
    private float m_MaxBulletFrequencyPercent = 100f;
    
    [SerializeField] private float m_MaxAirSeconds = 90f;
    [SerializeField] private float m_AirRemainingSeconds= 90f;
    [SerializeField] private float m_SecondsOfAirToIncrease = 5f;
    [SerializeField] private float m_SecondsOfAirToRemove = 5f;
    [SerializeField] private bool m_AreDebugKeysOn = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if (m_AreDebugKeysOn)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                IncreaseAir();
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                DecreaseAir();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                IncreaseScore();
            }
        }

        AdjustAir(-Time.deltaTime);
        OnGainedOrLostAir?.Invoke(m_AirRemainingSeconds/m_MaxAirSeconds);

        if (m_AirRemainingSeconds <= 0f)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Restart");
        }
    }

    public void IncreaseAir()
    {
        Debug.Log("IncreaseAir");
        AdjustAir(m_SecondsOfAirToIncrease);
        OnGainedOrLostAir?.Invoke(m_AirRemainingSeconds/m_MaxAirSeconds);
    }

    public void DecreaseAir()
    {
        AdjustAir(-m_SecondsOfAirToRemove);
        OnGainedOrLostAir?.Invoke(m_AirRemainingSeconds/m_MaxAirSeconds);
    }

    public void AdjustAir(float secondsAdjustment)
    {
        m_AirRemainingSeconds += secondsAdjustment;
        ValidateAir();
    }
    
    private void ValidateAir()
    {
        if (m_AirRemainingSeconds <= 0)
        {
            //to do
            // DoGameOver();
        }
        else if (m_AirRemainingSeconds > m_MaxAirSeconds)
        {
            m_AirRemainingSeconds = m_MaxAirSeconds;
        }
    }
    
    public void IncreaseScore(int scoreToAdd = 10)
    {
        Debug.Log("IncreaseScore");
        m_Score += scoreToAdd;
        OnGainedScore?.Invoke(m_Score);
    }

    public void IncreaseBulletFrequency()
    {
        AdjustBulletFrequencyPercent(m_BulletFrequencyPercent);
    }

    public void DecreaseBulletFrequency()
    {
        AdjustBulletFrequencyPercent(-m_BulletFrequencyPercent);
    }

    public void AdjustBulletFrequencyPercent(float percentIncrease)
    {
        m_BulletFrequencyPercent += percentIncrease;
        ValidateBulletFrequency();
    }
    
    private void ValidateBulletFrequency()
    {
        if (m_BulletFrequencyPercent <= m_MinBulletFrequencyPercent)
        {
            m_BulletFrequencyPercent = m_MinBulletFrequencyPercent;
        }
        else if (m_BulletFrequencyPercent > 100f)
        {
            m_BulletFrequencyPercent = 100f;
        }
    }
}
