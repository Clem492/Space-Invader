using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance;

    public bool IsPause;

    private InputSystem_Actions controls;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        controls = new InputSystem_Actions();
        controls.UI.Pause.performed += ctx => Pause();
    }
    private void OnEnable()
    {
        controls.UI.Pause.Enable();
    }
    private void OnDisable()
    {
        controls.UI.Pause.Disable();
    }
    private void Pause()
    {
        Debug.Log("[GameManager] Pause() à implémenter");
    }
}
