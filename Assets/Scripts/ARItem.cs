using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARItem : MonoBehaviour
{
    public static int IdCounter=0;
    public Vector3 size;
    public Vector3 rotation;
    public Vector3 positionTrunk;
    public GameObject originalItemBox;
    public GameObject proxyItemBox;
    public bool placed;
    public int id;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponentInChildren<Text>().text = "Item #" + id;
    }
    public void initialize(GameObject ItemBox, Vector3 dimension)
    {
        rotation = Vector3.zero;
        positionTrunk = Vector3.zero;
        proxyItemBox = null;
        placed = false;
        originalItemBox = ItemBox;
        size = dimension;
        id = ++IdCounter;
    }
}
