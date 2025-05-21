using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour {

    public float speed = 5;
    public int mouseInversionY;
    public int mouseInversionX;
    public float lookSensitivity;

    public float movex, movez;

    public Vector3 moveHorizontal;
    public Vector3 moveVertical;

    private PlayerMotorScript motor;
    public bool pauseGame = false;

	// Use this for initialization
	void Start () {
        motor = GetComponent<PlayerMotorScript>();
        motor.pauseMenu.enabled = false;
        lookSensitivity = PlayerPrefs.GetFloat("sensitivity");
        if (lookSensitivity == 0)
        {
            lookSensitivity = 10;
        }
        mouseInversionY = PlayerPrefs.GetInt("mouseInversionY");
        if (mouseInversionY == 0)
        {
            mouseInversionY = 1;
        }
        mouseInversionX = PlayerPrefs.GetInt("mouseInversionX");
        if (mouseInversionX == 0)
        {
            mouseInversionX = 1;
        }
    }
	
	// Update is called once per frame
	void Update () {
        PlayerLook();
        PlayerPause();
        
	}

    void PlayerLook()
    {
        movex = Input.GetAxisRaw(motor.player + "Horizontal");
        movez = -1 * Input.GetAxisRaw(motor.player + "Vertical");
        float rotationy = Input.GetAxis(motor.player + "LookHorizontal");
        float rotationx = Input.GetAxis(motor.player + "LookVertical");

        moveHorizontal = transform.right * movex;
        moveVertical = transform.forward * movez;

        Vector3 theVelocity = (moveHorizontal + moveVertical).normalized * speed;
        Vector3 theRotation = new Vector3(0, rotationy, 0) * lookSensitivity;
        Vector3 theCameraRotation = new Vector3(rotationx, 0, 0) * lookSensitivity;

        motor.Move(theVelocity);
        motor.Rotate(theRotation * mouseInversionX);
        motor.RotateCamera(theCameraRotation * mouseInversionY);

    }

    void PlayerPause()
    {
        if (Input.GetButtonDown(motor.player + "Pause"))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (!pauseGame)
        {
            motor.playerHud.enabled = false;
            motor.pauseMenu.enabled = true;
            motor.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            pauseGame = true;
        }
        else
        {
            motor.enabled = true;
            motor.playerHud.enabled = true;
            motor.pauseMenu.enabled = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            pauseGame = false;
        }
    }
}
