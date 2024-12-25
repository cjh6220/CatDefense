using UnityEngine;

// MonoBehaviour를 상속받은 싱글톤 기본 구현
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null; // _instance는 싱글톤 인스턴스를 저장하는 private static 변수

    // 싱글톤 인스턴스를 저장
    public static T Instance
    {
        get
        {
            // 인스턴스가 없다면 씬에서 찾거나 생성.
            if (_instance == null)
            {
                // 씬 전환 시 객체 유지.
                _instance = FindObjectOfType<T>(true);
            }

            // 싱글톤 인스턴스 반환
            return _instance;
        }
    }

    // 객체 생성 시 호출
    protected virtual void Awake()
    {
        // DontDestroyOnLoad는 씬이 변경되어도 객체가 파괴하지 않기
        DontDestroyOnLoad(this); 
    }
}

// 인스턴스가 없을 때 새로운 게임 오브젝트를 자동으로 생성하는 싱글톤 구현
public class PhoenixSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 싱글톤 인스턴스 저장
    private static T _instance = null;

    // Instance는 싱글톤 인스턴스에 접근할 수 있는 public static 프로퍼티
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>(true);

                // 필요하다면 새 게임 오브젝트를 생성
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                }

            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
    }
}

// 싱글톤 인스턴스를 관리하며 애플리케이션이 종료될 때 파괴 방지.
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance; // 싱글톤 인스턴스 저장.
    private static bool _applicationIsQuitting = false; // 애플리케이션이 종료되는지 표시.

    // 인스턴스에 접근할 수 있는 프로퍼티
    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning(typeof(T) + " [Singleton] instance is already destroyed on application quit. Returning null.");
                return null;
            }

            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).ToString());
                    _instance = singletonObject.AddComponent<T>();
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    // 객체 중복 생성 방지
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 애플리케이션 종료 시점에 싱글턴 인스턴스가 파괴됨을 표시
    // 서버 모듈 때문에 임시 주석처리
    //protected virtual void OnDestroy()
    //{
    //    if (_instance == this)
    //    {
    //        _instance = null;
    //        _applicationIsQuitting = true;
    //    }
    //}
}