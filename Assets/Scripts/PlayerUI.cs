using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System;

public class PlayerUI : MonoBehaviour
{
    GameManager gameManager;
    Scene field1;
    public PlayerControllerScript player;
    public PlayerMotorScript motor;
    public Canvas mainmenu;
    public Canvas creatematchmenu;
    public Canvas settingsmenu;
    public Canvas movementmenu;
    public Canvas displaymenu;
    public Canvas soundmenu;
    public Canvas weaponmenu;

    //environment
    public GameObject theMap;
    public GameObject field1Single;

    //bots
    public Text botLabel;
    public int bots;
    public GameObject bot;

    //score
    public Text scoreLabel;
    public int scoreLimit = 10;

    //crosshair
    char[] crosshairs = new char[] { '✣', '⊕', '⊗', '×', '✧', '✪', '➑', '◑', '✝', '♕', '♀', '♂', '❄', '♧', '♤', '♡', '♢', ' ' };
    int crosshairSelect = 0;
    public Text crosshair;

    //mouse
    int mouseInversionY = 1;
    int mouseInversionX = -1;
    public float mouseSensitivity;

    //weapon
    private int realisticReload;

    //sounds
    [SerializeField]
    Text MusicVolumeLabel;
    [SerializeField]
    Text FXVolumeLabel;
    public int MusicVolume = 3;
    public int FXVolume = 5;

    //labels
    public Text inversionY;
    public Text inversionX;
    public Text sensitivity;
    public Text dexterity;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        field1 = SceneManager.GetSceneByName("Field1");

        BackToSettings();
        Back();
        crosshair.text = PlayerPrefs.GetString("crosshair");
        mouseInversionY = player.mouseInversionY;
        mouseInversionX = player.mouseInversionX;
        mouseSensitivity = PlayerPrefs.GetFloat("sensitivity");
        if (mouseInversionY == 0)
        {
            mouseInversionY = -1;
        }
        else if (mouseInversionY == 1)
        {
            inversionY.text = "YES";
        }
        else if (mouseInversionY == -1)
        {
            inversionY.text = "NO";
        }

        if (mouseInversionX == 0)
        {
            mouseInversionX = 1;
        }
        else if (mouseInversionX == 1)
        {
            inversionX.text = "NO";
        }
        else if (mouseInversionX == -1)
        {
            inversionX.text = "YES";
        }

