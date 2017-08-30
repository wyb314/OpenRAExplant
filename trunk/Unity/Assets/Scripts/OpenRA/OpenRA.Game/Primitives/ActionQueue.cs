using System;
using System.Collections.Generic;

namespace OpenRA.Primitives
{
    public class ActionQueue
    {
        readonly List<DelayedAction> actions = new List<DelayedAction>();

        public void Add(Action a, long desiredTime)
        {
            if (a == null)
                throw new ArgumentNullException("a");

            lock (actions)
            {
                var action = new DelayedAction(a, desiredTime);
                var index = Index(action);
                actions.Insert(index, action);
            }
        }

        public void PerformActions(long currentTime)
        {
            DelayedAction[] pendingActions;
            lock (actions)
            {
                var dummyAction = new DelayedAction(null, currentTime);
                var index = Index(dummyAction);
                if (index <= 0)
                    return;

                pendingActions = new DelayedAction[index];
                actions.CopyTo(0, pendingActions, 0, index);
                actions.RemoveRange(0, index);
            }

            foreach (var delayedAction in pendingActions)
                delayedAction.Action();
        }

        int Index(DelayedAction action)
        {
            // Returns the index of the next action with a strictly greater time.
            var index = actions.BinarySearch(action);
            if (index < 0)
                return ~index;
            while (index < actions.Count && action.CompareTo(actions[index]) >= 0)
                index++;
            return index;
        }
    }

    struct DelayedAction : IComparable<DelayedAction>
    {
        public readonly long Time;
        public readonly Action Action;

        public DelayedAction(Action action, long time)
        {
            Action = action;
            Time = time;
        }

        public int CompareTo(DelayedAction other)
        {
            return Time.CompareTo(other.Time);
        }

        public override string ToString()
        {
            return "Time: " + Time + " Action: " + Action;
        }
    }
}
