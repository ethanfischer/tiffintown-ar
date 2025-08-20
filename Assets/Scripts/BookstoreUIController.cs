using UnityEngine;
using UnityEngine.UIElements;

namespace TiffinAR.UI
{
    public class BookstoreUIController : MonoBehaviour
    {
        [SerializeField] public VisualTreeAsset uiTemplate;
        
        private void Start()
        {
            SetupUI();
        }
        
        private void SetupUI()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("UIDocument component not found!");
                return;
            }
            
            if (uiTemplate == null)
            {
                Debug.LogError("UI Template not assigned!");
                return;
            }
            
            uiDocument.visualTreeAsset = uiTemplate;
            
            var root = uiDocument.rootVisualElement;
            
            // Find and setup UI elements
            var searchField = root.Q<TextField>("search-field");
            var cartButton = root.Q<Button>("cart-button");
            var profileButton = root.Q<Button>("profile-button");
            var ebooksBtn = root.Q<Button>("ebooks-btn");
            var audiobooksBtn = root.Q<Button>("audiobooks-btn");
            
            if (searchField != null && cartButton != null)
            {
                Debug.Log("Mobile bookstore UI successfully loaded!");
                
                // Setup toggle functionality
                if (ebooksBtn != null && audiobooksBtn != null)
                {
                    ebooksBtn.clicked += () => {
                        ebooksBtn.AddToClassList("toggle-active");
                        ebooksBtn.RemoveFromClassList("toggle-inactive");
                        audiobooksBtn.AddToClassList("toggle-inactive");
                        audiobooksBtn.RemoveFromClassList("toggle-active");
                    };
                    
                    audiobooksBtn.clicked += () => {
                        audiobooksBtn.AddToClassList("toggle-active");
                        audiobooksBtn.RemoveFromClassList("toggle-inactive");
                        ebooksBtn.AddToClassList("toggle-inactive");
                        ebooksBtn.RemoveFromClassList("toggle-active");
                    };
                }
            }
            else
            {
                Debug.LogWarning("Bookstore UI elements not found");
            }
        }
    }
}