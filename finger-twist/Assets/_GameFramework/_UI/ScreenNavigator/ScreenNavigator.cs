using System;
using System.Collections.Generic;

namespace GameFramework.UI
{
    public sealed class ScreenNavigator : IScreenNavigator, IDisposable
    {
        public event Action OnHistoryPanelsHasBeenEnded;
      
        private readonly Stack<IScreen> _history = new Stack<IScreen>(8);

        private IScreen _current;

        public IScreen Previous => _history.Count > 0 ? _history.Peek() : null;
        public IScreen Current => _current;

        public void NavigateTo(IScreen screen, bool saveToHistory)
        {
            _current?.Hide();

            if (saveToHistory)
                _history.Push(_current);

            _current = screen;
            _current.Show();
        }

        public void NavigateBack()
        {
            if (_history.Count == 0)
            {
                OnHistoryPanelsHasBeenEnded?.Invoke();
                return;
            }

            if (_history.Count <= 0)
                return;

            _current?.Hide();
            _current = _history.Pop();
            _current.Show();
        }

        public void ClearHistory()
            => _history.Clear();

        public void Dispose()
        {
            _history.Clear();
            _current = null;
        }
    }
}
