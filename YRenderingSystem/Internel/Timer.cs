using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YRenderingSystem
{
    public class Timer : IDisposable
    {
        public Timer(Action callBack)
        {
            _isfirst = false;
            _isRunning = false;
            _isDisposed = false;
            _callBack = callBack;
            _stopwatch = new Stopwatch();
            _resetEvent = new AutoResetEvent(false);
        }

        private Action _callBack;
        private Thread _thread;
        private int _dueTime;
        private int _period;
        private bool _isRunning;
        private bool _isfirst;
        private bool _isDisposed;
        private long _lastTick;
        private Stopwatch _stopwatch;
        private AutoResetEvent _resetEvent;

        private void _InitThread()
        {
            _thread = new Thread(_ThreadLoop);
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.IsBackground = true;
            _thread.Priority = ThreadPriority.Normal;
        }

        private void _DisposeThread()
        {
            if (_thread == null) return;
            try
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
            catch (Exception)
            {
            }
            finally
            {
                _thread = null;
            }
        }

        private void _ThreadLoop()
        {
            while (_isRunning)
            {
                if (_isfirst)
                {
                    _isfirst = false;
                    if (_dueTime > 0)
                        Thread.Sleep(_dueTime);
                    if (_dueTime < 0)
                        continue;
                    _callBack();
                    continue;
                }
                var now = _stopwatch.ElapsedMilliseconds;
                if (_period < 0 || now - _lastTick < _period)
                {
                    if (_period < 0)
                        _resetEvent.WaitOne();
                    continue;
                }
                else
                {
                    _lastTick = now;
                    _callBack();
                }
            }
        }

        public void Start(int dueTime, int period)
        {
            if (_isDisposed || _isRunning) return;

            _dueTime = dueTime;
            _period = period;
            _DisposeThread();
            _InitThread();
            _isfirst = true;
            _isRunning = true;
            _stopwatch.Start();
            _thread.Start();
        }

        public void Change(int dueTime, int period)
        {
            if (_isDisposed) return;

            if (!_isRunning)
                Start(Timeout.Infinite, Timeout.Infinite);

            while ((_thread.ThreadState & System.Threading.ThreadState.Unstarted) != 0)
                Thread.Sleep(1);

            _dueTime = dueTime;
            _period = period;

            _isfirst = true;
            _lastTick = 0;
            _resetEvent.Set();
            _stopwatch.Restart();
        }

        public void Stop()
        {
            if (_isDisposed) return;

            _isfirst = false;
            _isRunning = false;
            _stopwatch.Stop();
            _DisposeThread();
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _DisposeThread();
            _resetEvent.Dispose();
            _resetEvent = null;
            _stopwatch = null;
            _callBack = null;
        }
    }
}