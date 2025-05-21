using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawnScript : MonoBehaviour
{
    GameObject item;
    PlayerMotorScript motor;

    public enum AmmoType { HANDGUN, RIFLE, MACHINEGUN, SHOTGUN };
    public AmmoType amount = AmmoType.HANDGUN;
    public int numMags = 1;

    AudioSource spawnAudio;
    public AudioClip itemSpawn;
    public AudioClip itemPickup;
    public AudioClip itemDeny;

    public GameObject handgunAmmo;
    public GameObject rifleAmmo;
    public GameObject machinegunAmmo;
    public GameObject shotgunAmmo;

    float respawnTime = 5;

    float timer;
    bool spawned;


    // Use this for initialization
    void Start()
    {
        spawnAudio = gameObject.GetComponent<AudioSource>();
        spawnAudio.volume = PlayerUI.GetVolume(PlayerPrefs.GetInt("fxvolume"));
        timer = respawnTime;
    }

    // Update is called once per frame
    void Update()
    {
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
            SpawnItem();
        }
    }

    void Rotate()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 50);
    }

    void SpawnItem()
    {
        switch (amount)
        {
            case AmmoType.HANDGUN:
                item = Instantiate(handgunAmmo, transform);
                break;
            case AmmoType.RIFLE:
                item = Instantiate(rifleAmmo, transform);
                break;
            case AmmoType.MACHINEGUN:
                item = Instantiate(machinegunAmmo, transform);
                break;
            case AmmoType.SHOTGUN:
                item = Instantiate(shotgunAmmo, transform);
                break;
        }

        spawned = true;
        spawnAudio.PlayOneShot(itemSpawn);
        timer = respawnTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (item != null)
        {
            motor = other.GetComponentInParent<PlayerMotorScript>();
            if (motor != null)
            {
                if (!motor.GiveItem(item, numMags))
                {
                    spawnAudio.PlayOneShot(itemDeny);
                    timer = 1;
                }
                else
                {
                    spawnAudio.PlayOneShot(itemPickup);
                    timer = respawnTime;
                }
                spawned = false;
            }
        }
    }
}
