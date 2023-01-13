using System;
using UnityEngine;

namespace Tide
{
    public class GameManager : Singleton<GameManager>
    {
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;   
        }
    }
}