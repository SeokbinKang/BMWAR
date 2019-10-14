using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARApplication : MonoBehaviour
{
    public static ARApplication ActiveInstance;
    private AppState mState;
    public GameObject promptText;
    // Start is called before the first frame update
    void Start()
    {
        ActiveInstance = this;
        mState = AppState.S0_Idle;
        Reset();
    }
    void Update()
    {
        UpdatePrompt();
    }
    private void Reset()
    {
        mState = AppState.S1_FindSpace;
    }
    /// <summary>
    /// Application Status 
    /// </summary>
    public AppState GetState()
    {
        return mState;
    }
    /// <summary>
    /// Populate a user prompt for each state
    /// </summary>
    private void UpdatePrompt()
    {
        if (mState == AppState.S0_Idle)
        {
            promptText.SetActive(false);
        } else
        {
            promptText.SetActive(true);
        }
        if(mState == AppState.S1_FindSpace)
        {
            promptText.GetComponent<UserPrompt>().SetPrompt(0);
        } else if (mState == AppState.S2_PlaceTruck)
        {
            promptText.GetComponent<UserPrompt>().SetPrompt(1);
        } else if (mState == AppState.S3_PlaceObject)
        {
            promptText.GetComponent<UserPrompt>().SetPrompt(2);
        } else if (mState == AppState.S4_MeasureObject)
        {
            promptText.GetComponent<UserPrompt>().SetPrompt(3);
        }
    }
    /// <summary>
    /// Change the app state
    /// </summary>
    public void SetStatus(AppState st) {
        this.mState = st;
    }

    /// <summary>
    /// Change the app state
    /// </summary>
    public void SetStatus(int index)
    {
        this.mState = (AppState)index;
    }
}
public enum AppState
{
    S0_Idle,
    S1_FindSpace,
    S2_PlaceTruck,
    S3_PlaceObject,
    S4_MeasureObject

}
