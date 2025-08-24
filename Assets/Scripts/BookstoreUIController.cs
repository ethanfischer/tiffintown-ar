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
            var searchField = root.Q<TextField>();
            var profileButton = root.Q<Button>("profile-btn");
            var ebooksBtn = root.Q<Button>("ebooks-btn");
            var audiobooksBtn = root.Q<Button>("audiobooks-btn");
            
            if (ebooksBtn != null && audiobooksBtn != null)
            {
                Debug.Log("Mobile bookstore UI successfully loaded!");
                
                // Setup toggle functionality
                {
                    var trendingItems = root.Q<ScrollView>("trending-items");
                    
                    ebooksBtn.clicked += () => {
                        ebooksBtn.AddToClassList("toggle-active");
                        ebooksBtn.RemoveFromClassList("toggle-inactive");
                        audiobooksBtn.AddToClassList("toggle-inactive");
                        audiobooksBtn.RemoveFromClassList("toggle-active");
                        
                        // Show ebooks, hide audiobooks
                        if (trendingItems != null)
                        {
                            var book1 = trendingItems.Q<VisualElement>("book-1");
                            var book2 = trendingItems.Q<VisualElement>("book-2");
                            var magazine1 = trendingItems.Q<VisualElement>("magazine-1");
                            var magazine2 = trendingItems.Q<VisualElement>("magazine-2");
                            
                            if (book1 != null) book1.style.display = DisplayStyle.Flex;
                            if (book2 != null) book2.style.display = DisplayStyle.Flex;
                            if (magazine1 != null) magazine1.style.display = DisplayStyle.None;
                            if (magazine2 != null) magazine2.style.display = DisplayStyle.None;
                        }
                    };
                    
                    audiobooksBtn.clicked += () => {
                        audiobooksBtn.AddToClassList("toggle-active");
                        audiobooksBtn.RemoveFromClassList("toggle-inactive");
                        ebooksBtn.AddToClassList("toggle-inactive");
                        ebooksBtn.RemoveFromClassList("toggle-active");
                        
                        // Show audiobooks, hide ebooks
                        if (trendingItems != null)
                        {
                            var book1 = trendingItems.Q<VisualElement>("book-1");
                            var book2 = trendingItems.Q<VisualElement>("book-2");
                            var magazine1 = trendingItems.Q<VisualElement>("magazine-1");
                            var magazine2 = trendingItems.Q<VisualElement>("magazine-2");
                            
                            if (book1 != null) book1.style.display = DisplayStyle.None;
                            if (book2 != null) book2.style.display = DisplayStyle.None;
                            if (magazine1 != null) magazine1.style.display = DisplayStyle.Flex;
                            if (magazine2 != null) magazine2.style.display = DisplayStyle.Flex;
                        }
                    };
                    
                    // Initialize to show only ebooks by default
                    var trendingItemsInit = root.Q<ScrollView>("trending-items");
                    if (trendingItemsInit != null)
                    {
                        var magazine1 = trendingItemsInit.Q<VisualElement>("magazine-1");
                        var magazine2 = trendingItemsInit.Q<VisualElement>("magazine-2");
                        if (magazine1 != null) magazine1.style.display = DisplayStyle.None;
                        if (magazine2 != null) magazine2.style.display = DisplayStyle.None;
                    }
                }
            }
        }
    }
}