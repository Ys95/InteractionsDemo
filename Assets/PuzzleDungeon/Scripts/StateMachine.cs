using System;

namespace Tide
{
    public class States<T> where T : Enum
    {
        public event Action E_StateChanged;
        
        public T CurrentState  { get; private set; }
        public T PreviousState { get; private set; }

        private bool _invokeEvents;
        
        public void ChangeState(T newState)
        {
            if(newState.Equals(CurrentState))
            {
                return;
            }

            PreviousState = CurrentState;
            CurrentState  = newState;

            if (_invokeEvents)
            {
                E_StateChanged?.Invoke();
            }
        }

        public States(T initialState, bool invokeEvents)
        {
            PreviousState = initialState;
            CurrentState  = initialState;
            _invokeEvents = invokeEvents;
        }
    }
}