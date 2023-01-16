using PuzzleDungeon.Input;

namespace PuzzleDungeon.Character
{
    public interface IInputReceiver
    {
        public void ListenToInputEvents(InputManager        input);
        public void StopListeningToInputEvents(InputManager input);
    }
}