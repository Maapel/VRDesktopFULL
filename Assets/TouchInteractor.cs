using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInteractor : MonoBehaviour
{
    RaycastHit touch;
    [Header("Touch Interactor Fingers")]
    [Tooltip("Add the fingers last bone(eg . b_l_index3) transforms ")]
        [SerializeField] private List<Transform> fingers;
        

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform finger in fingers)
        {
            var tip = finger.GetChild(2);
            if (Physics.Raycast(tip.position, tip.right, out touch,(tip.position - finger.position).magnitude))
            {

                if (touch.transform.CompareTag("Screen"))
                {
                    var screen = touch.transform.parent;
                    var pos = screen.GetComponent<ScreenController>().getTouchPos(touch.point);
                    Debug.Log(pos.x.ToString() +"  "+ pos.y.ToString());
                }
            }
        }
    }
}
