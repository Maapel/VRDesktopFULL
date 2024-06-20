using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    public int scrren_id = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector2 getTouchPos(Vector3 position)
    {
        var localPos = transform.InverseTransformPoint(position);
        return new Vector2(localPos.x , localPos.y);
    }

    public void sendTouch()
    {

    }
}
