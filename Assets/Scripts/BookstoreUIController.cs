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
            var booksBtn = root.Q<Button>("books-btn");
            var magazinesBtn = root.Q<Button>("magazines-btn");
            
            if (searchField != null && cartButton != null)
            {
                Debug.Log("Mobile bookstore UI successfully loaded!");
                
                // Setup toggle functionality
                if (booksBtn != null && magazinesBtn != null)
                {
                    var trendingItems = root.Q<ScrollView>("trending-items");
                    
                    booksBtn.clicked += () => {
                        booksBtn.AddToClassList("toggle-active");
                        booksBtn.RemoveFromClassList("toggle-inactive");
                        magazinesBtn.AddToClassList("toggle-inactive");
                        magazinesBtn.RemoveFromClassList("toggle-active");
                        
                        // Show books, hide magazines
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
                    
                    magazinesBtn.clicked += () => {
                        magazinesBtn.AddToClassList("toggle-active");
                        magazinesBtn.RemoveFromClassList("toggle-inactive");
                        booksBtn.AddToClassList("toggle-inactive");
                        booksBtn.RemoveFromClassList("toggle-active");
                        
                        // Show magazines, hide books
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
                    
                    // Initialize to show only books by default
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
            else
            {
                Debug.LogWarning("Bookstore UI elements not found");
            }
        }
    }
}