using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReturnToSceneButton : MonoBehaviour
{
    public string targetSceneName = "MinigamesMenu";
    public Button button;

    private void Start()
    {
        if (button == null) button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ReturnToTargetScene);
        }
    }

    public void ReturnToTargetScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
