using UnityEditor.Animations;
using UnityEngine;

public class CharacterAnimationHandler : MonoBehaviour
{
    private Animator m_Animator;
    [SerializeField] private GameObject m_Player;


    void Awake()
    {
        m_Animator = gameObject.GetComponent<Animator>();

    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        m_Animator.SetFloat("pos_X", m_Player.transform.position.x);
        m_Animator.SetFloat("pos_Y", m_Player.transform.position.y);

        Debug.Log(m_Player.transform.position.x);
        Debug.Log("Setting floats");
    }
}
