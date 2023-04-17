using System;
using System.Windows;
using System.Windows.Threading;

namespace NodeRedConfigurator
{
    public class CycleAction : EventArgs, IDisposable
    {
        /// <summary>
        /// Произошло изменение состояния включения циклического выполнения действия
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Произошел останов циклического выполнения действия
        /// </summary>
        public event EventHandler CycleStoped;

        /// <summary>
        /// Произошел запуск циклического выполнения действия
        /// </summary>
        public event EventHandler CycleStarted;

        public CycleState State = CycleState.IsStoped;

        private DispatcherTimer timer = new DispatcherTimer();

        public string actionName;
        private bool disposedValue;

        /// <summary>
        /// Циклическое действие
        /// </summary>
        /// <param name="action">метод</param>
        /// <param name="intervalValue">значение интервала</param>
        /// <param name="intervalType">тип интервала</param>
        /// <param name="actionName">имя метода</param>
        public CycleAction(Action action, int intervalValue, TimeIntervalType intervalType = TimeIntervalType.Milliseconds, string actionName = null)
        {
            this.actionName = actionName;
            Initialize(action, intervalValue, intervalType, actionName);
        }

        public void Start()
        {
            timer.Start();
            if (timer.IsEnabled) State = CycleState.IsStarted;
            else State = CycleState.IsStoped;
            StateChanged?.Invoke(this, EventArgs.Empty);
            CycleStarted?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            timer.Stop();
            if (timer.IsEnabled) State = CycleState.IsStarted;
            else State = CycleState.IsStoped;
            StateChanged?.Invoke(this, EventArgs.Empty);
            CycleStoped?.Invoke(this, EventArgs.Empty);
        }

        public void ResetTimer()
        {
            timer = new DispatcherTimer();
        }

        public CycleState GetState()
        {
            if (timer.IsEnabled) State = CycleState.IsStarted;
            else State = CycleState.IsStoped;
            return State;
        }

        private void Initialize(Action action, int intervalValue, TimeIntervalType intervalType, string actionName = null)
        {
            switch (intervalType)
            {
                case TimeIntervalType.Milliseconds:
                    timer.Interval = TimeSpan.FromMilliseconds(intervalValue);
                    break;

                case TimeIntervalType.Seconds:
                    timer.Interval = TimeSpan.FromSeconds(intervalValue);
                    break;

                case TimeIntervalType.Minutes:
                    timer.Interval = TimeSpan.FromMinutes(intervalValue);
                    break;

                case TimeIntervalType.Hours:
                    timer.Interval = TimeSpan.FromHours(intervalValue);
                    break;

                case TimeIntervalType.Days:
                    timer.Interval = TimeSpan.FromDays(intervalValue);
                    break;

                default:
                    timer.Interval = TimeSpan.FromMilliseconds(intervalValue);
                    break;
            }

            timer.Tick += (sender, args) =>
            {
                try
                {
                    action.DynamicInvoke(new object[] { });
                }
                catch (Exception e)
                {
                    string message = "";
                    if (actionName == null) message = $"{action.Method.Name}: {e.Message}";
                    else
                    {
                        message = $"{actionName}: {e.Message}";
                    }
                    MessageBox.Show(message);
                }
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                    if (timer != null)
                        timer.Tick -= (sender, args) => { };
                }
                timer = null;
                actionName = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public enum TimeIntervalType
    {
        Milliseconds,
        Seconds,
        Minutes,
        Hours,
        Days
    }

    public enum CycleState
    {
        IsStoped,
        IsStarted
    }
}