using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
   [SerializeField] private TMP_InputField joinCodeField;
   
    public async void StartHost()
    {
        await HostSingleton.Instance.GameManger.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instace.GameManager.StartClientAsync(joinCodeField.text);
    }
}
