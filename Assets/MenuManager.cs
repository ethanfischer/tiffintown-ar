using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    Button _backButton;
    [SerializeField]
    Button _homeButton;
    [SerializeField]
    Button _shopButton;
    [SerializeField]
    Button _profileButton;
    

    void Start()
    {
        _homeButton.onClick.AddListener(GoHome);
        _shopButton.onClick.AddListener(GoShop);
        _profileButton.onClick.AddListener(GoProfile);
    }

    void GoHome()
    {
        Debug.Log("Go home");
    }
    
    void GoShop()
    {
        Debug.Log("Go shop");
    }
    
    void GoProfile()
    {
        Debug.Log("Go profile");
    }
}


public static class UIExtensions
{
    public static void Show(this CanvasGroup canvasGroup)
    {
        canvasGroup.gameObject.SetActive(true);
    }
    
    public static void Hide(this CanvasGroup canvasGroup)
    {
        canvasGroup.gameObject.SetActive(false);
    }
}