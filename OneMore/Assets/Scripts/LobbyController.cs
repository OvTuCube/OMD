using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickedLobbyPlayButton()
    {
        Debug.Log("LobbyPlayButton");
        //씬컨트롤러에서 씬이동시킴
        //일단 임시로 여기서이동
        SceneManager.LoadScene("LandScene");
    }

    public void OnClickedLobbyExitButton()
    {
        Debug.Log("LobbyExitButton");
        Application.Quit();
    }

}
