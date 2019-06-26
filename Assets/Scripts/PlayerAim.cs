using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Camera))]
public class PlayerAim : MonoBehaviour
{
    public int range = 4;
    Vector3 screenCenter;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        screenCenter = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        bool isCrystal = false;

        if (Physics.Raycast(cam.ScreenPointToRay(screenCenter), out hit, range)) {
            isCrystal = hit.transform.CompareTag("Crystal");
            GameManager.instance.VisibleCrystal(isCrystal);
        }

        if (isCrystal && CrossPlatformInputManager.GetButtonDown("Collect"))
        {
            GameManager.instance.CrystalCollected();
            Destroy(hit.transform.parent.gameObject);
        }
    }
}
