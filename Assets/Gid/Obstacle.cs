using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public float health = 1;
    public GameObject effect;
    public bool bubble;


    public void TakeDamage(float Damage)
    {
        health -= Damage;
        if (health <= 0)
        {
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
