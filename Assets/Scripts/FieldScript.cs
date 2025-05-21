using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldScript : MonoBehaviour
{
    public Transform spawn1;
    public Transform spawn2;
    public Transform spawn3;
    public Transform spawn4;

    int[] scores = new int[2];
    int scoreLimit;
    int i;

    private Transform[] spawnPoints;

    public bool endGame = false;

    // Start is called before the first frame update
    void Start()
    {
        scoreLimit = PlayerPrefs.GetInt("scorelimit");
        spawnPoints = GetComponentsInChildren<Transform>();
        SetSpawns();
    }

    // Update is called once per frame
    void Update()
    {
        EndGame();
    }

    private void SetSpawns()
    {
        foreach (Transform spawn in spawnPoints)
        {
            switch (spawn.name)
            {
                case "Spawn1":
                    spawn1 = spawn;
                    break;
                case "Spawn2":
                    spawn2 = spawn;
                    break;
                case "Spawn3":
                    spawn3 = spawn;
                    break;
                case "Spawn4":
                    spawn4 = spawn;
                    break;
            }
        }
    }

    public int[] GetScores()
    {
        return scores;
    }

    public void InscreaseScores(int player)
    {
        scores[player]++;
    }

    public void EndGame()
    {
        for (i = 0; i < scores.Length; i++)
        {
            if (scores[i] >= scoreLimit && scoreLimit != 0)
            {
                endGame = true;
            }
        }
    }
}
