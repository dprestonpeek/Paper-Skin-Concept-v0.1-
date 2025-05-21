using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSpawnScript : MonoBehaviour
{
    public enum HealthAmount { QUARTER, HALF };
    public HealthAmount amount = HealthAmount.QUARTER;

    GameObject item;
    PlayerMotorScript motor;

    AudioSource spawnAudio;
    public AudioClip itemSpawn;
    public AudioClip itemPickup;
    public AudioClip itemDeny;

    public GameObject health25;
    public GameObject health50;

    float respawnTime = 30;

    int itemValue;
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
            case HealthAmount.QUARTER:
                item = Instantiate(health25, transform);
                itemValue = 25;
                break;
            case HealthAmount.HALF:
                item = Instantiate(health50, transform);
                itemValue = 50;
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
                if (!motor.GiveItem(item, itemValue))
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
