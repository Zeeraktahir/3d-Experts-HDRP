using Dummiesman;
//using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using System.Threading.Tasks;
//using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.Networking;
//using UnityEngine.Rendering;
//using static TreeEditor.TextureAtlas;
//using Random = UnityEngine.Random;

public class MTLLoader : MonoBehaviour {
    public List<string> SearchPaths = new List<string>() { "%FileName%_Textures", string.Empty};

    private FileInfo _objFileInfo = null;

    /// <summary>
    /// The texture loading function. Overridable for stream loading purposes.
    /// </summary>
    /// <param name="path">The path supplied by the OBJ file, converted to OS path seperation</param>
    /// <param name="isNormalMap">Whether the loader is requesting we convert this into a normal map</param>
    /// <returns>Texture2D if found, or NULL if missing</returns>
    public virtual Texture2D TextureLoadFunction(string path, bool isNormalMap)
    {
        //find it
        foreach (var searchPath in SearchPaths)
        {
            //replace varaibles and combine path
            string processedPath = (_objFileInfo != null) ? searchPath.Replace("%FileName%", Path.GetFileNameWithoutExtension(_objFileInfo.Name)) 
                                                          : searchPath;
            string filePath = Path.Combine(processedPath, path);

            //return if eists
            if (File.Exists(filePath))
            {
                var tex = ImageLoader.LoadTexture(filePath);


                if(isNormalMap)
                    tex = ImageUtils.ConvertToNormalMap(tex);

                return tex;
            }
        }

        //not found
        return null;
    }

    private Texture2D TryLoadTexture(string texturePath, bool normalMap = false)
    {
        //swap directory seperator char
        texturePath = texturePath.Replace('\\', Path.DirectorySeparatorChar);
        texturePath = texturePath.Replace('/', Path.DirectorySeparatorChar);

        return TextureLoadFunction(texturePath, normalMap);
    }
    
    private int GetArgValueCount(string arg)
    {
        switch (arg)
        {
            case "-bm":
            case "-clamp":
            case "-blendu":
            case "-blendv":
            case "-imfchan":
            case "-texres":
                return 1;
            case "-mm":
                return 2;
            case "-o":
            case "-s":
            case "-t":
                return 3;
        }
        return -1;
    }

    private int GetTexNameIndex(string[] components)
    {
        for(int i=1; i < components.Length; i++)
        {
            var cmpSkip = GetArgValueCount(components[i]);
            if(cmpSkip < 0)
            {
                return i;
            }
            i += cmpSkip;
        }
        return -1;
    }

    private float GetArgValue(string[] components, string arg, float fallback = 1f)
    {
        string argLower = arg.ToLower();
        for(int i=1; i < components.Length - 1; i++)
        {
            var cmp = components[i].ToLower();
            if(argLower == cmp)
            {
                return OBJLoaderHelper.FastFloatParse(components[i+1]);
            }
        }
        return fallback;
    }

    private string GetTexPathFromMapStatement(string processedLine, string[] splitLine)
    {
        int texNameCmpIdx = GetTexNameIndex(splitLine);
        if(texNameCmpIdx < 0)
        {
            Debug.LogError($"texNameCmpIdx < 0 on line {processedLine}. Texture not loaded.");
            return null;
        }

        int texNameIdx = processedLine.IndexOf(splitLine[texNameCmpIdx]);
        string texturePath = processedLine.Substring(texNameIdx);

        return texturePath;
    }

    //Texture2D KdTexture;
    //public IEnumerator LoadTextures(string url)
    //{
    //    UnityWebRequest wr = new UnityWebRequest(url);
    //    DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
    //    wr.downloadHandler = texDl;
    //    yield return wr.SendWebRequest();

    //    if (wr.result == UnityWebRequest.Result.Success)
    //    {
    //        Texture2D t = texDl.texture;
    //        KdTexture = t;
    //    }
    //}

