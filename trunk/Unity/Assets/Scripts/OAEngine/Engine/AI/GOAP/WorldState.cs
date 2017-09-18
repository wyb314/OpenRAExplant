using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.AI.GOAP.Enums;

namespace Engine.AI.GOAP
{
    public class Value
    {

    }


    public class ValueVector : Value
    {
        public UnityEngine.Vector3 Vector;

        public ValueVector(UnityEngine.Vector3 vector) { Vector = vector; }

        public override string ToString() { return Vector.ToString(); }
    }

    public class ValueAgent : Value
    {
        public Agent Agent;

        public ValueAgent(Agent a) { Agent = a; }

        public override string ToString() { return Agent.name; }
    }

    public class ValueBool : Value
    {
        public bool Bool;

        public ValueBool(bool b) { Bool = b; }

        public override string ToString() { return Bool.ToString(); }
    }

    public class ValueFloat : Value
    {
        public float Float;

        public ValueFloat(float f) { Float = f; }

        public override string ToString() { return Float.ToString(); }
    }

    public class ValueInt : Value
    {
        public int Int;

        public ValueInt(int i) { Int = i; }

        public override string ToString() { return Int.ToString(); }
    }

    public class ValueEvent : Value
    {
        public E_EventTypes Event;

        public ValueEvent(E_EventTypes eventType) { Event = eventType; }

        public override string ToString() { return Event.ToString(); }
    }

    public class ValueOrder : Value
    {
        public E_OrderType Order;

        public ValueOrder(E_OrderType order) { Order = order; }

        public override string ToString() { return Order.ToString(); }
    }
    public class WorldState
    {
    }
}
