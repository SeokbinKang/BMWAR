using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// A car class to manage the car information and related UI components
/// </summary>
public class MyCar : MonoBehaviour
{
    // an UI icon showing a car image
    public GameObject carImage;

    // an UI label showing a car name
    public GameObject carName;

    // selected car index
    public int MyCarIndex = 0;

    // the size of the trunk 
    public Vector3 TrunkSize;

    // UI instance to select a car model
    public GameObject UICarSelect;

    // 3D GameObject for the projected trunk model
    public GameObject TrunkGameObject;

    // 2D GameObject for managing user items
    public GameObject ItemInventory;
    // Start is called before the first frame update
    void Start()
    {
        MyCarIndex = -1;
        UICarSelect.SetActive(false);
        carName.GetComponent<Text>().text = "Unselected";
        TrunkSize = Vector3.zero;
        TrunkGameObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (MyCarIndex == -1 && UICarSelect.activeSelf==false)
        {
            OnClick();
            return;
        }
        //update item UIs
    }

    /// <summary>
    /// Retrieve car information upon user selection
    /// </summary>
    private void loadCar()
    {
        //update the image, name, size
        carImage.GetComponent<RawImage>().texture = Assets2D.getCarTexture(MyCarIndex);
        carName.GetComponent<Text>().text = Assets2D.getCarNames(MyCarIndex);
        TrunkSize = Assets2D.getTrunkSizes(MyCarIndex);

        //update 3D layout

    }
    /// <summary>
    /// Getter for the trunk size
    /// </summary>
    public Vector3 GetTrunkSize()
    {
        return TrunkSize;
    }
    /// <summary>
    /// Callback for selecting a car model
    /// </summary>
    public void OnSelectCar(int idx)
    {
        MyCarIndex = idx;
        loadCar();
        ItemInventory.GetComponent<ItemList>().EnableAutoFit(true);
       
    }
    /// <summary>
    /// Callback for showing a model selection UI
    /// </summary>
    public void OnClick()
    {
        //Open up the menu for selecting a car model
        UICarSelect.SetActive(true);

    }
    /// <summary>
    /// Function to increase the size of the trunk
    /// </summary>
    public void ExtendTrunk()
    {
        //TODO: increase the trunk size (by folding the rear seats)
    }

    /// <summary>
    /// Delete existing trunk objects
    /// </summary>
    public void DestroyTrunkObject()
    {
        if (this.TrunkGameObject == null) return;
        GameObject.Destroy(this.TrunkGameObject);
    }
    /// <summary>
    /// Delete item objects in the trunk
    /// </summary>
    public void ClearTrunk()
    {
        if(this.TrunkGameObject == null) return;
        TrunkGameObject.GetComponent<Trunk3D>().ClearTrunk();
    }
    public void SetTrunkGameObject(GameObject o)
    {
        this.TrunkGameObject = o;
    }
    /// <summary>
    /// Add a 3D item to the trunk
    /// </summary>
    public GameObject AddObjectToTrunk(Vector3 localPostion, Vector3 size)
    {
        if (TrunkGameObject == null) return null;
        return TrunkGameObject.GetComponent<Trunk3D>().AddItem(localPostion, size);
    }
    /// <summary>
    /// Add a 3D item to the inventory 
    /// </summary>
    public void AddItem(GameObject go, Vector3 dimension)
    {
        ItemInventory.GetComponent<ItemList>().AddItem(go, dimension);
    }

}
