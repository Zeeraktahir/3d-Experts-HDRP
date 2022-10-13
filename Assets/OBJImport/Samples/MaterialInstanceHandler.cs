using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MaterialInstanceHandler : MonoBehaviour
{
    public Material instanceMaterial;
    
    [SerializeField]
    Color color;
    
    [SerializeField]
    Texture materialTexture;

    void ConvertGOToEntity()
    {
        gameObject.AddComponent<ConvertToEntity>().ConversionMode = ConvertToEntity.Mode.ConvertAndDestroy;
    }

    public void AssignMatAndTexture(Material material, Texture texture)
    {
        instanceMaterial = material;
        instanceMaterial.mainTexture = texture;
        //ConvertGOToEntity();
    }
}