    public async void LoadTexturesAsync(string url, string textureName)
    {

        //swap directory seperator char
        url = url.Replace('\\', Path.DirectorySeparatorChar);
        url = url.Replace('/', Path.DirectorySeparatorChar);

        print("Async started: " + url);

        if (File.Exists(url))
        {
            var www = UnityWebRequestTexture.GetTexture(url);
            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var texture2D = DownloadHandlerTexture.GetContent(www);

                if (ObjFromStream.instance.demoPurpose)
                {
                    ObjFromStream.instance.loadedTextures.Add(texture2D);

                    if (ObjFromStream.instance.loadedTextures.Count - 1 < 0)
                        ObjFromStream.instance.loadedObj[0].transform.GetChild(0).gameObject.AddComponent<MaterialInstanceHandler>().AssignMatAndTexture(ObjFromStream.instance.loadedObj[0].transform.GetChild(0).GetComponent<MeshRenderer>().material, ObjFromStream.instance.loadedTextures[0]);
                    else
                        ObjFromStream.instance.loadedObj[ObjFromStream.instance.loadedTextures.Count - 1].transform.GetChild(0).gameObject.AddComponent<MaterialInstanceHandler>().AssignMatAndTexture(ObjFromStream.instance.loadedObj[ObjFromStream.instance.loadedTextures.Count - 1].transform.GetChild(0).GetComponent<MeshRenderer>().material, ObjFromStream.instance.loadedTextures[ObjFromStream.instance.loadedTextures.Count - 1]);
                }
                else
                {
                    ObjFromStream.instance.CallCoroutine(this, texture2D, textureName);
                    //StartCoroutine(SaveTexture(texture2D, textureName));
                }
            }
        }
        print("Async ended");
    }

    //public IEnumerator LoadTexturesAsync(string url, string textureName)
    //{

    //    //swap directory seperator char
    //    url = url.Replace('\\', Path.DirectorySeparatorChar);
    //    url = url.Replace('/', Path.DirectorySeparatorChar);

    //    print("Async started: " + url);

    //    if (File.Exists(url))
    //    {
    //        var www = UnityWebRequestTexture.GetTexture(url);
    //        yield return www.SendWebRequest();

    //        if (www.result == UnityWebRequest.Result.Success)
    //        {
    //            var texture2D = DownloadHandlerTexture.GetContent(www);

    //            if (ObjFromStream.instance.demoPurpose)
    //            {
    //                ObjFromStream.instance.loadedTextures.Add(texture2D);

    //                if (ObjFromStream.instance.loadedTextures.Count - 1 < 0)
    //                    ObjFromStream.instance.loadedObj[0].transform.GetChild(0).gameObject.AddComponent<MaterialInstanceHandler>().AssignMatAndTexture(ObjFromStream.instance.loadedObj[0].transform.GetChild(0).GetComponent<MeshRenderer>().material, ObjFromStream.instance.loadedTextures[0]);
    //                else
    //                    ObjFromStream.instance.loadedObj[ObjFromStream.instance.loadedTextures.Count - 1].transform.GetChild(0).gameObject.AddComponent<MaterialInstanceHandler>().AssignMatAndTexture(ObjFromStream.instance.loadedObj[ObjFromStream.instance.loadedTextures.Count - 1].transform.GetChild(0).GetComponent<MeshRenderer>().material, ObjFromStream.instance.loadedTextures[ObjFromStream.instance.loadedTextures.Count - 1]);
    //            }
    //            else
    //            {
    //                ObjFromStream.instance.CallCoroutine(this, texture2D, textureName);
    //                //StartCoroutine(SaveTexture(texture2D, textureName));
    //            }
    //        }
    //    }
    //    else
    //    {
    //        print("Texture file does not exists");
    //    }
    //    print("Async ended");
    //}


    public IEnumerator SaveTexture(Texture2D texture, string textureName)
    {
        byte[] bytes = texture.EncodeToJPG();
        var dirPath = Application.dataPath + "/Resources/Textures";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        string texturePath = dirPath + "/" + textureName + ".jpg";

        File.WriteAllBytes(texturePath, bytes);
        Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPath);

        yield return new WaitForSeconds(1f);
        var loadedTexture2D = (Texture2D)Resources.Load("Textures/" + textureName, typeof(Texture2D));
        yield return new WaitForSeconds(1f);
        ObjFromStream.instance.loadedTextures.Add(loadedTexture2D);

        if (ObjFromStream.instance.loadedTextures.Count - 1 < 0)
            ObjFromStream.instance.loadedObj[0].transform.GetChild(0).gameObject.AddComponent<MaterialInstanceHandler>().AssignMatAndTexture(ObjFromStream.instance.loadedObj[0].transform.GetChild(0).GetComponent<MeshRenderer>().material, ObjFromStream.instance.loadedTextures[0]);
        else
            ObjFromStream.instance.loadedObj[ObjFromStream.instance.loadedTextures.Count - 1].transform.GetChild(0).gameObject.AddComponent<MaterialInstanceHandler>().AssignMatAndTexture(ObjFromStream.instance.loadedObj[ObjFromStream.instance.loadedTextures.Count - 1].transform.GetChild(0).GetComponent<MeshRenderer>().material, ObjFromStream.instance.loadedTextures[ObjFromStream.instance.loadedTextures.Count - 1]);


