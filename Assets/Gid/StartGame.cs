using UnityEngine;

public class StartGame : MonoBehaviour
{
    public void GoToGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }
}
