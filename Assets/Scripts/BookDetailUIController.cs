using UnityEngine;
using UnityEngine.UIElements;

namespace TiffinAR.UI
{
    public class BookDetailUIController : MonoBehaviour
    {
        [SerializeField] public VisualTreeAsset bookDetailTemplate;
        
        private BookData currentBook;
        private System.Action onBackPressed;
        
        public void ShowBookDetail(BookData book, System.Action backCallback = null)
        {
            currentBook = book;
            onBackPressed = backCallback;
            SetupBookDetailUI();
        }
        
        private void SetupBookDetailUI()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("UIDocument component not found!");
                return;
            }
            
            if (bookDetailTemplate == null)
            {
                Debug.LogError("Book Detail Template not assigned!");
                return;
            }
            
            uiDocument.visualTreeAsset = bookDetailTemplate;
            
            var root = uiDocument.rootVisualElement;
            
            // Set book information
            var bookCover = root.Q<VisualElement>("book-cover-large");
            var bookTitle = root.Q<Label>("book-title");
            var bookAuthor = root.Q<Label>("book-author");
            var bookDescription = root.Q<Label>("book-description");
            var backButton = root.Q<Button>("back-button");
            var buyButton = root.Q<Button>("buy-button");
            
            // Update book details
            if (bookTitle != null) bookTitle.text = currentBook.title;
            if (bookAuthor != null) bookAuthor.text = currentBook.author;
            if (bookDescription != null) bookDescription.text = currentBook.description;
            
            // Set cover image
            if (bookCover != null && !string.IsNullOrEmpty(currentBook.coverImagePath))
            {
                bookCover.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(currentBook.coverImagePath));
            }
            
            // Setup button handlers
            if (backButton != null)
            {
                backButton.clicked += () => {
                    Debug.Log("Back button clicked - calling callback");
                    onBackPressed?.Invoke();
                };
            }
            else
            {
                Debug.LogError("Back button not found in BookDetail UI!");
            }
            
            if (buyButton != null)
            {
                buyButton.clicked += () => {
                    HandleBuyButton();
                };
            }
        }
        
        private void HandleBuyButton()
        {
            Debug.Log($"Buy button clicked for: {currentBook.title}");
            // Here you would implement the purchase logic
            // For now, just show a debug message
        }
    }
}