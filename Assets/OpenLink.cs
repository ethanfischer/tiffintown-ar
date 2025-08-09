using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public string Link { get; set; }
    
    public void OpenLinkInBrowser()
    {
        Application.OpenURL(Link);
    }
}
