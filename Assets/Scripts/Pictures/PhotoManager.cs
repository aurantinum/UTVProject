using UnityEngine;
using UnityEngine.UI;

public class PhotoManager : MonoBehaviour
{
    [SerializeField] Image book;
    [SerializeField] Image[] photos;

    [SerializeField] CameraManager cameraManager;

    private void OnScrapbook()
    {
        int index = 0;
        foreach (var picture in cameraManager.pictures)
        {
            photos[index].sprite = picture.sprite;

            index++;
        }
    }
}