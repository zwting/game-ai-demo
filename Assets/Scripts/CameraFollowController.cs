using UnityEngine;

public class CameraFollowController:MonoBehaviour
{
    [Tooltip("跟随目标的距离")]
    public float disToTarget = 4f;
    [Tooltip("距离水平面的角度")]
    public float angleToHorizontal = 45f;
    
    private static CameraFollowController _instance;

    public static CameraFollowController Instance
    {
        get { return _instance; }
    }

    private Camera _cachedCamera = null;

    public Camera CurCamera
    {
        private set { _cachedCamera = value; }
        get
        {
            if (_cachedCamera == null)
            {
                _cachedCamera = Camera.current;
            }

            return _cachedCamera;
        }
    }

    public Transform CurFollowTarget { private set; get; }
    
    public bool IsPause { private set; get; }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }

        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void FollowTarget(Transform target, Camera camera = null)
    {
        if (camera != null)
        {
            CurCamera = camera;
        }

        CurFollowTarget = target;

        IsPause = false;
    }

    public void SetActive(bool active)
    {
        IsPause = active;
    }

    private void Update()
    {
        if (IsPause)
        {
            return;
        }
        
        //先计算出相机应该在的位置
        
        
        
    }
}