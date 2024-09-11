using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private bool initialized = false;

    /// <summary>
    /// 이미 종료처리 했는지 확인하는 변수
    /// </summary>
    private static bool isShutDown = false;

    /// <summary>
    /// 싱글톤 객체
    /// </summary>
    private static T instance;

    /// <summary>
    /// 싱글톤의 객체를 읽기 위한 프로퍼티
    /// </summary>
    public static T Inst
    {
        get
        {
            // 종료 처리 했을 때
            if (isShutDown)
            {
                // 경고 출력
                Debug.LogWarning($"{typeof(T).Name} 싱글톤은 이미 삭제 중이다.");
                // null 반환
                return null;
            }
            // instance가 없을 때
            if (instance == null)
            {
                T singletone = FindObjectOfType<T>();
                if (singletone == null)
                {
                    GameObject gameObj = new GameObject();
                    gameObj.name = $"{typeof(T).Name} Singletone";
                    singletone = gameObj.AddComponent<T>();
                }
                instance = singletone;

                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(instance.gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!initialized)
        {
            OnPreInitialize();
        }
        if (mode != LoadSceneMode.Additive)
        {
            OnIntialize();
        }
    }

    private void OnApplicationQuit()
    {
        isShutDown = true;
    }

    /// <summary>
    /// 싱글톤이 만들어질 때 단 한번만 호출될 초기화 함수
    /// </summary>
    protected virtual void OnPreInitialize()
    {
        initialized = true;
    }

    /// <summary>
    /// 싱글톤이 만들어지고 씬이 Single로 로드될 때마다 호출 될 초기화 함수
    /// </summary>
    protected virtual void OnIntialize()
    {

    }
}
