using Unity.Netcode;
using UnityEngine;

public class NetworkedAnimatorBoolController : NetworkBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetBoolParameterServerRpc(string parameterName, bool value)
    {
        SetBoolParameterClientRpc(parameterName, value);
    }

    [ClientRpc]
    public void SetBoolParameterClientRpc(string parameterName, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(parameterName, value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TriggerTransitionToPanicServerRpc()
    {
        TriggerTransitionToPanicClientRpc();
    }

    [ClientRpc]
    public void TriggerTransitionToPanicClientRpc()
    {
        if (animator != null)
        {
            animator.SetTrigger("TransitionToPanic");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetTriggerServerRpc()
    {
        ResetTriggerClientRpc();
    }


    [ClientRpc]
    public void ResetTriggerClientRpc()
    {
        if (animator != null)
        {
            animator.ResetTrigger("TransitionToPanic");
        }
    }
}
