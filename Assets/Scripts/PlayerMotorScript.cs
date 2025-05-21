using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMotorScript : MonoBehaviour {

    //Game
    public enum Player { P1, P2, P3, P4 };
    public Player player;
    public FieldScript field;
    public GameObject hitObject;

    public enum Gamemode { SINGLE, MULTI }
    private Gamemode mode;
    private int scoreLimit;

    //Look & Move
    public Camera cam;
    private Rigidbody rb;

    private Vector3 velocity = Vector3.zero;
    private Vector3 thevel;
    private Vector3 rotation = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;

    public GameObject rightHand;
    public GameObject leftHand;
    public GameObject body;

    private float playerSpeed = 4;
    private float jumpForce = 14;

    public bool dead;

    private bool walking;
    private bool grounded;
    private bool landing;
    private bool thismighthurt;
    private bool thisisgonnahurt;
    private bool crouching;
    private bool hitTheDeck;
    private bool jumpAgain;


    //Weapon
    public WeaponScript weapon;
    public GameObject bulletSpawnPoint;
    public GameObject theWeapon;

    public GameObject glove;
    public GameObject handgun;
    public GameObject rifle;
    public GameObject machinegun;
    public GameObject shotgun;

    private bool[] weapons = { false, false, false, false, false };
    private int[] weaponAmmo = { 0, 0, 0, 0, 0 };
    private int[] weaponMags = { 0, 0, 0, 0, 0 };

    public Text crosshair;
    public int realisticReload;
    private float aimSpeed = 20;
    private float recoilSpeed = 20;
    private float recoilTime;
    private float reloadTime;
    private float switchTime = .25f;
    private float bufferTime = 1;
    private float shotTime;
    private int weaponNum = 4;

    private Vector3 handPos;
    private Vector3 rightHandPos = new Vector3(0.5f, -0.5f, 1.25f);
    private Vector3 leftHandPos = new Vector3(-0.5f, -0.5f, 1.25f);
    private Vector3 reloadPos = new Vector3(0, -1, 1.25f);
    private Vector3 glovePos = new Vector3(0.5f, 0, 2);
    private Vector3 rightHandRealPosition = new Vector3(0, 0f, 2);
    private Vector3 realPosition;
    private Vector3 realDirection;
    private int layerMask;
    private Quaternion rightHandAngle = new Quaternion(0, -3, 0, 0);

    private bool aim;
    private bool loading;
    private bool recoil;
    private bool switching;
    private bool shooting;
    private bool realReloading;
    private bool magReleased;
    private bool magReloaded;


    //Sounds
    public AudioSource footstepAudio;
    public AudioSource playerAudio;
    public AudioClip jump;
    public AudioClip hurt;
    public AudioClip land;
    public AudioClip swap;

    public AudioClip itemSpawn;
    public AudioClip itemPickup;
    public AudioClip itemDeny;


    //HUD
    public Text ammoLabel;
    public Text healthLabel;
    public Text weaponLabel;
    public Text scoreLabel;
    public Text messageLabel;
    public Text detailLabel;
    public Canvas playerHud;
    public Canvas pauseMenu;

    private int health = 100;
    private int score = 0;
    private float scrollWheel;
    public float endGameTime;
    

    void Start () {
        rb = GetComponent<Rigidbody>();
        weapon = transform.GetComponentInChildren<WeaponScript>();
        field = FindObjectOfType<FieldScript>();
        mode = Gamemode.SINGLE;
        player = Player.P1;
        handPos = rightHandPos;
        InitializeSettings();
    }

    private void InitializeSettings()
    {
        scoreLimit = PlayerPrefs.GetInt("scorelimit");
        scoreLabel.text = score + ":" + scoreLimit;
        realisticReload = PlayerPrefs.GetInt("realisticReload");

        playerAudio.volume = PlayerUI.GetVolume(PlayerPrefs.GetInt("fxvolume"));
        footstepAudio.volume = PlayerUI.GetVolume(PlayerPrefs.GetInt("fxvolume"));
    }

    private void FixedUpdate()
    {
        if (dead)
        {
            PlayerRespawn();
        }
        else
        {
            PlayerMove();
            PlayerWeapon();
            PlayerRotate();
            LoadWeapon();
        }
        Settings();
        Timer();

        thevel = rb.velocity;
    }

    public bool IsDead()
    {
        return dead;
    }

    public void PlayerPoint(BotScript theBot, PlayerMotorScript theEnemy)
    {
        if (!theBot.dead)
        {
            if (theBot.health - weapon.GetDamage() <= 0)
            {
                score++;
                scoreLabel.text = score + ":" + scoreLimit;
                field.InscreaseScores(1);
                if (score == scoreLimit && scoreLimit != 0)
                {
                    //win
                }
            }
            theBot.BotHurt(weapon.GetDamage());
        }
        else
        {

        }
    }

    //udpates the weapon label on the player's HUD
    private void HUDUpdate()
    {
        switch (weaponNum)
        {
            case 0:
                weaponLabel.text = "Handgun";
                break;
            case 1:
                weaponLabel.text = "Rifle";
                break;
            case 2:
                weaponLabel.text = "Machinegun";
                break;
            case 3:
                weaponLabel.text = "Shotgun";
                break;
            case 4:
                weaponLabel.text = "Glove";
                break;
        }
    }

    private void PlayerRespawn()
    {
        if (Input.GetButtonDown(player + "Shoot"))
        {
            int spawnpoint = Random.Range(1, 4);
            rb.transform.localRotation = Quaternion.Euler(0, rb.transform.localRotation.y, rb.transform.localRotation.z);
            messageLabel.text = "";
            detailLabel.text = "";
            GiveWeapon(glove, 4);
            dead = false;
            health = 100;
            InitializeSettings();
            ammoLabel.text = weaponAmmo[weaponNum].ToString() + ":" + weaponMags[weaponNum].ToString();
            healthLabel.text = health.ToString();

            for (int i = 0; i < weapons.Length; i++)
            {
                weaponAmmo[i] = 0;
                weaponMags[i] = 0;
                weapons[i] = false;
            }

            switch (spawnpoint)
            {
                case 1:
                    rb.transform.position = field.spawn1.position;
                    break;
                case 2:
                    rb.transform.position = field.spawn2.position;
                    break;
                case 3:
                    rb.transform.position = field.spawn3.position;
                    break;
                case 4:
                    rb.transform.position = field.spawn4.position;
                    break;

            }
        }
    }

    private void LoadWeapon()
    {
        if (loading) { 
            if (reloadTime > 0)
            {
                rightHand.transform.localPosition = Vector3.Lerp(rightHand.transform.localPosition, reloadPos, aimSpeed * Time.deltaTime);
            }
            if (loading && reloadTime <= 0)
            {
                loading = false;
            }
        }
        if (switching)
        {
            if (switchTime > 0)
            {
                rightHand.transform.localPosition = Vector3.Lerp(rightHand.transform.localPosition, reloadPos, aimSpeed * Time.deltaTime);
            }
            if (switching && switchTime <= 0)
            {
                switching = false;
            }
        }
    }

    private void ReloadWeapon()
    {
        if (weaponMags[weaponNum] > 0 && !loading)
        {
            loading = true;
            reloadTime = weapon.GetReloadTime();
            weaponAmmo[weaponNum] = weapon.defammo;
            weaponMags[weaponNum]--;
            weapon.Reload();
        }
        else
        {
            weapon.NoAmmo();
        }
    }

    void Timer()
    {
        if (shotTime > 0)
        {
            shotTime -= Time.deltaTime;
        }
        else if (shotTime <= 0 && shooting)
        {
            shotTime = 0;
            shooting = false;
            RaycastHit realHit;
            if (Physics.Raycast(realPosition, realDirection, out realHit, Mathf.Infinity, layerMask))
            {
                hitObject = realHit.collider.gameObject;
                if (hitObject.gameObject.tag == "bot")
                {
                    BotScript bot = hitObject.GetComponentInParent<BotScript>();
                    PlayerPoint(bot, null);
                }
                else if (hitObject.gameObject.tag == "player")
                {
                    PlayerMotorScript enemy = hitObject.GetComponentInParent<PlayerMotorScript>();
                    PlayerPoint(null, enemy);
                }
            }
            weapon.Shell();
        }

        if (endGameTime > 0)
        {
            endGameTime -= Time.deltaTime;
        }
        else
        {
            endGameTime = 0;
        }

        if (recoilTime > 0)
        {
            recoilTime -= Time.deltaTime;
        }
        else
        {
            recoilTime = 0;
        }

        if (reloadTime > 0)
        {
            reloadTime -= Time.deltaTime;
        }
        else
        {
            reloadTime = 0;
        }

        if (switchTime > 0)
        {
            switchTime -= Time.deltaTime;
        }
        else
        {
            switchTime = 0;
        }

        if (scrollWheel > 0)
        {
            bufferTime -= Time.deltaTime;
        }
        else if (scrollWheel < 0)
        {
            bufferTime += Time.deltaTime;
        }
        else
        {
            bufferTime = 0;
        }
    }

    public void Move(Vector3 theVelocity)
    {
        velocity = theVelocity;
    }

    public void Rotate(Vector3 theRotation)
    {
        rotation = theRotation;
    }

    public void RotateCamera(Vector3 theCameraRotation)
    {
        cameraRotation = theCameraRotation;
    }

    private void PlayerMove()
    {
        if (health <= 0)
        {
            MakeDeath();
        }
        RaycastHit fall;
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out fall, Mathf.Infinity, layerMask))
        {
            if (fall.distance > 5)
            {
                grounded = false;
                landing = true;
            }
            if (fall.distance > 15)
            {
                thismighthurt = true;
            }
            else if (thismighthurt && fall.distance < 10 && fall.distance > 6)
            {
                thisisgonnahurt = true;
            }

            if (thismighthurt && grounded)
            {
                thismighthurt = false;
            }

            if (thisisgonnahurt && grounded)
            {
                if (health > 0)
                {
                    AdjustHealth(-10);
                }
                playerAudio.PlayOneShot(hurt);
                thisisgonnahurt = false;
            }
            if (landing && grounded)
            {
                playerAudio.PlayOneShot(land);
                landing = false;
            }
        }

        Vector3 newVel;
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime * playerSpeed);
            walking = true;
        }
        else
        {
            walking = false;
        }

        if (velocity.x + velocity.z == 0 && grounded)
        {
            newVel = rb.velocity;
            newVel.x = 0;
            newVel.z = 0;
            newVel.y = rb.velocity.y;

            rb.velocity = newVel;
        }
        if (walking && grounded && !crouching)
        {
            if (!footstepAudio.isPlaying)
            {
                footstepAudio.Play();
            }
        }
        else
        {
            footstepAudio.Pause();
        }
        if (Input.GetButton(player + "Crouch") && grounded)
        {
            if (hitTheDeck)
            {
                PlayerHittheDeck();
            }
            else
            {
                crouching = true;
                PlayerCrouch();
            }
        }
        else
        {
            if (Input.GetButton(player + "Jump") && grounded && jumpAgain)
            {
                PlayerJump();
                jumpAgain = false;
            }
            else if (!Input.GetButton(player + "Jump") && grounded)
            {
                jumpAgain = true;
            }
        }
        if (!Input.GetButton(player + "Crouch") && grounded)
        {
            hitTheDeck = false;
            crouching = false;
            PlayerStand();
        }
        if (Input.GetButton(player + "Aim"))
        {
            PlayerAim();
        }
        else
        {
            PlayerSideArm();
        }
        if (Input.GetButton(player + "Inspect"))
        {
            PlayerRealUnload();
        }
    }

    private void PlayerWeapon()
    {
        if (weapon != null)
        {
            if (weapon.gameObject.name.Contains("BoxingGlove"))
            {
                if (Input.GetButtonDown(player + "Shoot"))
                {
                    rightHand.transform.localPosition = Vector3.Lerp(rightHand.transform.localPosition, glovePos, aimSpeed * Time.deltaTime);
                    if (!aim)
                    {
                        weapon.Shoot();
                    }
                    weapon.Shell();
                }
                if (!Input.GetButton(player + "Shoot"))
                {
                    PlayerSideArm();
                }

            }
            else
            {
                scrollWheel = Input.GetAxis(player + "SwitchWeapon");
                if (bufferTime == 0)
                {
                    bufferTime = 1;
                    if (scrollWheel > 0)
                    {
                        SwitchWeapon(false);
                    }
                    if (scrollWheel < 0)
                    {
                        SwitchWeapon(true);
                    }
                }
                if (weapon.gameObject.name.Contains("Machinegun"))
                {
                    weaponNum = 2;
                    if (Input.GetButton(player + "Shoot"))
                    {
                        if (recoilTime == 0 && weaponAmmo[weaponNum] > 0 && !loading)
                        {
                            PlayerShoot();
                        }
                        else if (weaponAmmo[weaponNum] <= 0)
                        {
                            weapon.NoAmmo();
                        }
                    }
                }
                else
                {
                    if (weapon.gameObject.name.Contains("Handgun"))
                    {
                        weaponNum = 0;
                    }
                    else if (weapon.gameObject.name.Contains("Rifle"))
                    {
                        weaponNum = 1;
                    }
                    else if (weapon.gameObject.name.Contains("Shotgun"))
                    {
                        weaponNum = 3;
                    }

                    if (Input.GetButtonDown(player + "Shoot"))
                    {
                        if (recoilTime == 0 && weaponAmmo[weaponNum] > 0 && !loading)
                        {
                            PlayerShoot();
                        }
                        else if (weaponAmmo[weaponNum] <= 0)
                        {
                            weapon.NoAmmo();
                        }
                    }
                }
            }

            if (recoil)
            {
                rightHand.gameObject.transform.localRotation = Quaternion.Lerp(rightHand.transform.localRotation, weapon.recoilPos, 25 * Time.deltaTime);
                if (rightHand.gameObject.transform.localRotation.x <= weapon.boundary)
                {
                    recoil = false;
                }
            }
            else
            {
                if (!loading)
                {
                    rightHand.transform.localRotation = Quaternion.Lerp(rightHand.transform.localRotation, Quaternion.Euler(0, 0, 0), 25 * Time.deltaTime);

                }
            }
        }
        if (PlayerPrefs.GetInt("realisticReload") == 0)
        {
            if (Input.GetButtonDown(player + "Reload"))
            {
                ReloadWeapon();
            }
        }
        else
        {
            if (Input.GetButton(player + "Reload"))
            {
                ReloadWeapon();
            }
        }
        ammoLabel.text = weaponAmmo[weaponNum].ToString() + ":" + weaponMags[weaponNum].ToString();
    }

    private void PlayerRotate()
    {
        rb.MoveRotation(transform.rotation * Quaternion.Euler(rotation));
        cam.transform.Rotate(cameraRotation);

        Quaternion rotLock = cam.transform.localRotation;
        if (rotLock.x > 0.7f)
        {
            rotLock.x = 0.7f;
        }
        if (rotLock.x < -0.7f)
        {
            rotLock.x = -0.7f;
        }
        cam.transform.localRotation = rotLock;

        //Mathf.Clamp(cam.transform.rotation.x, )
        //Debug.Log(cam.transform.localRotation);
        //Debug.Log(rotLock);
    }

    private void PlayerJump()
    {
        grounded = false;
        //player.velocity = (jumpForce * Vector3.up);
        rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        playerAudio.PlayOneShot(jump);
    }

    private void PlayerHittheDeck()
    {
        body.transform.localScale = Vector3.Lerp(body.transform.localScale, new Vector3(body.transform.localScale.x, .75f, body.transform.localScale.z), aimSpeed * Time.deltaTime);
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(cam.transform.localPosition.x, -4.5f, cam.transform.localPosition.z), aimSpeed * Time.deltaTime);
        playerSpeed = 1;
    }

    private void PlayerCrouch()
    {
        body.transform.localScale = Vector3.Lerp(body.transform.localScale, new Vector3(body.transform.localScale.x, 3.75f, body.transform.localScale.z), aimSpeed * Time.deltaTime);
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(cam.transform.localPosition.x, -3f, cam.transform.localPosition.z), aimSpeed * Time.deltaTime);
        playerSpeed = 2.5f;
        if (Input.GetButton(player + "Jump"))
        {
            hitTheDeck = true;
        }
    }

    private void PlayerStand()
    {
        body.transform.localScale = Vector3.Lerp(body.transform.localScale, new Vector3(body.transform.localScale.x, 8.75f, body.transform.localScale.z), aimSpeed * Time.deltaTime);
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(cam.transform.localPosition.x, -.5f, cam.transform.localPosition.z), aimSpeed * Time.deltaTime);

        playerSpeed = 4;
    }

    private void PlayerAim()
    {
        aim = true;
        rightHand.transform.localPosition = Vector3.Lerp(rightHand.transform.localPosition, weapon.sightHandPos, aimSpeed * Time.deltaTime);
    }

    private void PlayerSideArm()
    {
        aim = false;
        rightHand.transform.localPosition = Vector3.Lerp(rightHand.transform.localPosition, handPos, aimSpeed * Time.deltaTime);

    }

    private void PlayerRealUnload()
    {
        realReloading = true;

        rightHand.transform.localPosition = Vector3.Lerp(rightHand.transform.localPosition, rightHandRealPosition, aimSpeed * Time.deltaTime);
        rightHand.transform.localRotation = Quaternion.Lerp(rightHand.transform.localRotation, rightHandAngle, aimSpeed * Time.deltaTime);
        //leftHand.transform.localPosition = Vector3.Lerp(leftHand.transform.localPosition, leftHandPos, aimSpeed * Time.deltaTime);

        if (!magReleased && Input.GetButton(player + "Shoot"))
        {
            magReleased = true;
        }
        if (magReleased)
        {

        }
    }

    private void PlayerRealLoad()
    {

    }

    private void PlayerShoot()
    {
        weapon.Shoot();

        RaycastHit instantHit;
        layerMask = 1 << 8;
        layerMask = ~layerMask;
        // Does the ray intersect any objects excluding the player layer
        realPosition = bulletSpawnPoint.transform.position;
        realDirection = weapon.transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(bulletSpawnPoint.transform.position, weapon.transform.TransformDirection(Vector3.forward), out instantHit, Mathf.Infinity, layerMask))
        {
            shotTime = instantHit.distance / 300;
            shooting = true;
            //Debug.DrawRay(bulletSpawnPoint.transform.position, weapon.transform.forward, Color.red, Mathf.Infinity, true);
        }
        PlayerRecoil();
        weaponAmmo[weaponNum]--;
    }

    private void PlayerRecoil()
    {
        recoilTime = weapon.recoilTime;
        recoil = true;
    }

    public void GiveWeapon(GameObject newWeapon, int theWeaponNum)
    {
        theWeapon = Instantiate(newWeapon, rightHand.transform);
        weaponNum = theWeaponNum;
        
        //Destroy(newWeapon.gameObject);
        Destroy(weapon.gameObject);
        weapon = FindObjectOfType<WeaponScript>();
        weapon.weaponAudio = weapon.GetComponent<AudioSource>();
        bulletSpawnPoint = weapon.GetBulletSpawn();
        if (!weapons[weaponNum])
        {
            weapons[weaponNum] = true;
            weaponMags[weaponNum]++;
            ReloadWeapon();
        }
        else
        {
            weaponMags[weaponNum]++;
            switching = true;
            switchTime = 0.25f;
            playerAudio.PlayOneShot(swap);
        }
    }

    public void SwitchWeapon(bool next)
    {
        int nextWeapon = -1;
        //weaponAmmo[weaponNum] = ammo;

        if (next && weaponNum < 3)
        {
            for (int i = weaponNum + 1; i < weapons.Length - 1; i++)
            {
                if (weapons[i])
                {
                    nextWeapon = i;
                    break;
                }
            }
        }
        else if (!next && weaponNum > 0)
        {
            for (int i = weaponNum - 1; i >= 0; i--)
            {
                if (weapons[i])
                {
                    nextWeapon = i;
                    break;
                }
            }
        }

        if (nextWeapon > -1 && weaponNum != nextWeapon)
        {
            switching = true;
            switchTime = 0.25f;
            switch (nextWeapon)
            {
                case 0:
                    theWeapon = Instantiate(handgun, rightHand.transform);
                    weaponLabel.text = "Handgun";
                    break;
                case 1:
                    theWeapon = Instantiate(rifle, rightHand.transform);
                    weaponLabel.text = "Rifle";
                    break;
                case 2:
                    theWeapon = Instantiate(machinegun, rightHand.transform);
                    weaponLabel.text = "Machinegun";
                    break;
                case 3:
                    theWeapon = Instantiate(shotgun, rightHand.transform);
                    weaponLabel.text = "Shotgun";
                    break;
            }

            //ammo = weaponAmmo[nextWeapon];

            Destroy(weapon.gameObject);
            weapon = FindObjectOfType<WeaponScript>();
            weapon.weaponAudio = weapon.GetComponent<AudioSource>();
            bulletSpawnPoint = weapon.GetBulletSpawn();
            playerAudio.PlayOneShot(swap);
        }
    }

    public bool GiveItem(GameObject newItem, int value)
    {
        if (newItem.transform.name.Contains("Health"))
        {
            if (health < 90 && health + value < 110)
            {
                AdjustHealth(value);
                Destroy(newItem.gameObject);
                return true;
            }
        }
        else if (newItem.transform.name.Contains("Ammo"))
        {
            if (GiveAmmo(GetWeaponNumByName(newItem.transform.name), value))
            {
                Destroy(newItem.gameObject);
                return true;
            }
        }
        Destroy(newItem.gameObject);
        return false;
    }

    public bool GiveAmmo(int weapon, int numMags)
    {
        if (weapons[weapon])
        {
            weaponMags[weapon] += numMags;
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetWeaponNumByName(string weaponName)
    {
        if (weaponName.Contains("Handgun"))
        {
            return 0;
        }
        else if (weaponName.Contains("Rifle"))
        {
            return 1;
        }
        else if (weaponName.Contains("Machinegun"))
        {
            return 2;
        }
        else if (weaponName.Contains("Shotgun"))
        {
            return 3;
        }
        else {
            return 4;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "floor")
        {
            grounded = true;
        }
    }

    private int GetInventorySize()
    {
        int size = 0;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i])
            {
                size++;
            }
        }
        return size;
    }

    public void SwitchDexterity(bool left)
    {
        if (left)
        {
            handPos = leftHandPos;
        }
        else
        {
            handPos = rightHandPos;
        }
    }

    private void Settings()
    {
        HUDUpdate();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        crosshair.text = PlayerPrefs.GetString("crosshair");
        if (field.endGame)
        {
            EndGame();
        }
    }

    private void AdjustHealth(int amount)
    {
        health += amount;
        healthLabel.text = health.ToString();
    }

    private void MakeDeath()
    {
        rb.transform.localRotation = Quaternion.Lerp(rb.transform.localRotation, Quaternion.Euler(90, rb.transform.localRotation.y, rb.transform.localRotation.z), aimSpeed * Time.deltaTime);
        messageLabel.text = "you died";
        detailLabel.text = "press 'fire' to respawn";
        dead = true;
        footstepAudio.Stop();
    }

    private void EndGame()
    {
        int[] scores = field.GetScores();
        if (mode == Gamemode.MULTI)
        {
            if (scores[0] > scores[1])
            {
                messageLabel.text = "Player 1 Wins";
                detailLabel.text = "press 'fire' to respawn";
            }
            else
            {
                messageLabel.text = "Player 2 Wins";
                detailLabel.text = "press 'fire' to respawn";
            }
        }
        else
        {
            messageLabel.text = "Player 1 Wins";
            detailLabel.text = "press 'fire' to respawn";
        }
        if (!dead)
        {
            endGameTime = 1;
            dead = true;
        }
        footstepAudio.Stop();
        if (endGameTime == 0)
        {
            field.endGame = false;
            score = 0;
            scores[0] = 0;
            scores[1] = 0;
        }
    }
}
