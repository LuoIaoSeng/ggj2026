using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene1 : MonoBehaviour
{

    [SerializeField] private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(() =>
        {
            GameInstance.Instance.gameData.checkpoint = 10;
            GameInstance.Instance.SaveData();
            SceneManager.LoadScene("SerializationScene2");
        });
    }
}
