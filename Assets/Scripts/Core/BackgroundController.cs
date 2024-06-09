using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] GameObject MainBackground;
    [SerializeField] GameObject OvertimeBackground;

    public void ActivateMainBG()
    {
        MainBackground.SetActive(true);
        OvertimeBackground.SetActive(false);
    }
    public void ActivatOverTimeBG()
    {
        OvertimeBackground.SetActive(true);
        MainBackground.SetActive(false);
    }
    
}
