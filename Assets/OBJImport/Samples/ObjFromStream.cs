using Dummiesman;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Scenes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ObjFromStream : MonoBehaviour
{
    [SerializeField]
    TMP_InputField objPathInputfield;

    public Material masterMaterial;
    public static ObjFromStream instance;
    public List<GameObject> loadedObj;
    public List<Texture2D> loadedTextures;
    public GameObject ParentObject;
    public bool demoPurpose;

    public int currentTextureNum;
    public int totalTextures;

    //string file_path = "E:/Invozone/Invozone Projects/3D experts streaming project/Meshes/Industrial/Fin_Fan_Lidar+photo Decimated 10%";
    //string file_path = "E:/Invozone/Invozone Projects/3D experts streaming project/Blender Projects/Fin_Fan_Lidar+photo under 1M polys";
    public string file_path = "E:/Invozone/Invozone Projects/3D experts streaming project/Meshes/Heater";
    //string file_path = "E:/Invozone/Invozone Projects/3D experts streaming project/Blender Projects/Ulco_Model_3_decimation - 10%";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        //StartCoroutine(GetRequest());
        //Invoke("LoadObjButton", 1f);
    }

    public void LoadObjButton()
    {
        file_path = objPathInputfield.textComponent.text;
        print(file_path);
        //StartCoroutine(GetRequest());
    }

    //private void OnDisable()
    //{
    //    if (System.IO.File.Exists(path))
    //    {
    //        if (AssetDatabase.DeleteAsset(path))
    //            print("deleted");
    //        else
    //            Debug.LogError(string.Format("Can not deleted '{0}'. Unknown error.", path));

    //    }
    //    else
    //    {
    //        Debug.LogError(string.Format("Can not deleted '{0}'. File does not exist.", path));
    //    }
    //}

    //void LoadObjsFromDirectory()
    //{
    //    var info = new DirectoryInfo(file_path);
    //    var fileInfoObjs = info.GetFiles("*.obj");
    //    var fileInfoMtls = info.GetFiles("*.mtl");
    //    int i = 0;
    //    foreach (FileInfo file in fileInfoObjs)
    //    {

    //        var wwwObj = new WWW(file.ToString());
    //        while (!wwwObj.isDone)
    //            System.Threading.Thread.Sleep(1);

    //        var wwwMtl = new WWW(fileInfoMtls[i].ToString());
    //        while (!wwwMtl.isDone)
    //            System.Threading.Thread.Sleep(1);

    //        //create stream and load
    //        var textStreamObj = new MemoryStream(Encoding.UTF8.GetBytes(wwwObj.text));
    //        var textStreamMtl = new MemoryStream(Encoding.UTF8.GetBytes(wwwMtl.text));
    //        loadedObj.Add(new OBJLoader().Load(textStreamObj, textStreamMtl));
    //        loadedObj[i].transform.parent = ParentObject.transform;
    //        i++;
    //    }
    //}

    public void CallCoroutine(MTLLoader mTLLoader, Texture2D texture2D, string textureName)
    {
        StartCoroutine(mTLLoader.SaveTexture(texture2D, textureName));
    }

    public void CallCoroutine2(MTLLoader mTLLoader, string texturePath, string textureName)
    {
        //StartCoroutine(mTLLoader.LoadTexturesAsync(texturePath, textureName));
    }

    IEnumerator GetRequest()
    {
        print(file_path);
        var info = new DirectoryInfo(file_path);
        var fileInfoObjs = info.GetFiles("*.obj");
        var fileInfoMtls = info.GetFiles("*.mtl");
        int i = 0;
        totalTextures = fileInfoMtls.Length;
        foreach (FileInfo file in fileInfoObjs)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(file.ToString()))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = file.ToString().Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        //create stream and load
                        var textStreamObj = new MemoryStream(Encoding.UTF8.GetBytes(webRequest.downloadHandler.text));

                        UnityWebRequest webRequestMtl = UnityWebRequest.Get(fileInfoMtls[i].ToString());
                        yield return webRequestMtl.SendWebRequest();

                        switch (webRequestMtl.result)
                        {
                            case UnityWebRequest.Result.DataProcessingError:
                                Debug.Log("Mtl not found");
                                loadedObj.Add(new OBJLoader().Load(textStreamObj));
                                loadedObj[i].transform.parent = ParentObject.transform;
                                //yield return new WaitForSeconds(2f);
                                break;
                            case UnityWebRequest.Result.Success:
                                Debug.Log("Mtl found");
                                var textStreamMtl = new MemoryStream(Encoding.UTF8.GetBytes(webRequestMtl.downloadHandler.text));
                                loadedObj.Add(new OBJLoader().Load(textStreamObj, textStreamMtl));
                                loadedObj[i].transform.parent = ParentObject.transform;
                                //yield return new WaitForSeconds(2f);
                                break;
                        }
                        break;
                }
            }
            i++;
            currentTextureNum = i;
        }
    }

}
