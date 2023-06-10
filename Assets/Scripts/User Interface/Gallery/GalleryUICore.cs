using Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UserInterface
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public sealed class GalleryUICore : MonoBehaviour
    {
        #region Editor Fields
        [SerializeField] private Camera _camera = null;
        [SerializeField] private GameObject _galleryImage = null;
        [SerializeField] private RectTransform _galleryContent = null;
        [SerializeField] private RectTransform _galleryViewport = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        #endregion

        #region Fields
        private GalleryObjectsLoader _galleryObjectsLoader = null;
        private List<GalleryObject> _galleryImages = null;
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            _galleryObjectsLoader = FindObjectOfType<GalleryObjectsLoader>();
            _galleryImages = new List<GalleryObject>();
        }

        private void Start()
        {
            foreach (string item in _galleryObjectsLoader.ImageNames)
            {
                GameObject image = Instantiate(_galleryImage, _galleryContent);

                image.GetSafeComponent(out GalleryObject component);
                component.Init(_galleryObjectsLoader.URL, item);

                _galleryImages.Add(component);
            }

            foreach (GalleryObject item in _galleryImages)
            {
                if (item.RectTransform.IsVisible(_galleryViewport))
                {
                    if (item.ImageIsSet) continue;
                    item.LaunchImageDownload().Forget();
                }
            }

            _scrollRect.onValueChanged.AddListener(OnScroll);
        }

        private void OnDisable()
        {
            _scrollRect.onValueChanged.RemoveListener(OnScroll);
        }
        #endregion

        #region Event Handlers
        private void OnScroll(Vector2 vector)
        {
            foreach (GalleryObject item in _galleryImages)
            {
                if (item.RectTransform.IsVisible(_galleryViewport))
                {
                    if (item.ImageIsSet) continue;
                    item.LaunchImageDownload().Forget();
                }
            }
        }
        #endregion
    }
}