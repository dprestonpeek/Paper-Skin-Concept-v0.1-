using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotScript : MonoBehaviour
{
    Vector3 rightHandPos = new Vector3(0.5f, -0.5f, 1.25f);
    GameObject theWeapon;

    public bool dead;
    public int health = 10;
    public float timer = 1;

    int timerDefault = 15;

    public int rotationCount = 1;

    public Vector3 newPosition;
    public Vector3 currPosition;
    FieldScript field;

    AudioSource audioSource;
    [SerializeField]
    AudioClip hurt1;
    [SerializeField]
    AudioClip hurt2;
    [SerializeField]
    AudioClip hurt3;
    [SerializeField]
    AudioClip hurt4;
    [SerializeField]
    AudioClip hurt5;
    [SerializeField]
    AudioClip hurt6;
    [SerializeField]
    AudioClip hurt7;
    [SerializeField]
    AudioClip hurt8;
    [SerializeField]
    AudioClip hurt9;
    [SerializeField]
    AudioClip death;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerUI.GetVolume(PlayerPrefs.GetInt("fxvolume"));
        dead = true;
        field = FindObjectOfType<FieldScript>();
        System.Random rand = new System.Random();
        timerDefault = rand.Next(5, 20);
    }

    // Update is called once per frame
    void Update()
    {
        BotMove();
        Timer();
    }

    void Timer()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (timer <= 0)
        {
            //timer = 0;
        }
    }

    void BotMove()
    {
        if (!field.endGame)
        {
            if (dead)
            {
                if (timer == 0)
                {
                    transform.localRotation = Quaternion.Euler(0, transform.localRotation.y, transform.localRotation.z);
                    dead = false;
                    health = 100;

                    switch (Random.Range(1, 4))
                    {
                        case 1:
                            transform.position = new Vector3(0, 5, 0);
                            break;
                        case 2:
                            transform.position = new Vector3(40, 9.25f, 0);
                            break;
                        case 3:
                            transform.position = new Vector3(-50, 0.5f, -30);
                            break;
                        case 4:
                            transform.position = new Vector3(-50, 0.5f, 30);
                            break;
                    }
                }
            }
            else
            {
                currPosition = transform.position;
                switch (rotationCount)
                {
                    case 1:
                        newPosition = new Vector3(transform.position.x - 0.25f, transform.position.y, transform.position.z);
                        break;
                    case 2:
                        newPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.25f);
                        break;
                    case 3:
                        newPosition = new Vector3(transform.position.x + 0.25f, transform.position.y, transform.position.z);
                        break;
                    case 4:
                        newPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.25f);
                        break;
                }
                transform.position = newPosition;

                if (timer == 0)
                {
                    BotRotate();
                }

                if (health <= 0)
                {
                    MakeDeath();
                }
            }
        }
    }

    void BotRotate()
    {
        rotationCount = Random.Range(1, 4);
        timer = timerDefault;
        if (timer <= 9.75f)
        {
        }

    }

    void GiveWeapon(GameObject newWeapon)
    {
        //if (dead)
        //{
        //    theWeapon = Instantiate(glove, rightHand.transform);
        //    weaponNum = 4;
        //}
        //else
        //{
        //    if (newWeapon.transform.name.Contains("Handgun"))
        //    {
        //        theWeapon = Instantiate(handgun, rightHand.transform);
        //        weaponNum = 0;
        //    }
        //    else if (newWeapon.transform.name.Contains("Rifle"))
        //    {
        //        theWeapon = Instantiate(rifle, rightHand.transform);
        //        weaponNum = 1;
        //    }
        //    else if (newWeapon.transform.name.Contains("Machinegun"))
        //    {
        //        theWeapon = Instantiate(machinegun, rightHand.transform);
        //        weaponNum = 2;
        //    }
        //    else if (newWeapon.transform.name.Contains("Shotgun"))
        //    {
        //        theWeapon = Instantiate(shotgun, rightHand.transform);
        //        weaponNum = 3;
        //    }
        //}
    }

    public void BotHurt(int amount)
    {
        if (health > 90)
        {
            audioSource.PlayOneShot(hurt1);
        }
        else if (health > 80)
        {
            audioSource.PlayOneShot(hurt2);
        }
        else if (health > 70)
        {
            audioSource.PlayOneShot(hurt3);
        }
        else if (health > 60)
        {
            audioSource.PlayOneShot(hurt4);
        }
        else if (health > 50)
        {
            audioSource.PlayOneShot(hurt5);
        }
        else if (health > 40)
        {
            audioSource.PlayOneShot(hurt6);
        }
        else if (health > 30)
        {
            audioSource.PlayOneShot(hurt7);
        }
        else if (health > 20)
        {
            audioSource.PlayOneShot(hurt8);
        }
        else if (health > 10)
        {
            audioSource.PlayOneShot(hurt9);
        }
        health -= amount;
    }

    void MakeDeath()
    {
        audioSource.PlayOneShot(death);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(90, transform.localRotation.y, transform.localRotation.z), 20 * Time.deltaTime);
        dead = true;
        timer = 0;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "wall" || other.gameObject.tag == "stair")
        {
            switch (rotationCount)
            {
                case 1:
                    rotationCount = 3;
                    break;
                case 2:
                    rotationCount = 4;
                    break;
                case 3:
                    rotationCount = 1;
                    break;
                case 4:
                    rotationCount = 2;
                    break;
            }
        }
    }
}
