using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private CharacterStatus m_CharacterStatus;
    [SerializeField] private Slider m_AirSlider;
    [SerializeField] private TextMeshProUGUI m_ScoreText;
    [SerializeField] private string m_ScorePrefixText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScore(0);
        m_CharacterStatus.OnGainedOrLostAir += UpdateNormalizedAirSlider;
        m_CharacterStatus.OnGainedScore += UpdateScore;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void UpdateNormalizedAirSlider(float newAirValue)
    {
        m_AirSlider.value = newAirValue;
    }

    void UpdateScore(int newScore)
    {
        m_ScoreText.text = m_ScorePrefixText + " " + newScore.ToString();
    }
    
    
}
