using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string target;

    public void LoadScene()
    {
        SceneManager.LoadScene(target);
    }
}
