using UnityEngine;
using UnityEngine.UI;

public class PhotoManager : MonoBehaviour
{
    [SerializeField] GameObject book;
    [SerializeField] Image[] photos;
    bool completed;

    [SerializeField] CameraManager cameraManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            OnScrapbook();
        }
    }

    private void OnScrapbook()
    {
        if (book.activeSelf) { book.SetActive(false); return; }
        book.SetActive(true);

        int index = 0;
        foreach (var pictureValue in cameraManager.pictures.Values)
        {
            photos[index].sprite = pictureValue.sprite;

            index++;
        }

        if (cameraManager.pictures.Count == 8) completed = true;
    }

    public bool HasWon()
    {
        return cameraManager.pictures.Count == 8;
    }
}