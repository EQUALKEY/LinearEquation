using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour {
    public void GameClose() {
        Application.OpenURL("https://www.quebon.tv/game/triFunction/exit");
    }

    public void ToTitle() {
        SceneManager.LoadScene("Title");
    }

    public void Restart() {
        SceneManager.LoadScene("Play");
    }
}
