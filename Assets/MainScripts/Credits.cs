using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScroller : MonoBehaviour
{
    public float scrollSpeed = 50f;
    public string mainMenuSceneName = "TitleScreen";

    void Update()
    {
        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

        if (Input.anyKeyDown || transform.localPosition.y > 2000)
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}