using UnityEngine;
using UnityEngine.UI;

public class PhotoManager : Singleton<PhotoManager>
{
    [SerializeField] GameObject book;
    [SerializeField] Image[] photos;
    bool completed;

    [SerializeField] CameraManager cameraManager;

    private void OnScrapbook()
    {
        if (book.activeSelf) { book.SetActive(false); return; }
        book.SetActive(true);

        int index = 0;
        foreach (var picture in cameraManager.pictures)
        {
            photos[index].sprite = picture.sprite;

            index++;
        }

        if (cameraManager.pictures.Count == photos.Length) completed = true;
    }
}