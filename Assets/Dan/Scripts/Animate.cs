using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Animate : MonoBehaviour
{
    private const string kXPosString = "x_pos";
    private const string kYPosString = "y_pos";

    [SerializeField] private GameObject m_Character;
    private Animator m_Animator;
    
    void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        Debug.Log("Updating Animate script");
        m_Animator.SetFloat(kXPosString, m_Character.transform.position.x);
        m_Animator.SetFloat(kYPosString, m_Character.transform.position.y);
    }
}
