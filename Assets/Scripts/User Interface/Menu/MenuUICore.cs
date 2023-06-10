using Network;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

namespace UserInterface
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public sealed class MenuUICore : MonoBehaviour, IProgress<float>
    {
        #region Editor Fields
        [SerializeField] private Button _toGallery = null;
        [SerializeField] private GameObject _loadingScreen = null;
        [SerializeField] private GameObject _menuScreen = null;
        [SerializeField] private Slider _progressBar = null;
        #endregion

        #region Fields
        private GalleryObjectsLoader _galleryObjectsLoader = null;
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            _galleryObjectsLoader = FindObjectOfType<GalleryObjectsLoader>();            
        }

        private void Start()
        {
            _toGallery.onClick.AddListener(OnGalleryClick);

            _loadingScreen.SetActive(false);
            _menuScreen.SetActive(true);
        }
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
