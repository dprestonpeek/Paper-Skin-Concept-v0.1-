using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawnScript : MonoBehaviour {

    GameObject weapon;
    PlayerMotorScript motor;
    BotScript bot;

    public enum WeaponType { HANDGUN, RIFLE, MACHINEGUN, SHOTGUN, GLOVE };
    public WeaponType type = WeaponType.HANDGUN;
    int weaponNum = 0;

    AudioSource spawnAudio;
    public AudioClip spawn;
    
    public GameObject handgun;
    public GameObject rifle;
    public GameObject machinegun;
    public GameObject shotgun;
    public GameObject glove;

    float timer = 5;
    bool spawned;

    // Use this for initialization
    void Start () {
        spawnAudio = GetComponent<AudioSource>();
        spawnAudio.volume = PlayerUI.GetVolume(PlayerPrefs.GetInt("fxvolume"));
    }
	
	// Update is called once per frame
	void Update () {
        TimeToRespawn();
        Rotate();
	}

    void TimeToRespawn()
    {
        if (!spawned)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            SpawnWeapon();
        }
    }

    void Rotate()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 50);
    }

    void SpawnWeapon()
    {
        switch (type) {
            case WeaponType.HANDGUN:
                weapon = Instantiate(handgun, transform);
                weaponNum = 0;
                break;
            case WeaponType.RIFLE:
                weapon = Instantiate(rifle, transform);
                weaponNum = 1;
                break;
            case WeaponType.MACHINEGUN:
                weapon = Instantiate(machinegun, transform);
                weaponNum = 2;
                break;
            case WeaponType.SHOTGUN:
                weapon = Instantiate(shotgun, transform);
                weaponNum = 3;
                break;
            case WeaponType.GLOVE:
                weapon = Instantiate(glove, transform);
                weaponNum = 4;
                break;
        }
        spawned = true;
        spawnAudio.PlayOneShot(spawn);
        timer = 5;
    }

    private void OnTriggerEnter(Collider other)
    {
        motor = other.GetComponentInParent<PlayerMotorScript>();
        if (motor != null)
        {
            if (weapon != null && spawned)
            {
                motor.GiveWeapon(weapon, weaponNum);
                Destroy(weapon);
                spawned = false;
            }
        }
        else
        {
            bot = other.GetComponentInParent<BotScript>();
            if (bot != null)
            {
                if (weapon != null && spawned)
                {

                    spawned = false;
                }
            }
        }
    }
}
