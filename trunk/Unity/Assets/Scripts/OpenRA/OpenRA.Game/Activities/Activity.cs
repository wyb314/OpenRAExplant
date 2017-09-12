using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Traits;

namespace OpenRA.Activities
{
    public enum ActivityState { Queued, Active, Done, Canceled }

    public abstract class Activity
    {
        public ActivityState State { get; private set; }

        /// <summary>
        /// Returns the top-most activity *from the point of view of the calling activity*. Note that the root activity
        /// can and likely will have next activities of its own, which would in turn be the root for their children.
        /// </summary>
        public Activity RootActivity
        {
            get
            {
                var p = this;
                while (p.ParentActivity != null)
                    p = p.ParentActivity;

                return p;
            }
        }

        Activity parentActivity;
        public Activity ParentActivity
        {
            get
            {
                return parentActivity;
            }

            protected set
            {
                parentActivity = value;

                var next = NextInQueue;
                if (next != null)
                    next.ParentActivity = parentActivity;
            }
        }

        Activity childActivity;
        protected Activity ChildActivity
        {
            get
            {
                return childActivity != null && childActivity.State < ActivityState.Done ? childActivity : null;
            }

            set
            {
                if (value == this || value == ParentActivity || value == NextInQueue)
                    childActivity = null;
                else
                {
                    childActivity = value;

                    if (childActivity != null)
                        childActivity.ParentActivity = this;
                }
            }
        }

        Activity nextActivity;

        /// <summary>
        /// The getter will return either the next activity or, if there is none, the parent one.
        /// </summary>
        public virtual Activity NextActivity
        {
            get
            {
                return nextActivity != null ? nextActivity : ParentActivity;
            }

            set
            {
                if (value == this || value == ParentActivity || (value != null && value.ParentActivity == this))
                    nextActivity = null;
                else
                {
                    nextActivity = value;

                    if (nextActivity != null)
                        nextActivity.ParentActivity = ParentActivity;
                }
            }
        }

        /// <summary>
        /// The getter will return the next activity on the same level _only_, in contrast to NextActivity.
        /// Use this to check whether there are any follow-up activities queued.
        /// </summary>
        public Activity NextInQueue
        {
            get { return nextActivity; }
            set { NextActivity = value; }
        }

        public bool IsInterruptible { get; protected set; }
        public bool IsCanceled { get { return State == ActivityState.Canceled; } }

        public Activity()
        {
            IsInterruptible = true;
        }

        public Activity TickOuter(Actor self)
        {
            if (State == ActivityState.Done && Game.Settings.Debug.StrictActivityChecking)
                throw new InvalidOperationException("Actor {0} attempted to tick activity {1} after it had already completed.".F(self, this.GetType()));

            if (State == ActivityState.Queued)
            {
                OnFirstRun(self);
                State = ActivityState.Active;
            }

            var ret = Tick(self);
            if (ret == null || (ret != this && ret.ParentActivity != this))
            {
                // Make sure that the Parent's ChildActivity pointer is moved forwards as the child queue advances.
                // The Child's ParentActivity will be set automatically during assignment.
                if (ParentActivity != null && ParentActivity != ret)
                    ParentActivity.ChildActivity = ret;

                if (State != ActivityState.Canceled)
                    State = ActivityState.Done;

                OnLastRun(self);
            }

            return ret;
        }

        public abstract Activity Tick(Actor self);

        /// <summary>
        /// Runs once immediately before the first LogicTick() execution.
        /// </summary>
        protected virtual void OnFirstRun(Actor self) { }

        /// <summary>
        /// Runs once immediately after the last LogicTick() execution.
        /// </summary>
        protected virtual void OnLastRun(Actor self) { }

        public virtual bool Cancel(Actor self, bool keepQueue = false)
        {
            if (!IsInterruptible)
                return false;

            if (ChildActivity != null && !ChildActivity.Cancel(self))
                return false;

            if (!keepQueue)
                NextActivity = null;

            ChildActivity = null;
            State = ActivityState.Canceled;

            return true;
        }

        public virtual void Queue(Activity activity)
        {
            if (NextInQueue != null)
                NextInQueue.Queue(activity);
            else
                NextInQueue = activity;
        }

        public virtual void QueueChild(Activity activity)
        {
            if (ChildActivity != null)
                ChildActivity.Queue(activity);
            else
                ChildActivity = activity;
        }

        /// <summary>
        /// Prints the activity tree, starting from the root or optionally from a given origin.
        ///
        /// Call this method from any place that's called during a tick, such as the LogicTick() method itself or
        /// the Before(First|Last)Run() methods. The origin activity will be marked in the output.
        /// </summary>
        /// <param name="origin">Activity from which to start traversing, and which to mark. If null, mark the calling activity, and start traversal from the root.</param>
        /// <param name="level">Initial level of indentation.</param>
        protected void PrintActivityTree(Activity origin = null, int level = 0)
        {
            if (origin == null)
                RootActivity.PrintActivityTree(this);
            else
            {
                Console.Write(new string(' ', level * 2));
                if (origin == this)
                    Console.Write("*");

                Console.WriteLine(this.GetType().ToString().Split('.').Last());

                if (ChildActivity != null)
                    ChildActivity.PrintActivityTree(origin, level + 1);

                if (NextInQueue != null)
                    NextInQueue.PrintActivityTree(origin, level);
            }
        }

        public virtual IEnumerable<Target> GetTargets(Actor self)
        {
            yield break;
        }
    }

    public abstract class CompositeActivity : Activity
    {
        /// <summary>
        /// The getter will return the first non-null value of either child, next or parent activity, in that order, or ultimately null.
        /// </summary>
        public override Activity NextActivity
        {
            get
            {
                if (ChildActivity != null)
                    return ChildActivity;
                else if (NextInQueue != null)
                    return NextInQueue;
                else
                    return ParentActivity;
            }
        }
    }

    public static class ActivityExts
    {
        public static IEnumerable<Target> GetTargetQueue(this Actor self)
        {
            return self.CurrentActivity
                .Iterate(u => u.NextActivity)
                .TakeWhile(u => u != null)
                .SelectMany(u => u.GetTargets(self));
        }
    }


}
