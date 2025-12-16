using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickUrl : MonoBehaviour
{
    private void OnMouseDown()
    {
        Application.OpenURL("https://www.instagram.com/movi_click?utm_source=ig_web_button_share_sheet&igsh=ZDNlZDc0MzIxNw==");
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
