using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float speed;

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Enemy")
        {
            if (collision.collider.gameObject.GetComponent<Obstacle>() != null)
            {
                collision.collider.gameObject.GetComponent<Obstacle>().TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //on collide check collider apply damage
}
