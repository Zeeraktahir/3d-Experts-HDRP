using Dummiesman;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class ObjFromStream : MonoBehaviour
{
    public Shader masterShader;
    public Material masterMaterial;
    public static ObjFromStream instance;
    public List<GameObject> loadedObj;
    public List<Texture2D> loadedTextures;
    public GameObject ParentObject;
    string file_path = "E:/Invozone/Invozone Projects/3D experts streaming project/Blender Projects/Ulco_Model_3_0.3_decimation - Temp";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(GetRequest());
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

    void LoadObjsFromDirectory()
    {
        //string file_path = "E:/Invozone/Invozone Projects/3D experts streaming project/Meshes/Heater";
        var info = new DirectoryInfo(file_path);
        var fileInfoObjs = info.GetFiles("*.obj");
        var fileInfoMtls = info.GetFiles("*.mtl");
        int i = 0;
        foreach (FileInfo file in fileInfoObjs)
        {

            var wwwObj = new WWW(file.ToString());
            while (!wwwObj.isDone)
                System.Threading.Thread.Sleep(1);

            var wwwMtl = new WWW(fileInfoMtls[i].ToString());
            while (!wwwMtl.isDone)
                System.Threading.Thread.Sleep(1);

            //create stream and load
            var textStreamObj = new MemoryStream(Encoding.UTF8.GetBytes(wwwObj.text));
            var textStreamMtl = new MemoryStream(Encoding.UTF8.GetBytes(wwwMtl.text));
            loadedObj.Add(new OBJLoader().Load(textStreamObj, textStreamMtl));
            loadedObj[i].transform.parent = ParentObject.transform;
            i++;
        }
    }

    IEnumerator GetRequest()
    {
        var info = new DirectoryInfo(file_path);
        var fileInfoObjs = info.GetFiles("*.obj");
        var fileInfoMtls = info.GetFiles("*.mtl");
        int i = 0;
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
                                break;
                            case UnityWebRequest.Result.Success:
                                Debug.Log("Mtl found");
                                var textStreamMtl = new MemoryStream(Encoding.UTF8.GetBytes(webRequestMtl.downloadHandler.text));
                                loadedObj.Add(new OBJLoader().Load(textStreamObj, textStreamMtl));
                                loadedObj[i].transform.parent = ParentObject.transform;
                                break;
                        }
                        break;
                }
            }
            i++;
        }
    }

}
