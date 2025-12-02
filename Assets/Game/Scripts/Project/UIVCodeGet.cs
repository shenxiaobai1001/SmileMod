using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVCodeGet : MonoBehaviour
{
    public static UIVCodeGet Instance { get; private set; }
    public Transform effectPos;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
