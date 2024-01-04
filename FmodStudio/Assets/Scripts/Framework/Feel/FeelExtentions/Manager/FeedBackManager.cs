using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThGold.Common;
using MoreMountains.Feedbacks;
using static MoreMountains.Feedbacks.MMF_Fade;
using MoreMountains.FeedbacksForThirdParty;
using System;
using UnityEngine.SceneManagement;
using ThGold.Pool;

namespace ThGold.Feel
{
    public enum LoadSceneType
    {
        Dirct,
        Async,
        Additive,
        Add,
    }
    public class FeedBackManager : MonoSingleton<FeedBackManager>
    {
        public AsyncOperation asyncLoadSceneOperation;
        #region Cache
        [Header("Cache")]
        [SerializeField] private static Dictionary<string, MMFeedbacks> StaticFeelbackDic;
        private Dictionary<string, GameObjectPool> AllPoolDic;
        #endregion

        #region depository 
        [Header("Depository")]

        public GameObject FeedbacksPoolParent;//�����
        public GameObject OperationParent;//������
        #endregion

        public void Awake()
        {
            if (FeedbacksPoolParent == null)
            {
                FeedbacksPoolParent = new GameObject("FeedbacksPond");
                FeedbacksPoolParent.transform.parent = this.transform;
            }
            if (OperationParent == null)
            {
                OperationParent = new GameObject("OperationParent");
                OperationParent.transform.parent = this.transform;
            }
            AllPoolDic = new Dictionary<string, GameObjectPool>();
            InitFeedBackList();

            InitPool();
        }


        private void InitFeedBackList()
        {
            StaticFeelbackDic = new Dictionary<string, MMFeedbacks>();
            MMFeedbacks[] mMArray = FindObjectsOfType<MMFeedbacks>();
            foreach (var feedback in mMArray)
            {
                StaticFeelbackDic.Add(feedback.transform.name, feedback);
            }

        }
        #region Pool
        private void InitPool()
        {

        }
        public void AddPool(string PoolName, int initSize, GameObject prefab, Transform parent)
        {
            AllPoolDic.Add(PoolName, new GameObjectPool(initSize, prefab, parent));
        }
        /// <summary>
        /// ��һ�������
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public GameObjectPool FindPoolByName(string Name)
        {
            if (AllPoolDic.ContainsKey(Name))
            {
                return AllPoolDic[Name];
            }
            else
            {
                Debug.LogError($"Error_FeedManager������Pool:{Name}");
                return null;
            }
        }
        #endregion
        /// <summary>
        ///����Feedback
        /// </summary>
        /// <param name="feedbackName"></param>
        public void PlayFeedBack(string feedbackName)
        {
            if (StaticFeelbackDic.ContainsKey(feedbackName))
            {
                StaticFeelbackDic[feedbackName].PlayFeedbacks();
            }
            else
            {
                Debug.LogError($"Error_Error_FeedManager������:{feedbackName}");
            }
        }
        public MMFeedbacks FindFeedBackByName(string feedbackName)
        {
            if (StaticFeelbackDic.ContainsKey(feedbackName))
            {
                return StaticFeelbackDic[feedbackName];
            }
            else
            {
                Debug.LogError($"Error_FeedManager������:{feedbackName}");
                return null;
            }
        }
        /// <summary>
        /// �Ӷ�����ó���,�Զ�����
        /// step1:�ó���
        /// step2:Play
        /// step3:computedʱ�Ż�
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="Target"></param>
        public void GetFeedBacks(string PoolName,Transform Target = null)
        {
            if (string.IsNullOrEmpty(PoolName))
            {
                Debug.LogError("Error_FeedManager_PoolNameΪ��!");
                return;
            }
            GameObjectPool pool = FindPoolByName(PoolName);
            GameObject FeedbackObj = pool.Get();
            if (Target == null)
            {
                FeedbackObj.transform.parent = OperationParent.transform;
            }
            else
            {
                FeedbackObj.transform.parent = Target;
                FeedbackObj.transform.position = Target.position;

            }
            MMFeedbacks Feedbacks = FeedbackObj.GetComponent<MMFeedbacks>();
            Feedbacks.Events.OnComplete.RemoveAllListeners();
            Feedbacks.Events.OnComplete.AddListener(() =>
            {
                ReturnFeedbacks(FeedbackObj,pool);
            });
            Feedbacks.Events.OnInited.AddListener(() =>
            {
                Feedbacks.PlayFeedbacks();
            });
            Feedbacks.Initialization();
        }
        /// <summary>
        /// �Żض����
        /// </summary>
        public void ReturnFeedbacks(GameObject objs, GameObjectPool pool)
        {
            Debug.Log("Return");
            objs.GetComponent<MMFeedbacks>().Events.OnComplete.RemoveAllListeners();
            objs.GetComponent<MMFeedbacks>().Events.OnInited.RemoveAllListeners();
            pool.Return(objs);
            objs.transform.parent = pool.Parent;
        }

        public void ReturnFeedBacks()
        {

        }

        public void OnCompleteTest()
        {
            Debug.Log("OnComplete");
        }

