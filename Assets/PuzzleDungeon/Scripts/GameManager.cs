using PuzzleDungeon.Tools;
using UnityEngine;

namespace PuzzleDungeon
{
    public class GameManager : Singleton<GameManager>
    {
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}