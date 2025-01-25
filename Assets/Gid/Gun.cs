using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    public GameObject gunMuzzle;
    public float secondsPerShot;
    public float damageMultiplier;
    public float timeSinceLastShot;
    public AudioSource soundPlayer;
    public AudioClip[] gunshots;
    public GameObject[] shootEffects;

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        if (Input.GetMouseButton(0))
        {
            if (timeSinceLastShot > secondsPerShot) {
                GameObject bulletSpawned = Instantiate(bullet, gunMuzzle.transform.position, Quaternion.identity);
                bulletSpawned.GetComponent<Projectile>().damage *= damageMultiplier;
                Destroy(bulletSpawned, 3f);
                //AudioClip shot = gunshots[Random.Range(0, gunshots.Length)];
                Destroy(Instantiate(shootEffects[Random.Range(0, shootEffects.Length)], gunMuzzle.transform), 3f);
                //soundPlayer.PlayOneShot(shot);
                timeSinceLastShot = 0;
            }
        }
    }
}
