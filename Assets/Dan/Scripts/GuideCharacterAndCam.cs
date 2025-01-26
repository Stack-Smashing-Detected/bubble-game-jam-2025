
using UnityEngine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
        
public class GuideCharacterAndCam : MonoBehaviour
{
    private GameObject m_Character;
    private Camera m_Camera;
    public GameObject groundthing;
    [SerializeField] private float m_TravelSpeed = 10f;
    [SerializeField] private float m_CameraTrailDistance = 3f;
    [SerializeField] private float m_StrafeSpeed = 4f;
    [SerializeField] private float m_HorizontalBound = 2f;
    [SerializeField] private float m_VerticalBound = 1f;
    public WeaponController playerGun;

    PlayerInputHandler m_InputHandler;
    // Add Player to the Actor List in Awake
    void Awake()
    {
        ActorsManager actorsManager = GetComponent<ActorsManager>();
        if (actorsManager != null)
        {
            actorsManager.SetPlayer(m_Character);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_InputHandler = GetComponent<PlayerInputHandler>();
        DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerCharacterController>(m_InputHandler,
            this, gameObject);

        m_Camera = transform.Find("CameraHolder").transform.Find("Camera").GetComponent<Camera>();
        m_Camera.enabled = true;
        m_Character = transform.Find("CharacterHolder").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input = m_InputHandler.GetMoveInput();
        
        m_Character.transform.position += new Vector3(m_StrafeSpeed * input.x, m_StrafeSpeed * input.z, m_TravelSpeed) * Time.deltaTime;

        groundthing.transform.position = new Vector3(groundthing.transform.position.x, groundthing.transform.position.y, m_Character.transform.position.z);

        

        // Keep character within bounds
        if (m_Character.transform.position.x > m_HorizontalBound)
        {
            m_Character.transform.position = new Vector3(m_HorizontalBound, m_Character.transform.position.y, m_Character.transform.position.z);
        }
        else if (m_Character.transform.position.x < -m_HorizontalBound)
        {
            m_Character.transform.position = new Vector3(-m_HorizontalBound, m_Character.transform.position.y, m_Character.transform.position.z);
        }
        if (m_Character.transform.position.y > m_VerticalBound)
        {
            m_Character.transform.position = new Vector3(m_Character.transform.position.x, m_VerticalBound, m_Character.transform.position.z);
        }
        else if (m_Character.transform.position.y < -m_VerticalBound)
        {
            m_Character.transform.position = new Vector3(m_Character.transform.position.x, -m_VerticalBound, m_Character.transform.position.z);
        }
        
        m_Camera.transform.position = new Vector3(0, 0, m_Character.transform.position.z - m_CameraTrailDistance);  
    }
}
