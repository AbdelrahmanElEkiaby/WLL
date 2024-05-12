using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;
    public HostGameManger GameManger { get; private set; }
    public static HostSingleton Instance
    {
        get
        {
            if(instance != null) { return instance; }

            instance = FindObjectOfType<HostSingleton>();

            if(instance == null) { return null; }

            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        GameManger = new HostGameManger(); // for ugs logic

    }

    private void OnDestroy()
    {
        GameManger?.Dispose();
    }
}
