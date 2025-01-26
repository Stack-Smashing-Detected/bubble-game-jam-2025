using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Animate : MonoBehaviour
{
    private const string kXPosString = "pos_X";
    private const string kYPosString = "pos_Y";

    [SerializeField] private GameObject m_CharacterAndCameraController;
    [SerializeField] private bool m_DoHackySpriteFlip = false;
    
    private Transform m_MovingCharacter;
    private CharacterController m_CharacterController; 
    
    private Animator m_Animator;

    private float m_XBound = 10f;
    private float m_YBound = 10f;
    private float m_InitialYOffset = 0f;

    private float m_XNormalised = 0f;
    private float m_YNormalised = 0f;
    
    
    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_MovingCharacter = m_CharacterAndCameraController.transform.Find("CharacterHolder");
        m_CharacterController = m_CharacterAndCameraController.GetComponent<CharacterController>();
    }

    void Start()
    {
        m_XBound = m_CharacterController.GetHorizontalBound();
        m_YBound = m_CharacterController.GetVerticalBound();
    }
    
    // Update is called once per frame
    void Update()
    {
        m_XBound = m_CharacterController.GetHorizontalBound();
        m_YBound = m_CharacterController.GetVerticalBound();
        
        m_XNormalised = m_MovingCharacter.transform.position.x / m_XBound;
        m_XNormalised = m_DoHackySpriteFlip? m_XNormalised : -m_XNormalised;    // Quick horizontal flip if needed
        
        m_YNormalised = (m_MovingCharacter.transform.position.y - m_InitialYOffset) / m_YBound;
        
        m_Animator.SetFloat(kXPosString, m_XNormalised);
        m_Animator.SetFloat(kYPosString, m_YNormalised);
    }
}
