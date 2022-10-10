using Dummiesman;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class ObjFromStreamNew : MonoBehaviour
{
    public Shader masterShader;
    public Material masterMaterial;
    
    [SerializeField]
    int listCount = 0;
    
    public static ObjFromStreamNew instance;
    public List<GameObject> loadedObj;
    public List<Texture2D> loadedTextures;
    public GameObject ParentObject;
    string file_path = "E:/Invozone/Invozone Projects/3D experts streaming project/Blender Projects/Ulco_Model_3_0.3_decimation";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        var info = new DirectoryInfo(file_path);
        var fileInfoObjs = info.GetFiles("*.obj");


        NativeList<JobHandle> jobHandlesList = new NativeList<JobHandle>(Allocator.Temp);

        ReallyToughParallelJob jobParallel = new ReallyToughParallelJob {
            objPath = file_path
        };



        jobParallel.Schedule(fileInfoObjs.Length, 10);

        JobHandle jobHandle = new JobHandle();

        jobHandlesList.Add(jobHandle);
        //jobHandle.Complete();
        JobHandle.CompleteAll(jobHandlesList);
        jobHandlesList.Dispose();



        StartCoroutine(GetRequest());
    }

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
            loadedObj[i].transform.GetChild(0).AddComponent<MaterialInstanceHandler>().AssignMatAndTexture(loadedObj[i].transform.GetChild(0).GetComponent<MeshRenderer>().material, loadedTextures[i]);
            i++;
        }
    }


    public struct ReallyToughParallelJob : IJobParallelFor
    {
        [ReadOnly] public string objPath;

        public void Execute(int index)
        {
            
        }
    }



    //[BurstCompile]
    //public struct ReallyToughJob : IJob
    //{
    //    public void Execute()
    //    {
    //        //Do something here..
    //        var info = new DirectoryInfo("E:/Invozone/Invozone Projects/3D experts streaming project/Blender Projects/Ulco_Model_3_0.3_decimation - Temp/");
    //        var fileInfoObjs = info.GetFiles("*.obj");
    //        var fileInfoMtls = info.GetFiles("*.mtl");
    //        int i = 0;
    //        foreach (FileInfo file in fileInfoObjs)
    //        {
    //            using (UnityWebRequest webRequest = UnityWebRequest.Get(file.ToString()))
    //            {
    //                // Request and wait for the desired page.
    //                //yield return webRequest.SendWebRequest();

    //                string[] pages = file.ToString().Split('/');
    //                int page = pages.Length - 1;

    //                switch (webRequest.result)
    //                {
    //                    case UnityWebRequest.Result.ConnectionError:
    //                    case UnityWebRequest.Result.DataProcessingError:
    //                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
    //                        break;
    //                    case UnityWebRequest.Result.ProtocolError:
    //                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
    //                        break;
    //                    case UnityWebRequest.Result.Success:
    //                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
    //                        //create stream and load
    //                        var textStreamObj = new MemoryStream(Encoding.UTF8.GetBytes(webRequest.downloadHandler.text));

    //                        UnityWebRequest webRequestMtl = UnityWebRequest.Get(fileInfoMtls[i].ToString());
    //                        //yield return webRequestMtl.SendWebRequest();

    //                        switch (webRequestMtl.result)
    //                        {
    //                            case UnityWebRequest.Result.DataProcessingError:
    //                                Debug.Log("Mtl not found");
    //                                loadedObj.Add(new OBJLoader().Load(textStreamObj));
    //                                loadedObj[i].transform.parent = ParentObject.transform;
    //                                break;
    //                            case UnityWebRequest.Result.Success:
    //                                Debug.Log("Mtl found");
    //                                var textStreamMtl = new MemoryStream(Encoding.UTF8.GetBytes(webRequestMtl.downloadHandler.text));
    //                                loadedObj.Add(new OBJLoader().Load(textStreamObj, textStreamMtl));
    //                                loadedObj[i].transform.parent = ParentObject.transform;
    //                                break;
    //                        }
    //                        break;
    //                }
    //            }
    //            loadedObj[i].transform.GetChild(0).gameObject.AddComponent<MaterialInstanceHandler>().AssignMatAndTexture(loadedObj[i].transform.GetChild(0).GetComponent<MeshRenderer>().material, loadedTextures[i]);
    //            i++;
    //        }
    //    }
    //}

}
