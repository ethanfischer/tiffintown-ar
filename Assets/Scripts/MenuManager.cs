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
    Button _profileButton;

    [SerializeField]
    GameObject _homeMenu;
    [SerializeField]
    GameObject _profileMenu;
    [SerializeField]
    BookMenu _bookMenu;

    //singleton unity pattern
    private static MenuManager _instance;
    public static MenuManager Instance
    { get
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<MenuManager>();
        }
        return _instance;
    } }


    void Start()
    {
        _homeButton.onClick.AddListener(GoHome);
        _profileButton.onClick.AddListener(GoProfile);
    }

    void GoHome()
    {
        _homeMenu.SetActive(true);
        _profileMenu.SetActive(false);
        _bookMenu.gameObject.SetActive(false);
        _backButton.GetComponentInChildren<TMP_Text>().text = "Home";
        _backButton.onClick.RemoveAllListeners();
        Debug.Log("Go home");
    }

    void GoShop()
    {
        _homeMenu.SetActive(false);
        _profileMenu.SetActive(false);
        _bookMenu.gameObject.SetActive(false);
        _backButton.GetComponentInChildren<TMP_Text>().text = "Shop";
        _backButton.onClick.RemoveAllListeners();
        Debug.Log("Go shop");
    }

    void GoProfile()
    {
        _homeMenu.SetActive(false);
        _profileMenu.SetActive(true);
        _bookMenu.gameObject.SetActive(false);
        _backButton.GetComponentInChildren<TMP_Text>().text = "Profile";
        _backButton.onClick.RemoveAllListeners();
        Debug.Log("Go profile");
    }

    public void GoBook(BookTitle bookTitle)
    {
        _homeMenu.SetActive(false);
        _profileMenu.SetActive(false);
        _bookMenu.SetBook(bookTitle);
        _bookMenu.gameObject.SetActive(true);
        _backButton.GetComponentInChildren<TMP_Text>().text = "<b>←</b>";
        _backButton.onClick.AddListener(GoHome);
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
