using System;
using UnityEngine;
using UnityEngine.Events;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] UnityEvent onAwake;
    [SerializeField] UnityEvent onStart;
    void Awake()
    {
        onAwake?.Invoke();
    }

    void Start()
    {
        onStart?.Invoke();
    }
}
