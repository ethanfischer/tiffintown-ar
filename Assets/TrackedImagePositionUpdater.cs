using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImagePositionUpdater : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;

    public GameObject modelPrefab;
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            if (spawnedPrefabs.TryGetValue(trackedImage.referenceImage.name, out var go))
            {
                Destroy(go);
                spawnedPrefabs.Remove(trackedImage.referenceImage.name);
            }
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (!spawnedPrefabs.ContainsKey(imageName))
        {
            GameObject prefab = Instantiate(modelPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
            prefab.transform.parent = trackedImage.transform;
            spawnedPrefabs[imageName] = prefab;
        }
        else
        {
            GameObject prefab = spawnedPrefabs[imageName];
            prefab.transform.position = trackedImage.transform.position;
            prefab.transform.rotation = trackedImage.transform.rotation;
        }

        spawnedPrefabs[imageName].SetActive(trackedImage.trackingState == TrackingState.Tracking);
    }
}
