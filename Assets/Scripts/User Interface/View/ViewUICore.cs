using Cysharp.Threading.Tasks;
using Data;
using Network;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

namespace UserInterface
{
    public sealed class ViewUICore : SceneUICore
    {
        #region Editor Fields
        [SerializeField] private Button _back = null;
        [SerializeField] private Button _previousImage = null;
        [SerializeField] private Button _nextImage = null;
        [SerializeField] private Image _selectedImage = null;
        #endregion

        #region Fields
        private int _currentImage;
        private GalleryObjectsLoader _galleryObjectsLoader = null;
        private SelectedImageData _selectedImageData = null;
        private CancellationTokenSource _disableCancellation = null;
        #endregion

        #region MonoBehaviour API
        protected override void Awake()
        {
            _galleryObjectsLoader = FindObjectOfType<GalleryObjectsLoader>();
            _selectedImageData = FindObjectOfType<SelectedImageData>();
            _disableCancellation = new CancellationTokenSource();

            base.Awake();
        }

        private void OnEnable()
        {
            _back.onClick.AddListener(OnClickBack);
            _nextImage.onClick.AddListener(OnClickNext);
            _previousImage.onClick.AddListener(OnClickPrevious);
        }

        private void Start()
        {
            SetImage();
        }

        private void OnDisable()
        {
            _back.onClick.RemoveListener(OnClickBack);
            _nextImage.onClick.RemoveListener(OnClickNext);
            _previousImage.onClick.RemoveListener(OnClickPrevious);

            _disableCancellation.Cancel();
        }
        #endregion

        #region Methods
        private void SetImage()
        {
            Tuple<string, string, Sprite> tuple = _selectedImageData.GetSelectedSprite();

            if (tuple.Item3 != null)
            {
                _selectedImage.sprite = tuple.Item3;
            }
            else
            {
                LaunchImageDownload(tuple.Item1, tuple.Item2).Forget();
            }

            _currentImage = _galleryObjectsLoader.ImageKeys.IndexOf(tuple.Item2);
        }
        #endregion

        #region Public Methods
        internal async UniTaskVoid LaunchImageDownload(string url, string name)
        {
            UnityWebRequest request = await UnityWebRequestTexture.GetTexture(url)
                .SendWebRequest()
                .WithCancellation(_disableCancellation.Token);

            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            request.Dispose();

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
            _selectedImage.sprite = sprite;
            _galleryObjectsLoader.ImageNames[name] = sprite;
        }
        #endregion

        #region Overridden Methods
        protected internal override void AutorotateSettings()
        {
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
        }

        protected internal override void OnClickEscape()
        {
            SceneManager.LoadScene((int)Scenes.Gallery);
        }
        #endregion

        #region Event Handlers
        private void OnClickBack()
        {
            SceneManager.LoadScene((int)Scenes.Gallery);
        }

        private void OnClickPrevious()
        {
            if (_currentImage - 1 < 0) return;

            string key = _galleryObjectsLoader.ImageKeys[_currentImage - 1];

            if (_galleryObjectsLoader.ImageNames[key] != null)
            {
                _selectedImage.sprite = _galleryObjectsLoader.ImageNames[key];
            }
            else
            {
                _selectedImage.sprite = null;

                string imageUrl = $"{_galleryObjectsLoader.URL}{key}";
                LaunchImageDownload(imageUrl, key).Forget();
            }

            _currentImage -= 1;
        }

        private void OnClickNext()
        {
            if (_currentImage + 1 > _galleryObjectsLoader.ImageKeys.Count) return;

            string key = _galleryObjectsLoader.ImageKeys[_currentImage + 1];

            if (_galleryObjectsLoader.ImageNames[key] != null)
            {
                _selectedImage.sprite = _galleryObjectsLoader.ImageNames[key];
            }
            else
            {
                _selectedImage.sprite = null;

                string imageUrl = $"{_galleryObjectsLoader.URL}{key}";
                LaunchImageDownload(imageUrl, key).Forget();
            }

            _currentImage += 1;
        }
        #endregion
    }
}
