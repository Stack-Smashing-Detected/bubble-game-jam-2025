using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float health = 1;
    public GameObject effect;
    public bool bubble;
        
    private CharacterStatus m_CharacterStatus;
    
    public bool forceYCoordinate;
    public int forcedYCoordinate;
    public int score;

    void Start()
    {
        m_CharacterStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStatus>();
        
        if (forceYCoordinate)
        {
            transform.position = new Vector3(transform.position.x, forcedYCoordinate, transform.position.z);
        }
    }
    
    public void TakeDamage(float Damage)
    {
        health -= Damage;
        if (health <= 0)
        {
            if (bubble)
            {
                // Debug.Log("Killed a bubble");
                // m_CharacterStatus.IncreaseAir();
            }
            else
            {
                //Debug.Log("Killed a non bubble");
                m_CharacterStatus.IncreaseScore(score);
            }

            GetDestroyed();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (bubble)
        {
            m_CharacterStatus.IncreaseAir();
            GetDestroyed();
        }
        else if (collision.collider.gameObject.tag == "Player")
        {
            m_CharacterStatus.AdjustAir(-health);
            GetDestroyed();
        }
    }

    public void GetDestroyed()
    {
        Destroy(Instantiate(effect, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity), 3f);
        Destroy(gameObject);
    }
}
