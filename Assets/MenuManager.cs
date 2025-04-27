using TMPro;
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


    [SerializeField]
    GameObject _homeMenu;
    [SerializeField]
    GameObject _shopMenu;
    [SerializeField]
    GameObject _profileMenu;
    

    void Start()
    {
        _homeButton.onClick.AddListener(GoHome);
        _shopButton.onClick.AddListener(GoShop);
        _profileButton.onClick.AddListener(GoProfile);
    }

    void GoHome()
    {
        _homeMenu.SetActive(true);
        _shopMenu.SetActive(false);
        _profileMenu.SetActive(false);
        _backButton.GetComponentInChildren<TMP_Text>().text = "Home";
        Debug.Log("Go home");
    }
    
    void GoShop()
    {
        _homeMenu.SetActive(false);
        _shopMenu.SetActive(true);
        _profileMenu.SetActive(false);
        _backButton.GetComponentInChildren<TMP_Text>().text = "Shop";
        Debug.Log("Go shop");
    }
    
    void GoProfile()
    {
        _homeMenu.SetActive(false);
        _shopMenu.SetActive(false);
        _profileMenu.SetActive(true);
        _backButton.GetComponentInChildren<TMP_Text>().text = "Profile";
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