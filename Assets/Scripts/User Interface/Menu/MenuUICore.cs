using Network;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

namespace UserInterface
{
    public sealed class MenuUICore : SceneUICore, IProgress<float>
    {
        #region Editor Fields
        [SerializeField] private Button _toGallery = null;
        [SerializeField] private GameObject _loadingScreen = null;
        [SerializeField] private GameObject _menuScreen = null;
        [SerializeField] private GameObject _galleryObjectsHolder = null;
        [SerializeField] private Slider _progressBar = null;
        #endregion

        #region Fields
        private GalleryObjectsLoader _galleryObjectsLoader = null;
        #endregion

        #region MonoBehaviour API
        protected override void Awake()
        {
            _galleryObjectsLoader = FindOrCreateGalleryObjectsLoader();

            base.Awake();
        }

        private void Start()
        {
            _toGallery.onClick.AddListener(OnGalleryClick);

            _loadingScreen.SetActive(false);
            _menuScreen.SetActive(true);
        }
        #endregion

        #region Methods
        private GalleryObjectsLoader FindOrCreateGalleryObjectsLoader()
        {
            GalleryObjectsLoader galleryObjectsLoader = FindObjectOfType<GalleryObjectsLoader>();

            if (galleryObjectsLoader == null)
            {
                GameObject imageHolder = Instantiate(_galleryObjectsHolder);
                DontDestroyOnLoad(imageHolder);
                imageHolder.GetSafeComponent(out galleryObjectsLoader);
            }

            return galleryObjectsLoader;
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

        protected internal override void OnClickEscape() { }
        #endregion

        #region Button Handlers
        private void OnGalleryClick()
        {
            _toGallery.onClick.RemoveListener(OnGalleryClick);

            _galleryObjectsLoader.WebLoad(this).Forget();

            _loadingScreen.SetActive(true);
            _menuScreen.SetActive(false);
        }
        #endregion

        #region Interface Implementation
        public void Report(float value)
        {
            _progressBar.value = value;

            if (value >= 1)
            {
                SceneManager.LoadScene((int)Scenes.Gallery);
            }
        }
        #endregion
    }
}
