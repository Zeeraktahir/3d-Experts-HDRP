using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;

[RequireComponent(typeof(Image))]
public class LoadMatAndTextures : MonoBehaviour
{
    Image _img;

    void Start()
    {
        _img = GetComponent<UnityEngine.UI.Image>();
        //Download("E:/Invozone/Invozone Projects/3D experts streaming project/Meshes/Heater/Ulco_Model_3_u1_v2.jpeg");
        _ = StartAsync();
    }

    async Task StartAsync()
    {
        print("Async started");
        UnityWebRequest wr = new UnityWebRequest("E:/Invozone/Invozone Projects/3D experts streaming project/Meshes/Heater/Ulco_Model_3_u1_v2.jpeg");
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        wr.downloadHandler = texDl;
        await wr.SendWebRequest();
        if (wr.result == UnityWebRequest.Result.Success)
        {
            Texture2D t = texDl.texture;
            Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height),
                Vector2.zero, 1f);
            _img.sprite = s;
        }
        print("Async ended");
    }

    public void Download(string url)
    {
        StartCoroutine(LoadFromWeb(url));
    }

    IEnumerator LoadFromWeb(string url)
    {
        print("Coroutine started");
        UnityWebRequest wr = new UnityWebRequest(url);
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        wr.downloadHandler = texDl;
        yield return wr.SendWebRequest();
        if (wr.result == UnityWebRequest.Result.Success)
        {
            Texture2D t = texDl.texture;
            Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height),
                Vector2.zero, 1f);
            _img.sprite = s;
        }
        print("Coroutine ended");
    }
}