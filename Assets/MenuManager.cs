using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    CanvasGroup _mainMenu;
    [SerializeField]
    CanvasGroup _menu2;
    [SerializeField]
    Button _mainMenuButton;
    [SerializeField]
    Button _backButton;

    void Start()
    {
        _mainMenuButton.onClick.AddListener(GoToMenu2);
        _backButton.onClick.AddListener(Proceed);
    }
    
    void Proceed()
    {
        Debug.Log("Proceed");
        _mainMenu.Hide();
        _menu2.Hide();
    }

    void GoToMenu2()
    {
        Debug.Log("Go to menu 2");
        _mainMenu.Hide();
        _menu2.Show();
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