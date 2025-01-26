using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    private int m_Score = 0;
    private float m_BulletFrequencyPercent = 50f;
    [SerializeField] private float m_MaxAirSeconds = 90f;
    [SerializeField] private float m_AirRemainingSeconds= 90f;
    [SerializeField] private float m_SecondsOfAirToIncrease = 5f;
    [SerializeField] private float m_SecondsOfAirToRemove = 5f;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_AirRemainingSeconds -= Time.deltaTime;
        
        // to do 
        if (m_AirRemainingSeconds <= 0)
        {
            // do game over
        }
    }

    public void IncreaseAir()
    {
        AdjustAir(m_SecondsOfAirToIncrease);
    }

    public void DecreaseAir()
    {
        AdjustAir(m_SecondsOfAirToRemove);
    }

    public void AdjustAir(float secondsAdjustment)
    {
        m_AirRemainingSeconds += secondsAdjustment;
        ValidateAir();
    }
    
    public void IncreaseScore(int scoreToAdd = 10)
    {
        m_Score += scoreToAdd;
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
    
    
    
}
