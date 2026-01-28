using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GameInstance.Instance.gameData.checkpoint);
    }
}
