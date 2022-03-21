using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // Update is called once per frame
    private Camera cam;
    public Vector3 parallaxRatio = new Vector3(.25f,.25f,.25f);
    public Vector3 parallaxOffset = new Vector3(0,0,0);
    private void Start()
    {
        
    }
    void Update()
    {

        var cam = Camera.main;
        if (cam)
        {
            transform.position = new Vector3(cam.transform.position.x - cam.transform.position.x * parallaxRatio.x,
                                             cam.transform.position.y - cam.transform.position.y * parallaxRatio.y,
                                             transform.position.z);
        }
    }
}
