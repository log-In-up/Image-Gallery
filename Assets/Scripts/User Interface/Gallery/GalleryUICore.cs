using Data;
using Network;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

namespace UserInterface
{
    public sealed class GalleryUICore : SceneUICore
    {
        #region Editor Fields
        [SerializeField] private GameObject _galleryImage = null;
        [SerializeField] private GameObject _selectedImageDataHolder = null;
        [SerializeField] private RectTransform _galleryContent = null;
        [SerializeField] private RectTransform _galleryViewport = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        #endregion

        #region Fields
        private GalleryObjectsLoader _galleryObjectsLoader = null;
        private SelectedImageData _selectedImageData = null;
        private List<GalleryObject> _galleryImages = null;
        #endregion

        #region MonoBehaviour API
        protected override void Awake()
        {
            _galleryObjectsLoader = FindObjectOfType<GalleryObjectsLoader>();
            _selectedImageData = FindOrCreateSelectedImageData();
            _galleryImages = new List<GalleryObject>();

            base.Awake();
        }

        private void OnEnable()
        {
            _scrollRect.onValueChanged.AddListener(OnScroll);
        }

        private void Start()
        {
            InstantiateImages();

            UpdateDownloadImages();
        }

        private void OnDisable()
        {
            foreach (GalleryObject item in _galleryImages)
            {
                item.ImageSelected -= ImageSelected;
            }

            _scrollRect.onValueChanged.RemoveListener(OnScroll);
        }
        #endregion

        #region Methods
        private SelectedImageData FindOrCreateSelectedImageData()
        {
            SelectedImageData selectedImageData = FindObjectOfType<SelectedImageData>();

            if (selectedImageData == null)
            {
                GameObject imageHolder = Instantiate(_selectedImageDataHolder);
                DontDestroyOnLoad(imageHolder);
                imageHolder.GetSafeComponent(out selectedImageData);
            }

            return selectedImageData;
        }

        private void InstantiateImages()
        {
            foreach (string item in _galleryObjectsLoader.ImageNames.Keys)
            {
                GameObject image = Instantiate(_galleryImage, _galleryContent);

                image.GetSafeComponent(out GalleryObject component);
                component.Init(_galleryObjectsLoader.URL, item, _galleryObjectsLoader);

                _galleryImages.Add(component);
            }

            foreach (GalleryObject item in _galleryImages)
            {
                item.ImageSelected += ImageSelected;
            }

            Canvas.ForceUpdateCanvases();
        }

        private void UpdateDownloadImages()
        {
            foreach (GalleryObject item in _galleryImages)
            {
                if (item.RectTransform.IsVisible(_galleryViewport))
                {
                    if (!item.ImageIsSet)
                    {
                        item.LaunchImageDownload().Forget();
                    }
                }
            }
        }
        #endregion

        #region Overridden Methods
        protected internal override void AutorotateSettings()
        {
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
        }

        protected internal override void OnClickEscape()
        {
            SceneManager.LoadScene((int)Scenes.Menu);
        }
        #endregion

        #region Event Handlers
        private void ImageSelected(string url, string name, Sprite sprite)
        {
            _selectedImageData.SendImageData(name, url, sprite);

            SceneManager.LoadScene((int)Scenes.View);
        }

        private void OnScroll(Vector2 vector)
        {
            UpdateDownloadImages();
        }
        #endregion
    }
}