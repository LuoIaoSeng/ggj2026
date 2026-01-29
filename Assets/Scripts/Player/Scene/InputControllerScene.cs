using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControllerScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (InputController.Fire1Down)
        {
            Debug.Log("Fire1");
        }
        if (InputController.Fire2Down)
        {
            Debug.Log("Fire2");
        }
    }
}
