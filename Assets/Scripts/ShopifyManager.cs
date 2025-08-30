using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace TiffinAR.UI
{
    [System.Serializable]
    public class ShopifyConfig
    {
        [Header("Shopify Store Configuration")]
        [Tooltip("Your Shopify store domain (e.g., your-store.myshopify.com)")]
        public string shopDomain = "";
        
        [Header("API Access")]
        [Tooltip("Your Storefront API access token")]
        public string storefrontAccessToken = "";
        
        [Header("API Settings")]
        [Tooltip("API version to use")]
        public string apiVersion = "2024-10";
    }
    
    [System.Serializable]
    public class ShopifyProduct
    {
        public string id;
        public string title;
        public string description;
        public string handle;
        public ShopifyVariant[] variants;
    }
    
    [System.Serializable]
    public class ShopifyVariant
    {
        public string id;
        public string title;
        public string price;
        public bool available;
    }
    
    [System.Serializable]
    public class ShopifyCart
    {
        public string id;
        public string checkoutUrl;
        public ShopifyLineItem[] lines;
    }
    
    [System.Serializable]
    public class ShopifyLineItem
    {
        public string id;
        public int quantity;
        public ShopifyVariant merchandise;
    }
    
    public class ShopifyManager : MonoBehaviour
    {
        [SerializeField] private ShopifyConfig config;
        
        private string GraphQLEndpoint => $"https://{config.shopDomain}/api/{config.apiVersion}/graphql.json";
        
        public static ShopifyManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void CreateCartAndCheckout(string productId, int quantity = 1, Action<string> onSuccess = null, Action<string> onError = null)
        {
            StartCoroutine(CreateCartAndCheckoutCoroutine(productId, quantity, onSuccess, onError));
        }
        
        private IEnumerator CreateCartAndCheckoutCoroutine(string productId, int quantity, Action<string> onSuccess, Action<string> onError)
        {
            if (string.IsNullOrEmpty(config.shopDomain) || string.IsNullOrEmpty(config.storefrontAccessToken))
            {
                onError?.Invoke("Shopify configuration not set. Please configure shop domain and access token.");
                yield break;
            }
            
            // GraphQL mutation to create a cart with the product
            string mutation = @"
            mutation cartCreate($cartInput: CartInput!) {
              cartCreate(input: $cartInput) {
                cart {
                  id
                  checkoutUrl
                  lines(first: 10) {
                    edges {
                      node {
                        id
                        quantity
                        merchandise {
                          ... on ProductVariant {
                            id
                            title
                            price {
                              amount
                            }
                          }
                        }
                      }
                    }
                  }
                }
                userErrors {
                  field
                  message
                }
              }
            }";
            
            // Create simple JSON manually (Unity JsonUtility doesn't handle complex nested objects well)
            string jsonData = $@"{{
              ""query"": ""{mutation.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "")}"",
              ""variables"": {{
                ""cartInput"": {{
                  ""lines"": [{{
                    ""merchandiseId"": ""{productId}"",
                    ""quantity"": {quantity}
                  }}]
                }}
              }}
            }}";
            
            using (UnityWebRequest request = new UnityWebRequest(GraphQLEndpoint, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                
                // Set headers
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("X-Shopify-Storefront-Access-Token", config.storefrontAccessToken);
                
                Debug.Log($"Sending Shopify request to: {GraphQLEndpoint}");
                Debug.Log($"Request body: {jsonData}");
                Debug.Log($"Access token (first 10 chars): {config.storefrontAccessToken.Substring(0, Math.Min(10, config.storefrontAccessToken.Length))}...");
                
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseText = request.downloadHandler.text;
                    Debug.Log($"Shopify response: {responseText}");
                    
                    try
                    {
                        var response = JsonUtility.FromJson<ShopifyCartCreateResponse>(responseText);
                        
                        Debug.Log($"Full Shopify response: {responseText}");
                        
                        if (response.data?.cartCreate?.cart?.checkoutUrl != null)
                        {
                            string checkoutUrl = response.data.cartCreate.cart.checkoutUrl;
                            Debug.Log($"Checkout URL created: {checkoutUrl}");
                            onSuccess?.Invoke(checkoutUrl);
                        }
                        else if (response.data?.cartCreate?.userErrors?.Length > 0)
                        {
                            string errorMsg = "Shopify Error: " + response.data.cartCreate.userErrors[0].message;
                            Debug.LogError(errorMsg);
                            Debug.LogError($"User error field: {response.data.cartCreate.userErrors[0].field}");
                            onError?.Invoke(errorMsg);
                        }
                        else
                        {
                            Debug.LogError($"Product ID used: {productId}");
                            onError?.Invoke("Failed to create checkout URL. Please check your product ID and configuration.");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to parse Shopify response: {e.Message}");
                        onError?.Invoke($"Failed to parse response: {e.Message}");
                    }
                }
                else
                {
                    string responseText = request.downloadHandler?.text ?? "No response";
                    string errorMsg = $"Shopify API Error: {request.error}. Status: {request.responseCode}";
                    Debug.LogError(errorMsg);
                    Debug.LogError($"Response body: {responseText}");
                    Debug.LogError($"Request URL was: {request.url}");
                    onError?.Invoke(errorMsg);
                }
            }
        }
        
        public void OpenCheckoutUrl(string checkoutUrl)
        {
            if (string.IsNullOrEmpty(checkoutUrl))
            {
                Debug.LogError("Checkout URL is empty");
                return;
            }
            
            Debug.Log($"Opening checkout URL: {checkoutUrl}");
            Application.OpenURL(checkoutUrl);
        }
        
        [System.Serializable]
        private class ShopifyCartCreateResponse
        {
            public ShopifyData data;
        }
        
        [System.Serializable]
        private class ShopifyData
        {
            public ShopifyCartCreate cartCreate;
        }
        
        [System.Serializable]
        private class ShopifyCartCreate
        {
            public ShopifyCartData cart;
            public ShopifyUserError[] userErrors;
        }
        
        [System.Serializable]
        private class ShopifyCartData
        {
            public string id;
            public string checkoutUrl;
        }
        
        [System.Serializable]
        private class ShopifyUserError
        {
            public string field;
            public string message;
        }
        
        // Method to validate configuration
        public bool IsConfigured()
        {
            return !string.IsNullOrEmpty(config.shopDomain) && 
                   !string.IsNullOrEmpty(config.storefrontAccessToken);
        }
        
        // Method to set configuration at runtime
        public void SetConfiguration(string shopDomain, string accessToken, string apiVersion = "2025-01")
        {
            config.shopDomain = shopDomain;
            config.storefrontAccessToken = accessToken;
            config.apiVersion = apiVersion;
        }
    }
}