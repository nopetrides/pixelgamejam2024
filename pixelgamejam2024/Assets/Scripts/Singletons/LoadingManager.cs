using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : Singleton<LoadingManager>
    {
        [SerializeField] private CanvasGroup _faderGroup;
        [SerializeField] private GameObject _loadingIcon;
        [SerializeField] private float _fadeTime = 0.5f;

        private AsyncOperation _sceneLoad;
        public Action OnFadeRemoved;
        public Action OnLoadComplete;

        private void Start()
        {
            _faderGroup.alpha = 0;
            _faderGroup.gameObject.SetActive(false);
        }

        #region Scene Load
        
        public void LoadScene(string sceneName)
        {
            _sceneLoad = SceneManager.LoadSceneAsync(sceneName);
            _sceneLoad.allowSceneActivation = false;
            StartCoroutine(PerformSceneLoading());
        }

        private IEnumerator PerformSceneLoading()
        {
            _faderGroup.gameObject.SetActive(true);
            _loadingIcon.SetActive(true);
            _faderGroup.DOFade(1, _fadeTime).OnComplete(FullyHidden);
            yield return new WaitUntil(() => _sceneLoad.isDone);
            LoadComplete();
        }

        private void FullyHidden()
        {
            _sceneLoad.allowSceneActivation = true;
        }

        private void LoadComplete()
        {
            OnLoadComplete?.Invoke();
            _faderGroup.DOFade(0, _fadeTime).OnComplete(FadeComplete);
        }

        private void FadeComplete()
        {
            _faderGroup.gameObject.SetActive(false);
            _loadingIcon.SetActive(false);
            OnFadeRemoved?.Invoke();
        }
        #endregion

        #region Transparent Blocker
        
        public void Wait(bool waiting)
        {
            if (waiting)
            {
                EnableTransparentLoader();
            }
            else
            {
                DisableTransparentLoader();
            }
        }
        
        private void EnableTransparentLoader()
        {
            _faderGroup.gameObject.SetActive(true);
            _faderGroup.DOFade(0.5f, _fadeTime).OnComplete(HalfFadeComplete);
        }

        private void DisableTransparentLoader()
        {
            _loadingIcon.SetActive(false);
            _faderGroup.DOFade(0f, _fadeTime).OnComplete(HalfFadeEnd);
        }

        private void HalfFadeComplete()
        {
            _loadingIcon.SetActive(true);
        }

        private void HalfFadeEnd()
        {
            _faderGroup.gameObject.SetActive(false);
            _loadingIcon.SetActive(false);
        }
        
        #endregion
    }