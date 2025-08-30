using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace TiffinAR.UI
{
    public class BookstoreUIController : MonoBehaviour
    {
        [SerializeField] public VisualTreeAsset uiTemplate;
        [SerializeField] private BookDetailUIController bookDetailController;
        
        // Sample book data
        private Dictionary<string, BookData> bookDatabase;
        
        private void Start()
        {
            InitializeBookDatabase();
            SetupUI();
        }
        
        private void InitializeBookDatabase()
        {
            bookDatabase = new Dictionary<string, BookData>
            {
                ["book-1"] = new BookData
                {
                    id = "book-1",
                    title = "The Story of Josephine Baker",
                    author = "illustrated by Sophie",
                    description = "A captivating biography of the legendary entertainer and civil rights activist Josephine Baker. This beautifully illustrated book tells the story of her rise from poverty to international stardom, her work as a spy during World War II, and her tireless fight for civil rights.",
                    coverImagePath = "thestoryofjosephinebaker",
                    price = 12.99f
                },
                ["book-2"] = new BookData
                {
                    id = "book-2",
                    title = "The Story of Ruth Harkness",
                    author = "illustrated by Sophie",
                    description = "The remarkable true story of Ruth Harkness, the American socialite who became the first person to bring a live giant panda to the Western world. An adventure story of determination, courage, and conservation.",
                    coverImagePath = "thestoryofruthharkness",
                    price = 14.99f
                }
            };
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
                
                // Setup book click handlers
                SetupBookClickHandlers(root);
            }
        }
        
        private void SetupBookClickHandlers(VisualElement root)
        {
            var trendingItems = root.Q<ScrollView>("trending-items");
            if (trendingItems != null)
            {
                // Add click handlers for each book
                var book1 = trendingItems.Q<VisualElement>("book-1");
                var book2 = trendingItems.Q<VisualElement>("book-2");
                
                if (book1 != null)
                {
                    book1.RegisterCallback<ClickEvent>(evt => {
                        if (bookDatabase.ContainsKey("book-1"))
                        {
                            ShowBookDetail(bookDatabase["book-1"]);
                        }
                    });
                }
                
                if (book2 != null)
                {
                    book2.RegisterCallback<ClickEvent>(evt => {
                        if (bookDatabase.ContainsKey("book-2"))
                        {
                            ShowBookDetail(bookDatabase["book-2"]);
                        }
                    });
                }
            }
        }
        
        private void ShowBookDetail(BookData book)
        {
            // If not assigned, try to find it automatically
            if (bookDetailController == null)
            {
                bookDetailController = FindObjectOfType<BookDetailUIController>();
            }
            
            if (bookDetailController != null)
            {
                bookDetailController.ShowBookDetail(book, () => {
                    // Back button callback - return to main bookstore view
                    SetupUI();
                });
            }
            else
            {
                Debug.LogError("BookDetailController not found! Make sure BookDetailUIController is in the scene.");
            }
        }
    }
}