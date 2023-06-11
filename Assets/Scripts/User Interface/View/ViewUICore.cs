using Cysharp.Threading.Tasks;
using Data;
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
        [SerializeField] private Image _selectedImage = null;
        #endregion

        #region Fields
        private SelectedImageData _selectedImageData = null;
        private CancellationTokenSource _disableCancellation = null;
        #endregion

        #region MonoBehaviour API
        protected override void Awake()
        {
            _selectedImageData = FindObjectOfType<SelectedImageData>();
            _disableCancellation = new CancellationTokenSource();

            base.Awake();
        }

        private void OnEnable()
        {
            _back.onClick.AddListener(OnClickBack);
        }

        private void Start()
        {
            Tuple<string, Sprite> tuple = _selectedImageData.GetSelectedSprite();

            if (tuple.Item2 != null)
            {
                _selectedImage.sprite = tuple.Item2;
            }
            else
            {
                LaunchImageDownload(tuple.Item1).Forget();
            }
        }

        private void OnDisable()
        {
            _back.onClick.RemoveListener(OnClickBack);

            _disableCancellation.Cancel();
        }
        #endregion

        #region Methods
        internal async UniTaskVoid LaunchImageDownload(string url)
        {
            UnityWebRequest request = await UnityWebRequestTexture.GetTexture(url)
                .SendWebRequest()
                .WithCancellation(_disableCancellation.Token);

            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            request.Dispose();

            _selectedImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
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
        #endregion
    }
}
