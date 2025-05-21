using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour {
    
    public GameObject weapon;
    PlayerMotorScript motor;
    public bool shooting;

    public AudioSource weaponAudio;
    public AudioClip cock;
    public AudioClip fire;
    public AudioClip shell;
    public AudioClip noammo;
    public AudioClip reload;

    public float recoilTime;
    public float reloadTime;
    public float boundary;
    public int ammo;
    public int defammo;
    public int damage;
    public bool inHand;
    private int realisticReload;

    public Vector3 sightHandPos;
    public Quaternion recoilPos;
    public GameObject bulletSpawnPoint;

    // Use this for initialization
    void Start()
    {
        weapon = this.gameObject;
        weaponAudio = GetComponent<AudioSource>();
        weaponAudio.volume = PlayerUI.GetVolume(PlayerPrefs.GetInt("fxvolume"));
        motor = GetComponentInParent<PlayerMotorScript>();
    }

    // Update is called once per frame
    void Update()
    {
        WeaponType();
    }

    void WeaponType()
    {
        realisticReload = PlayerPrefs.GetInt("realisticReaload");
        if (transform.name.Contains("BoxingGlove"))
        {
            recoilTime = 0;
            reloadTime = 1;
            defammo = 0;
            ammo = 0;
            damage = 5;

            sightHandPos = new Vector3(0, -0.1f, 0.7f);
        }
        else if (transform.name.Contains("Rifle"))
        {
            recoilTime = 1;
            reloadTime = 2;
            defammo = 10;
            ammo = 0;
            damage = 30;

            sightHandPos = new Vector3(0, -0.1f, 0.8f);
            recoilPos = Quaternion.Euler(-5, 0, 0);
            boundary = -.04f;
        }
        else if (transform.name.Contains("Handgun"))
        {
            recoilTime = 0.2f;
            reloadTime = 1.5f;
            defammo = 12;
            damage = 20;

            sightHandPos = new Vector3(0, -0.1f, 0.7f);
            recoilPos = Quaternion.Euler(-15, 0, 0);
            boundary = -.1f;
        }
        else if (transform.name.Contains("Machinegun"))
        {
            recoilTime = 0.15f;
            reloadTime = 3;
            defammo = 20;
            damage = 15;

            sightHandPos = new Vector3(0, -0.05f, .025f);
            recoilPos = Quaternion.Euler(-5f, 0, 0);
            boundary = -.04f;
        }
        else if (transform.name.Contains("Shotgun"))
        {
            recoilTime = 0.2f;
            reloadTime = 2;
            defammo = 8;
            damage = 30;

            sightHandPos = new Vector3(0, -0.05f, 0.025f);
            recoilPos = Quaternion.Euler(-10, 0, 0);
            boundary = -.04f;
        }
    }

    public GameObject GetBulletSpawn()
    {
        return bulletSpawnPoint;
    }

    public int GetDamage()
    {
        return damage;
    }

    public int GetAmmoCap()
    {
        WeaponType();
        return defammo;
    }

    public float GetReloadTime()
    {
        WeaponType();
        return reloadTime;
    }

    public void Cock()
    {
        weaponAudio.PlayOneShot(cock);
    }

    public void Shoot()
    {
        shooting = true;
        weaponAudio.PlayOneShot(fire);
    }

    public void Shell()
    {
        weaponAudio.PlayOneShot(shell);
    }

    public void NoAmmo()
    {
        weaponAudio.PlayOneShot(noammo);
    }

    public void Reload()
    {
        weaponAudio.PlayOneShot(reload);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (transform.name.Contains("BoxingGlove"))
        {
            if (shooting)
            {
                if (other.gameObject.tag == "bot")
                {
                    BotScript bot = other.gameObject.GetComponentInParent<BotScript>();
                    motor.PlayerPoint(bot, null);
                    shooting = false;
                }
            }
        }
    }
}
