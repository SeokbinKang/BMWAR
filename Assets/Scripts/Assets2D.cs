using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// manage the car information including images, trunk sizes, and model names.
public class Assets2D : MonoBehaviour
{
    public static Assets2D ActiveInstance;
    public Texture[] CarTextures;
    public string[] CarNames;
    public Vector3[] TrunkSizes;
    // Start is called before the first frame update
    void Start()
    {
        ActiveInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Texture getCarTexture(int i)
    {
        if (ActiveInstance == null || i < 0 || i > ActiveInstance.CarTextures.Length - 1) return null;
        return ActiveInstance.CarTextures[i];
    }
    public static string getCarNames(int i)
    {
        if (ActiveInstance == null || i < 0 || i > ActiveInstance.CarNames.Length - 1) return null;
        return ActiveInstance.CarNames[i];
    }
    public static Vector3 getTrunkSizes(int i)
    {
        if (ActiveInstance == null || i < 0 || i > ActiveInstance.TrunkSizes.Length - 1) return Vector3.zero;
        return ActiveInstance.TrunkSizes[i];
    }
}
