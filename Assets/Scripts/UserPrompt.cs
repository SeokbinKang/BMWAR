using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserPrompt : MonoBehaviour
{
    public string[] prompt_list;
    public GameObject textObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetPrompt(int idx)
    {
        textObject.GetComponent<Text>().text = prompt_list[idx];
    }
}
