using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ContainerPacking;
using ContainerPacking.Entities;
using System;
using ContainerPacking.Algorithms;

/// <summary>
/// Manage a collection of items that user found
/// </summary>
public class ItemList : MonoBehaviour
{
    // Main car instance
    public GameObject m_MyCar;

    //UI Icon for individual items on the left menu
    public GameObject ItemIconPrefab;

    //UI Icon for adding a new item
    public GameObject AddIcon;

    //UI Icon for auto-fit
    public GameObject AutoFitButton;

    //UI Icon for auto-fit result
    public GameObject AutoFitResult;

    //Color for indicating items placed in the trunk.
    public Color PackedItemColor;

    //Color for indicating items not placed in the turnk.
    public Color UnPackedItemColor;
    // Start is called before the first frame update
    void Start()
    {
        AutoFitButton.SetActive(false);
        AutoFitResult.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Enable the auto-fit button only when there's is a change in items or trunk size.
    /// </summary>
    public void EnableAutoFit(bool t)
    {
        AutoFitButton.SetActive(t);
        AutoFitResult.SetActive(false);
    }
    /// <summary>
    /// Add a new item. The esimated size is provided
    /// </summary>
    public void AddItem(GameObject ItemBox, Vector3 dimension)
    {
        GameObject itemGameObject = Instantiate(ItemIconPrefab, Vector3.zero, Quaternion.identity);
        itemGameObject.GetComponent<ARItem>().initialize(ItemBox, dimension);
        itemGameObject.transform.parent = this.transform;
        AddIcon.transform.SetAsLastSibling();
        EnableAutoFit(true);
    }
    /// <summary>
    /// Try auto packing.
    /// </summary>
    public void AutoPackItems()
    {
        bool ret = PackingItems();
        if(ret)
        {
            AutoFitResult.SetActive(true);
            AutoFitResult.GetComponentInChildren<Text>().text = "The items fit in the trunk.";
        } else
        {
            AutoFitResult.SetActive(true);
            AutoFitResult.GetComponentInChildren<Text>().text = "The items do not fit in the trunk.";
        }
    }

    /// <summary>
    /// Auto packing implementation. Reference: https://github.com/davidmchapman/3DContainerPacking
    /// </summary>
    public bool PackingItems()
    {
        //retrieve the container size        
        Vector3 containerSize = m_MyCar.GetComponent<MyCar>().GetTrunkSize();

        //map the item id to the GameObject on the left-UI pane
        Dictionary<int, GameObject> UIItemMap = new Dictionary<int, GameObject>();

        //container for the packing algorithm
        List<Container> containers = new List<Container>();
        //items for the packing algorithm
        List<Item> itemsToPack = new List<Item>();
        //the packing algorithm
        List<int> algorithms = new List<int>();
        m_MyCar.GetComponent<MyCar>().ClearTrunk();
        float scale = 1000f;
        containers.Add(new Container(1, new Decimal(containerSize.x* scale), new Decimal(containerSize.y* scale), new Decimal(containerSize.z* scale)));
        
        //convert items to the Item instances
        for(int i = 0; i < this.transform.childCount; i++)
        {
            GameObject child = this.transform.GetChild(i).gameObject;
            if (child == this.AddIcon) continue;
            
            Vector3 dims = child.GetComponent<ARItem>().size;
            int id = child.GetComponent<ARItem>().id;
            itemsToPack.Add(new Item(id, new Decimal(dims.x* scale), new Decimal(dims.y* scale), new Decimal(dims.z* scale), 1));
            UIItemMap[id] = child;
        }
        
        algorithms.Add((int)AlgorithmType.EB_AFIT);
        List<ContainerPackingResult> result = PackingService.Pack(containers, itemsToPack, algorithms);
        if (result.Count < 1) return false;
        AlgorithmPackingResult resultDetail = result[0].AlgorithmPackingResults[0];

        
        float total_volume=(float) containers[0].Volume;
        float item_volume = 0;

        //check the packing result
        foreach (Item i in resultDetail.PackedItems)
        {
            Vector3 localPosition = new Vector3((float)i.CoordX/ scale, (float)i.CoordY/ scale, (float)i.CoordZ/ scale);
            Vector3 size = new Vector3((float)i.Dim1/ scale, (float)i.Dim2/ scale, (float)i.Dim3/ scale);
            
            //Debug.Log("[BMWAR] item #+"+i.ID+": "+ localPosition + "   size: " + size);
            //visualize it in the trunk
            GameObject itemInContainer = m_MyCar.GetComponent<MyCar>().AddObjectToTrunk(localPosition, size);
            if (UIItemMap.ContainsKey(i.ID))
            {
                //change the UI color
                UIItemMap[i.ID].GetComponent<ARItem>().proxyItemBox = itemInContainer;
                UIItemMap[i.ID].GetComponent<RawImage>().color = PackedItemColor;
                UIItemMap.Remove(i.ID);
            }
            item_volume += (float) i.Volume;
        }
        
        //check the unpacked items and update their colors in the left UI pane.
        foreach(var key in UIItemMap.Keys)
        {
            UIItemMap[key].GetComponent<ARItem>().proxyItemBox = null;
            UIItemMap[key].GetComponent<RawImage>().color = UnPackedItemColor;
        }
        //calculate and update the free space
        float free_space = 100 - (item_volume * 100 / total_volume);
        m_MyCar.GetComponent<MyCar>().SetFreeSpace((int)free_space);
        if (resultDetail.IsCompletePack) return true;
        return false;
    }

}