        if (mouseSensitivity == 0)
        {
            mouseSensitivity = 2;
        }
        sensitivity.text = (mouseSensitivity).ToString();
        LoadSettings();
    }

    // Update is called once per frame
    void Update()
    {
        if (field1.isLoaded)
        {
            SceneManager.SetActiveScene(field1);
        }
    }

    public void LoadSettings()
    {
        bots = PlayerPrefs.GetInt("bots");
        botLabel.text = bots.ToString();
        scoreLimit = PlayerPrefs.GetInt("scorelimit");
        scoreLabel.text = scoreLimit.ToString();
        FXVolume = PlayerPrefs.GetInt("fxvolume");
        FXVolumeLabel.text = FXVolume.ToString();
        MusicVolume = PlayerPrefs.GetInt("musicvolume");
        MusicVolumeLabel.text = MusicVolume.ToString();
        SetVolumes();

        for (int i = 0; i < bots; i++)
        {
            Instantiate(bot);
        }
    }

    public void RealisticReload()
    {
        if (realisticReload == 0)
        {
            realisticReload = 1;
        }
        else
        {
            realisticReload = 0;
        }
        PlayerPrefs.SetInt("realisticReload", realisticReload);
    }

    public void NextCrosshair()
    {
        if (crosshairSelect == 17)
        {
            crosshairSelect = 0;
        }
        else
        {
            crosshairSelect++;
        }
        crosshair.text = crosshairs[crosshairSelect].ToString();
        PlayerPrefs.SetString("crosshair", crosshairs[crosshairSelect].ToString());
        PlayerPrefs.Save();
    }

    public void PrevCrosshair()
    {
        if (crosshairSelect == 0)
        {
            crosshairSelect = 17;
        }
        else
        {
            crosshairSelect--;
        }
        crosshair.text = crosshairs[crosshairSelect].ToString();
        PlayerPrefs.SetString("crosshair", crosshairs[crosshairSelect].ToString());
        PlayerPrefs.Save();
    }

    public void CreateMatchMenu()
    {
        mainmenu.enabled = false;
        creatematchmenu.enabled = true;
    }

    public void MoreBots()
    {
        if (bots < 3)
        {
            bots++;
        }  
        botLabel.text = bots.ToString();
        PlayerPrefs.SetInt("bots", bots);
    }

    public void LessBots()
    {
        if (bots > 0)
        {
            bots--;
        }
        botLabel.text = bots.ToString();
        PlayerPrefs.SetInt("bots", bots);
    }

    public void MoreKills()
    {
        if (scoreLimit < 50)
        {
            scoreLimit += 5;
        }
        scoreLabel.text = scoreLimit.ToString();
        PlayerPrefs.SetInt("scorelimit", scoreLimit);
    }

    public void LessKills()
    {
        if (scoreLimit > 0)
        {
            scoreLimit -= 5;
        }
        scoreLabel.text = scoreLimit.ToString();
        PlayerPrefs.SetInt("scorelimit", scoreLimit);
    }

    public void InvertMouseY()
    {
        if (mouseInversionY == -1)
        {
            mouseInversionY = 1;
            inversionY.text = "YES";
        }
        else if (mouseInversionY == 1)
        {
            mouseInversionY = -1;
            inversionY.text = "NO";
        }
        player.mouseInversionY = mouseInversionY;
        PlayerPrefs.SetInt("mouseInversionY", mouseInversionY);
        PlayerPrefs.Save();
    }

    public void InvertMouseX()
    {
        if (mouseInversionX == 1)
        {
            mouseInversionX = -1;
            inversionX.text = "YES";
        }
        else if (mouseInversionX == -1)
        {
            mouseInversionX = 1;
            inversionX.text = "NO";
        }
        player.mouseInversionX = mouseInversionX;
        PlayerPrefs.SetInt("mouseInversionX", mouseInversionX);
        PlayerPrefs.Save();
    }

    public void MoreSensitive()
    {
        if (mouseSensitivity < 5)
        {
            mouseSensitivity += .5f;
            sensitivity.text = (mouseSensitivity * 2).ToString();
            player.lookSensitivity = mouseSensitivity;
            PlayerPrefs.SetFloat("sensitivity", mouseSensitivity);
        }
    }

    public void LessSensitive()
    {
        if (mouseSensitivity > .5f)
        {
            mouseSensitivity -= .5f;
            sensitivity.text = (mouseSensitivity * 2).ToString();
            player.lookSensitivity = mouseSensitivity;
            PlayerPrefs.SetFloat("sensitivity", mouseSensitivity);
        }
    }

    public static float GetVolume(int input)
    {
        switch (input)
        {
            case 0:
                return 0;
            case 1:
                return .1f;
            case 2:
                return .2f;
            case 3:
                return .3f;
            case 4:
                return .4f;
            case 5:
                return .5f;
            case 6:
                return .6f;
            case 7:
                return .7f;
            case 8:
                return .8f;
            case 9:
                return .9f;
            case 10:
                return 1;
            default:
                return .5f;
        }
    }

    private void SetVolumes()
    {
        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
        {
            if (!source.transform.CompareTag("GameManager") && source.volume < 1)
            {
                source.volume = GetVolume(FXVolume);
            }
        }
        gameManager.audioSource.volume = GetVolume(MusicVolume);
    }

    public void FXVolumeUp()
    {
        if (FXVolume < 10)
        {
            FXVolume++;
            foreach (AudioSource source in FindObjectsOfType<AudioSource>())
            {
                if (!source.transform.CompareTag("GameManager"))
                {
                    source.volume = GetVolume(FXVolume);
                }
            }

            FXVolumeLabel.text = FXVolume.ToString();
            PlayerPrefs.SetInt("fxvolume", FXVolume);
        }
    }

    public void FXVolumeDown()
    {
        if (FXVolume > 0)
        {
            FXVolume--;
            foreach (AudioSource source in FindObjectsOfType<AudioSource>())
            {
                if (!source.transform.CompareTag("GameManager"))
                {
                    source.volume = GetVolume(FXVolume);
                }
            }

            FXVolumeLabel.text = FXVolume.ToString();
            PlayerPrefs.SetInt("fxvolume", FXVolume);
        }
    }

    public void MusicVolumeUp()
    {
        if (MusicVolume < 10)
        {
            MusicVolume++;
            gameManager.audioSource.volume = GetVolume(MusicVolume);
            MusicVolumeLabel.text = MusicVolume.ToString();
            PlayerPrefs.SetInt("musicvolume", MusicVolume);
        }
    }

    public void MusicVolumeDown()
    {
        if (MusicVolume > 0)
        {
            MusicVolume--;
            gameManager.audioSource.volume = GetVolume(MusicVolume);
            MusicVolumeLabel.text = MusicVolume.ToString();
            PlayerPrefs.SetInt("musicvolume", MusicVolume);
        }
    }

    public void DexteritySwitch()
    {
        if (dexterity.text == "RIGHT")
        {
            dexterity.text = "LEFT";
            motor.SwitchDexterity(true);
        }
        else
        {
            dexterity.text = "RIGHT";
            motor.SwitchDexterity(false);
        }
    }

    public void SettingsMenu()
    {
        mainmenu.enabled = false;
        settingsmenu.enabled = true;
    }

    public void MovementMenu()
    {
        settingsmenu.enabled = false;
        movementmenu.enabled = true;
    }

    public void DisplayMenu()
    {
        settingsmenu.enabled = false;
        displaymenu.enabled = true;
    }

    public void SoundMenu()
    {
        settingsmenu.enabled = false;
        soundmenu.enabled = true;
    }

    public void WeaponMenu()
    {
        settingsmenu.enabled = false;
        weaponmenu.enabled = true;
    }

    public void BackToSettings()
    {
        //displaymenu.enabled = false;
        weaponmenu.enabled = false;
        soundmenu.enabled = false;
        movementmenu.enabled = false;
        settingsmenu.enabled = true;
    }

    public void Back()
    {
        creatematchmenu.enabled = false;
        settingsmenu.enabled = false;
        mainmenu.enabled = true;
    }

    public void Play()
    {
        motor.dead = true;
        SceneManager.LoadScene("Field1", LoadSceneMode.Single);
        SceneManager.LoadScene("DontDestroyOnLoad", LoadSceneMode.Additive);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
