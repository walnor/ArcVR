using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleWeapon : MonoBehaviour
{
    public Transform AimPosition;
    public Transform PlayerCam;

    public GameObject BulletHolePrefab;
    public enum FireType { SemiAuto, Auto};

    public FireType Type = FireType.SemiAuto;

    public float FireRate = 3f;

    public int Ammo = 20;

    public LayerMask CanShoot;

    //float t = 0;
    public Text ammoText;

    public GameObject GunShotSoundPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Fire2"))
        {
            if (!(Vector3.Distance(AimPosition.position, transform.position) <= 0.001f))
            {
                float step = 10 * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, AimPosition.position, step);
            }
        }
        else if (!(Vector3.Distance(transform.parent.position, transform.position) <= 0.001f))
        {
            float step = 10 * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, step);
        }

        if (Input.GetButtonDown("Fire1"))
            Fire();

        if (Input.GetButtonDown("Reload"))
        {
            Reload();
        }
    }

    void Reload()
    {
        Ammo = 20;
        ammoText.text = "" + Ammo;
    }

    void Fire()
    {
        if (Ammo == 0)
            return;

        Ammo--;
        ammoText.text = "" + Ammo;
        PlayFireSound();

        RaycastHit t_hit;

        if (Physics.Raycast(PlayerCam.position, PlayerCam.forward, out t_hit, 1000f, CanShoot))
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

        }
    }

    void PlayFireSound()
    {
        GameObject shot = Instantiate(GunShotSoundPrefab, transform);
    }
}
