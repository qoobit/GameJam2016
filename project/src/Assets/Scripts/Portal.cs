using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string SceneName;
    public string PortalName;

    public Vector3 direction;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }




    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Hero")
        {
            GameControl.control.sceneName = SceneName;
            GameControl.control.portalName = PortalName;
            GameControl.control.Save();
            if (SceneName != "")
            {
                SceneManager.LoadScene(SceneName);
            }
        }
        
    }
}


