using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    #region Singleton
    public static GameManager singleton;

    private void Awake()
    {
        if (singleton == null)
        {
            DontDestroyOnLoad(gameObject);
            singleton = this;
        }
        else if (singleton != this)
        {
            DestroyImmediate(gameObject);
        }
    } 
    #endregion

    public SelectionController selectionController { get; private set; }


    // Use this for initialization
    private void Start () 
	{
        selectionController = GetComponent<SelectionController>();
    }
	
	// Update is called once per frame
	private void Update () 
	{
		
	}
}
