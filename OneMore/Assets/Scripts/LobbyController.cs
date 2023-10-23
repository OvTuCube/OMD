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
        //����Ʈ�ѷ����� ���̵���Ŵ
        //�ϴ� �ӽ÷� ���⼭�̵�
        SceneManager.LoadScene("LandScene");
    }

    public void OnClickedLobbyExitButton()
    {
        Debug.Log("LobbyExitButton");
        Application.Quit();
    }

}
