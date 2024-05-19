using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	// ReSharper disable once StaticMemberInGenericType
	private static readonly object InstanceLock = new();

	// ReSharper disable once StaticMemberInGenericType
	private static bool _quitting;

	public static T Instance
	{
		get
		{
			lock (InstanceLock)
			{
				if (_instance == null && !_quitting)
				{
					_instance = FindObjectOfType<T>();
					if (_instance == null)
					{
						var go = new GameObject(typeof(T).ToString());
						_instance = go.AddComponent<T>();

						DontDestroyOnLoad(_instance.gameObject);
					}
				}

				return _instance;
			}
		}
	}

	protected virtual void Awake()
	{
		if (_instance == null)
			_instance = gameObject.GetComponent<T>();
		else if (_instance.GetInstanceID() != GetInstanceID()) Destroy(gameObject);
		//throw new Exception($"Instance of {GetType().FullName} already exists, removing {ToString()}");
	}

	protected virtual void OnApplicationQuit()
	{
		_quitting = true;
	}
}