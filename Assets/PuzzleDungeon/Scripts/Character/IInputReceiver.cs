using PuzzleDungeon.Input;

namespace PuzzleDungeon.Character
{
    public interface IInputReceiver
    {
        public void ReceiveInputUpdate(InputManager               input);
        public void ListenToInputEvents(InputManager        input);
        public void StopListeningToInputEvents(InputManager input);
    }
}