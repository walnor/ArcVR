using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBase : MonoBehaviour
{
    public AudioSource audioScource;
    public AudioClip audioClip;

    public ParticleSystem MuzzleFlash;

    public LineRenderer bulletTrail;


    public GameObject BulletHolePrefab;

    public Transform BulletOrigin;

    public LayerMask CanShoot;

    //public int Ammo = 8;

    public Magazin Mag;

    public Transform MagAttachPoint;


    private bool BulletInChamber = true;

    float EjectTime = 0f;

    private void Update()
    {
        if (Mag == null)
        {
            if (EjectTime >= .5f)
            {
                Mag = LookForMag();

                if (Mag != null)
                {
                    Mag.Attach(MagAttachPoint);
                    EjectTime = 0f;
                }
            }
            else
            {
                EjectTime += Time.deltaTime;
            }
        }
    }

    public void Fire()
    {
        if (BulletInChamber)
        {
            BulletInChamber = false;

            FireBullet();

            audioScource.PlayOneShot(audioClip);
            MuzzleFlash.Play();
        }
        else
        {
            //Click Sound
        }

        if (Mag != null)
        {
            BulletInChamber = Mag.GiveBullet();
        }
    }

    public void Eject()
    {
        if (Mag != null)
        {
            Mag.Eject();

            Mag = null;
        }
    }

    private void FireBullet()
    {
        //ray traced bullet fire logic

        RaycastHit t_hit;

        Vector3 hitLocation = BulletOrigin.position + (BulletOrigin.forward * 100f);

        if (Physics.Raycast(BulletOrigin.position, BulletOrigin.forward, out t_hit, 1000f, CanShoot))
        {
            HitBox hb = t_hit.collider.GetComponent<HitBox>();

            if (hb != null)
            {
                hb.OnHit();
            }
            else
            {
                GameObject NewHole = Instantiate(BulletHolePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                NewHole.transform.LookAt(t_hit.point + t_hit.normal);
                NewHole.transform.parent = t_hit.collider.gameObject.transform;
                Destroy(NewHole, 10f);
            }

            hitLocation = t_hit.point;
        }

        BulletTrailSpawn(hitLocation);
    }

    private void BulletTrailSpawn(Vector3 hitPoint)
    {
        GameObject BTE = Instantiate(bulletTrail.gameObject, BulletOrigin.position, Quaternion.identity);

        LineRenderer lineR = BTE.GetComponent<LineRenderer>();

        lineR.SetPosition(0, BulletOrigin.position);
        lineR.SetPosition(1, hitPoint);

        Destroy(BTE, 0.03f);
    }

    private Magazin LookForMag()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, .2f);

        Magazin mag = null;

        foreach (var hitCollider in hitColliders)
        {

            mag = hitCollider.GetComponent<Magazin>();

            if (mag != null)
            {
                break;
            }
        }

        return mag;
    }
}
