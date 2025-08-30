using System;
using UnityEngine;

namespace TiffinAR.UI
{
    [Serializable]
    public struct BookData
    {
        [Header("Book Information")]
        public string id;
        public string title;
        public string author;
        public string description;
        public Texture2D coverImage;
        public string coverImagePath;
        
        [Header("Shopify Integration")]
        public string shopifyProductId; // Shopify product variant ID (gid://shopify/ProductVariant/...)
        public float price; // Display price (should match Shopify price)
        public bool isAvailableOnShopify; // Whether the product is available for purchase
        
        [Header("Display Settings")]
        public string priceDisplayText; // Formatted price text (e.g., "$12.99")
        
        // Helper method to check if Shopify integration is configured
        public bool HasShopifyIntegration => !string.IsNullOrEmpty(shopifyProductId);
    }
}