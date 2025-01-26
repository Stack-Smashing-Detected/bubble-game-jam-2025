using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float health = 1;
    public GameObject effect;
    public bool bubble;
    
    private CharacterStatus m_CharacterStatus;
    
    private void Start()
    {
        m_CharacterStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStatus>();
    }
    
    public void TakeDamage(float Damage)
    {
        health -= Damage;
        if (health <= 0)
        {
            Debug.Log("Shot a thing");
            
            if (bubble)
            {
                m_CharacterStatus.IncreaseAir();
            }
            else
            {
                m_CharacterStatus.IncreaseScore();
            }

            GetDestroyed();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (bubble)
        {
            GetDestroyed();
        }
    }

    public void GetDestroyed()
    {
        Destroy(Instantiate(effect, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity), 3f);
        Destroy(gameObject);
    }
}
