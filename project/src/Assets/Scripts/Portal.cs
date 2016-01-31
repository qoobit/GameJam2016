using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    public string SceneName;
    public string PortalName;


    void Start()
    {
        SceneName = PortalName = "";
    }

    // Update is called once per frame
    void Update()
    {

    }




    void OnTriggerEnter(Collider other)
    {
        GameControl.control.sceneName = SceneName;
        GameControl.control.portalName = PortalName;
        GameControl.control.Save();

        Application.LoadLevel(SceneName);
    }
}


