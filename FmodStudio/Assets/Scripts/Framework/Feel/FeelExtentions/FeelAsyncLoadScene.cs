using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ThGold.Feel
{
    public class FeelAsyncLoadScene : MonoBehaviour
    {
        public string FeelAsyncScene = "FeelAsyncScene";
        private AsyncOperation asyncOperation => FeedBackManager.Instance.asyncLoadSceneOperation;
        public Slider loadingBar;

        void Start()
        {
            // �첽���س������������������볡��
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.E))
            {
                // ������볡��
                if (asyncOperation.progress < 0.9f)
                    return;
                asyncOperation.allowSceneActivation = true;
                FeedBackManager.Instance.UnloadScene(FeelAsyncScene);
            }

            // ���½�������ʾ
            loadingBar.value = asyncOperation.progress;
        }
    }
}
