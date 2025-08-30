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
            var bookPrice = root.Q<Label>("book-price");
            var bookDescription = root.Q<Label>("book-description");
            var backButton = root.Q<Button>("back-button");
            var buyButton = root.Q<Button>("buy-button");
            
            // Update book details
            if (bookTitle != null) bookTitle.text = currentBook.title;
            if (bookAuthor != null) bookAuthor.text = currentBook.author;
            if (bookPrice != null) bookPrice.text = !string.IsNullOrEmpty(currentBook.priceDisplayText) ? currentBook.priceDisplayText : $"${currentBook.price:F2}";
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
            
            // Check if Shopify integration is configured
            if (!currentBook.HasShopifyIntegration)
            {
                Debug.LogWarning($"No Shopify product ID configured for {currentBook.title}");
                ShowErrorMessage("This product is not available for purchase at the moment.");
                return;
            }
            
            if (!currentBook.isAvailableOnShopify)
            {
                ShowErrorMessage("This product is currently out of stock.");
                return;
            }
            
            // Check if ShopifyManager exists
            if (ShopifyManager.Instance == null)
            {
                Debug.LogError("ShopifyManager not found in scene!");
                ShowErrorMessage("Shopping service is not available. Please try again later.");
                return;
            }
            
            if (!ShopifyManager.Instance.IsConfigured())
            {
                Debug.LogError("ShopifyManager is not configured!");
                ShowErrorMessage("Shopping service is not properly configured.");
                return;
            }
            
            // Show loading state
            SetBuyButtonLoading(true);
            
            // Create cart and get checkout URL
            ShopifyManager.Instance.CreateCartAndCheckout(
                currentBook.shopifyProductId,
                1, // quantity
                onSuccess: (checkoutUrl) => {
                    SetBuyButtonLoading(false);
                    Debug.Log($"Checkout URL received: {checkoutUrl}");
                    // Open checkout URL in browser
                    ShopifyManager.Instance.OpenCheckoutUrl(checkoutUrl);
                },
                onError: (errorMessage) => {
                    SetBuyButtonLoading(false);
                    Debug.LogError($"Shopify error: {errorMessage}");
                    ShowErrorMessage($"Purchase failed: {errorMessage}");
                }
            );
        }
        
        private void SetBuyButtonLoading(bool isLoading)
        {
            var buyButton = GetComponent<UIDocument>()?.rootVisualElement?.Q<Button>("buy-button");
            if (buyButton != null)
            {
                buyButton.text = isLoading ? "Processing..." : "Buy Now";
                buyButton.SetEnabled(!isLoading);
            }
        }
        
        private void ShowErrorMessage(string message)
        {
            // For now, just log the error. You could enhance this with a proper error UI later
            Debug.LogError($"Purchase Error: {message}");
            
            // Optional: Show a temporary message on the buy button
            var buyButton = GetComponent<UIDocument>()?.rootVisualElement?.Q<Button>("buy-button");
            if (buyButton != null)
            {
                string originalText = buyButton.text;
                buyButton.text = "Error - Try Again";
                
                // Reset button text after 3 seconds
                StartCoroutine(ResetButtonTextAfterDelay(buyButton, originalText, 3f));
            }
        }
        
        private System.Collections.IEnumerator ResetButtonTextAfterDelay(Button button, string originalText, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (button != null)
            {
                button.text = originalText;
            }
        }
    }
}