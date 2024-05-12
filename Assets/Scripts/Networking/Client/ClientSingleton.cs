using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{

    private static ClientSingleton instance;
    public ClientGameManager GameManager { get; private set;}

    public static ClientSingleton Instace // 3lshan nprotectha fa 3amelnha get bas 3shan ncall it men ai 7eta tania
    {
        get
        {
            if (instance != null) { return instance; }

            instance = FindObjectOfType<ClientSingleton>();

            if(instance == null)
            {
                Debug.LogError("no ClientSingleton in the scene!");
                return null;
            }

            return instance;
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager(); // for ugs logic odam (networking)

        return await GameManager.InitAsync();
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }


}