        /// <summary>
        /// ���س���
        /// </summary>
        /// <param name="feedbackName"></param>
        /// <param name="SceneName">��������</param>
        public void PlayLoadScene(string SceneName, LoadSceneType type = LoadSceneType.Dirct, string feedbackName = FeedBackCustomName.FeedBack_LoadScene)
        {
            if (StaticFeelbackDic.ContainsKey(feedbackName))
            {
                foreach (var feedback in StaticFeelbackDic[feedbackName].Feedbacks)
                {
                    if (feedback.Label.Equals("Load Scene"))
                    {
                        if (type == LoadSceneType.Dirct)
                        {
                            MMFeedbackLoadScene loadScene = (MMFeedbackLoadScene)feedback;
                            loadScene.DestinationSceneName = SceneName;
                            loadScene.LoadingMode = MMFeedbackLoadScene.LoadingModes.Direct;
                            StaticFeelbackDic[feedbackName].PlayFeedbacks();
                        }
                        if (type == LoadSceneType.Additive)
                        {
                            MMFeedbackLoadScene loadScene = (MMFeedbackLoadScene)feedback;
                            loadScene.DestinationSceneName = SceneName;
                            loadScene.LoadingMode = MMFeedbackLoadScene.LoadingModes.MMAdditiveSceneLoadingManager;
                            StaticFeelbackDic[feedbackName].PlayFeedbacks();
                        }
                        if (type == LoadSceneType.Add)
                        {
                            SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
                        }
                        if (type == LoadSceneType.Async)
                        {
                            SceneManager.LoadSceneAsync("FeelAsyncScene", LoadSceneMode.Single);
                            StartCoroutine(LoadSceneCoroutine(SceneName));
                            asyncLoadSceneOperation.allowSceneActivation = false;
                        }
                        break;
                    }
                }
            }
        }
        private IEnumerator LoadSceneCoroutine(string sceneName)
        {
            // ж��֮ǰ�ĳ���
            //SceneManager.UnloadSceneAsync("PreviousScene");

            // �첽�����³���
            asyncLoadSceneOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            // �ȴ������������
            while (!asyncLoadSceneOperation.isDone)
            {
                yield return null;
            }
        }
        public void UnloadScene(string sceneName, string feedbackName = FeedBackCustomName.FeedBack_UnLoadScene)
        {
            if (StaticFeelbackDic.ContainsKey(feedbackName))
            {
                foreach (var feedback in StaticFeelbackDic[feedbackName].Feedbacks)
                {
                    if (feedback.Label.Equals("Unload Scene"))
                    {
                        MMFeedbackUnloadScene loadScene = (MMFeedbackUnloadScene)feedback;
                        loadScene.SceneName = sceneName;
                        StaticFeelbackDic[feedbackName].PlayFeedbacks();
                        break;
                    }
                }
            }
        }
        public void PlayFade(string feedbackName = FeedBackCustomName.FeedBack_Fade)
        {
            StaticFeelbackDic[feedbackName]?.PlayFeedbacks();
        }
        /// <summary>
        /// ������Ҫ�������Ʒid FadType��tureΪFade IN�� false ΪFade out
        /// </summary>
        /// <param name="feedbackName"></param>
        /// <param name="id">ID</param>
        /// <param name="FadType">tureΪFade IN�� false ΪFade out</param>
        public void SetFade(string feedbackName, int id = 0, bool FadType = true)
        {
            if (StaticFeelbackDic.ContainsKey(feedbackName))
            {
                foreach (var feedback in StaticFeelbackDic[feedbackName].Feedbacks)
                {
                    if (feedback.Label.Equals("Fade"))
                    {
                        MMFeedbackFade fade = (MMFeedbackFade)feedback;
                        fade.ID = id;
                        fade.FadeType = FadType == true ? MMFeedbackFade.FadeTypes.FadeIn : MMFeedbackFade.FadeTypes.FadeOut;
                        break;
                    }
                }
            }
        }
        public void PlayCineTransition(string feedbackName = FeedBackCustomName.FeedBack_CineTransition)
        {
            StaticFeelbackDic[feedbackName]?.PlayFeedbacks();
        }
        /// <summary>
        /// ��ǰ����ʹ��eventģʽ��û�а������
        /// </summary>
        /// <param name="feedbackName"></param>
        /// <param name="Channel"></param>
        public void SetCineTransition(string feedbackName = FeedBackCustomName.FeedBack_CineTransition, int Channel = 0)
        {
            if (StaticFeelbackDic.ContainsKey(feedbackName))
            {
                foreach (var feedback in StaticFeelbackDic[feedbackName].Feedbacks)
                {
                    if (feedback.Label.Equals("Cinemachine Transition"))
                    {
                        MMFeedbackCinemachineTransition trans = (MMFeedbackCinemachineTransition)feedback;
                        trans.Channel = Channel;
                        break;
                    }
                }
            }
        }
        public void PlayParticle(GameObject prefab, Vector3 v3, string feedbackName = FeedBackCustomName.FeedBack_PlayParticles)
        {
            if (StaticFeelbackDic.ContainsKey(feedbackName))
            {
                foreach (var feedback in StaticFeelbackDic[feedbackName].Feedbacks)
                {
                    if (feedback.Label.Equals("Particles Instantiation"))
                    {
                        MMFeedbackParticlesInstantiation particle = (MMFeedbackParticlesInstantiation)feedback;
                        particle.Mode = MMFeedbackParticlesInstantiation.Modes.Cached;
                        particle.ParticlesPrefab = prefab.GetComponent<ParticleSystem>();
                        StaticFeelbackDic[feedbackName].PlayFeedbacks();
                        break;
                    }
                }
            }
        }

    }
}
