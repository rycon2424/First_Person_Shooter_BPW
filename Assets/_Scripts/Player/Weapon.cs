using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("WeaponStats")]
    public string weaponName;
    public bool primary;
    public bool auto;
    public bool sniperScope;
    public float zoom;
    public int damage;
    public float alertSoundRange;
    public float inaccuracy;
    public float fireRate;
    public ParticleSystem muzzleFlash;
    [Space]
    public GameObject impact;
    [Space]
    public AudioSource aus;
    public AudioClip shot;
    [Space]
    public int ammo;
    [Range(0, 7)] public int mags;
    [Range(1, 30)] public int maxInMag;

    [HideInInspector]
    public bool cooldown;

    public void Shoot(Transform spawnLocation, bool useSpawnLocation, bool aim)
    {
        cooldown = true;
        Invoke("Cooldown", fireRate);
        muzzleFlash.Play();

        float softInaccuracy = 0;
        if (aim)
        {
            softInaccuracy = inaccuracy / 200;
        }
        else
        {
            softInaccuracy = inaccuracy / 100;
        }

        Vector3 offset = new Vector3(Random.Range(-softInaccuracy, softInaccuracy), Random.Range(-softInaccuracy, softInaccuracy), Random.Range(-softInaccuracy, softInaccuracy));
        Vector3 newDirection;

      Ray myRay;
        RaycastHit hit;
        if (useSpawnLocation)
        {
            newDirection = (offset + spawnLocation.forward).normalized;
            Debug.DrawRay(spawnLocation.position, newDirection * 200, Color.red, 0.1f);  //RayCast Debug
            myRay = new Ray(spawnLocation.position, newDirection);
        }
        else
        {
            newDirection = (offset + transform.forward).normalized;
            Debug.DrawRay(gameObject.transform.position, newDirection * 200, Color.red, 0.1f);  //RayCast Debug
            myRay = new Ray(gameObject.transform.position, newDirection);
        }

        if (Physics.Raycast(myRay, out hit, 200))
        {
            BulletImpact bm;
            bm = Instantiate(impact, hit.point, Quaternion.Euler(hit.normal)).GetComponent<BulletImpact>();
            if (hit.collider.CompareTag("Humanoid"))
            {
                bm.PlaySound(BulletImpact.HitSound.flesh);
                Actor target = hit.collider.GetComponent<Actor>();
                target.TakeDamage(damage);
            }
            else
            {
                bm.PlaySound(BulletImpact.HitSound.wall);
            }
            //Debug.Log(hit.collider.name);
        }
        aus.clip = shot;
        aus.Play();
    }

    IEnumerator DestroyParticle(GameObject ps)
    {
        ParticleSystem particle = ps.GetComponent<ParticleSystem>();
        while (particle.IsAlive())
        {
            yield return new WaitForEndOfFrame();
        }
        Destroy(ps);
    }

    void Cooldown()
    {
        cooldown = false;
    }
    
}
