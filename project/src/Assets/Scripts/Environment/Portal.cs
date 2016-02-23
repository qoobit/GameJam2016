using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string NextSceneName;
    public string NextPortalName;

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
            GameControl.control.sceneName = NextSceneName;
            GameControl.control.portalName = NextPortalName;
            GameControl.control.Save();
            if (NextSceneName != "")
            {
                SceneManager.LoadScene(NextSceneName);
            }
        }
        
    }
}


