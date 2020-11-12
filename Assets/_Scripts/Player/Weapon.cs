using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("WeaponStats")]
    public string weaponName;
    public int damage;
    public float rangePenalty;
    public float inaccuracy;
    public float fireRate;
    public ParticleSystem muzzleFlash;
    [Space]
    public GameObject stoneImpact;
    [Space]

    public int ammo;
    [Range(0, 7)] public int mags;
    [Range(1, 30)] public int maxInMag;

    [HideInInspector]
    public bool cooldown;

    public void Shoot()
    {
        cooldown = true;
        Invoke("Cooldown", fireRate);
        muzzleFlash.Play();

        float softInaccuracy = inaccuracy / 100;

        Vector3 offset = new Vector3(Random.Range(-softInaccuracy, softInaccuracy), Random.Range(-softInaccuracy, softInaccuracy), 0);

        Vector3 newDirection = (offset + transform.forward).normalized;

        Debug.DrawRay(gameObject.transform.position, newDirection * 200, Color.red, 0.1f);  //RayCast Debug
        Ray myRay = new Ray(gameObject.transform.position, newDirection);
        RaycastHit hit;

        if (Physics.Raycast(myRay, out hit, 200))
        {
            StartCoroutine(DestroyParticle(Instantiate(stoneImpact, hit.point, Quaternion.Euler(hit.normal))));
            Debug.Log(hit.collider.name);
        }

        ammo--;
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
