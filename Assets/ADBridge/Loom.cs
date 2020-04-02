using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ADBridge
{

    public class Loom : MonoBehaviour
    {
        private static Loom _instance;
        private static Loom instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("Loom").AddComponent<Loom>();
                }
                return _instance;
            }
        }

        private struct DelayedQueueItem
        {
            public float time;
            public Action action;
        }

        private readonly List<Action> _actions = new List<Action>();

        private readonly List<Action> _currentActions = new List<Action>();

        private readonly List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

        private readonly List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

        public static void QueueOnMainThread(Action action, float time = 0f)
        {
            if (Math.Abs(time) > 0.001f)
            {
                lock (instance._delayed)
                {
                    instance._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                }
            }
            else
            {
                lock (instance._actions)
                {
                    instance._actions.Add(action);
                }
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        private void Update()
        {
            lock (_actions)
            {
                _currentActions.Clear();
                _currentActions.AddRange(_actions);
                _actions.Clear();
            }
            foreach (Action action in _currentActions)
            {
                action?.Invoke();
            }
            lock (_delayed)
            {
                _currentDelayed.Clear();
                _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
                foreach (DelayedQueueItem item in _currentDelayed)
                {
                    _delayed.Remove(item);
                }
            }
            foreach (DelayedQueueItem delayed in _currentDelayed)
            {
                delayed.action?.Invoke();
            }
        }
        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
