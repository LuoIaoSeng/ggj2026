using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            SavePoint();
        }
    }

    void SavePoint()
    {
        var temp = GameInstance.Instance.gameData;
        temp.checkpoint = Mathf.Max(temp.checkpoint, checkpointIndex);
        GameInstance.Instance.gameData = temp;
        GameInstance.Instance.SaveData();
    }
}
