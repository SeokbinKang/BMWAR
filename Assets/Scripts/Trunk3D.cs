using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trunk3D : MonoBehaviour
{
    
    // Physical size of the trunk. (unit: meter)    
    public Vector3 dimension;
    private float scale = 1; //unity and ARCore use meter unit by default. 


    
    // 3D model for visualization items in the trunk    
    public GameObject ItemBoxPrefab;

    
    // A collection of items in the trunk    
    public List<GameObject> ItemBoxList;

    // Start is called before the first frame update
    void Start()
    {
        ItemBoxList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ClearTrunk()
    {
        foreach(var item_ in ItemBoxList)
        {
            GameObject.Destroy(item_);
        }
        ItemBoxList.Clear();
        /*
        for(int i = this.transform.childCount-1; i >=0; i--)
        {
            GameObject.Destroy(this.transform.GetChild(i));
        }*/
    }
    public void SetDimension(Vector3 size)
    {
        this.dimension = size;
        this.transform.localScale = size;
    }

    /// <summary>
    /// Add a 3D item to the trunk. The position and size of the item is provided.
    /// </summary>
    public GameObject AddItem(Vector3 position, Vector3 size)
    {

        GameObject itemInContainer = Instantiate(ItemBoxPrefab, Vector3.zero, Quaternion.identity);
        Vector3 containerCenter = dimension / 2;
        containerCenter.y = 0;
        itemInContainer.transform.localScale = new Vector3(size.x, size.y, size.z);
        itemInContainer.transform.parent = this.transform.parent;
        itemInContainer.transform.localPosition = position-containerCenter+size/2f;
        itemInContainer.transform.localRotation = this.transform.localRotation;        
        ItemBoxList.Add(itemInContainer);
        return itemInContainer;        
    }
}
