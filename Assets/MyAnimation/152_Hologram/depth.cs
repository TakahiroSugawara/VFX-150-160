using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class depth : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.Depth;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