#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    string textureName;
    public Dictionary<string, Material> Load(Stream input)
    {
        var inputReader = new StreamReader(input);
        var reader = new StringReader(inputReader.ReadToEnd());

        Dictionary<string, Material> mtlDict = new Dictionary<string, Material>();
        Material currentMaterial = null;

        for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string processedLine = line.Clean();
            string[] splitLine = processedLine.Split(' ');

            //blank or comment
            if (splitLine.Length < 2 || processedLine[0] == '#')
                continue;

            //newmtl
            if (splitLine[0] == "newmtl")
            {
                textureName = processedLine.Substring(7);

                var newMtl = new Material(ObjFromStream.instance.masterMaterial) { name = textureName };
                mtlDict[textureName] = newMtl;
                currentMaterial = newMtl;

                continue;
            }

            //anything past here requires a material instance
            if (currentMaterial == null)
                continue;

            //diffuse color
            if (splitLine[0] == "Kd" || splitLine[0] == "kd")
            {
                var currentColor = currentMaterial.GetColor("_Color");
                var kdColor = OBJLoaderHelper.ColorFromStrArray(splitLine);

                currentMaterial.SetColor("_Color", new Color(kdColor.r, kdColor.g, kdColor.b, currentColor.a));
                continue;
            }

            //diffuse map
            if (splitLine[0] == "map_Kd" || splitLine[0] == "map_kd")
            {
                string texturePath = GetTexPathFromMapStatement(processedLine, splitLine);
                if(texturePath == null)
                {
                    continue; //invalid args or sth
                }
                Debug.Log("texturePath: " + texturePath);
                Debug.Log("textureName: " + textureName);
                //ObjFromStream.instance.CallCoroutine2(this, ObjFromStream.instance.file_path + "/" + texturePath, textureName);
                LoadTexturesAsync(ObjFromStream.instance.file_path + "/" + texturePath, textureName);
                continue;
            }
        }
        //return our dict
        return mtlDict;
    }

    /// <summary>
    /// Loads a *.mtl file
    /// </summary>
    /// <param name="path">The path to the MTL file</param>
    /// <returns>Dictionary containing loaded materials</returns>
    public Dictionary<string, Material> Load(string path)
    {
        _objFileInfo = new FileInfo(path); //get file info
        SearchPaths.Add(_objFileInfo.Directory.FullName); //add root path to search dir

        using (var fs = new FileStream(path, FileMode.Open))
        {
            return Load(fs); //actually load
        }
        
    }
}
