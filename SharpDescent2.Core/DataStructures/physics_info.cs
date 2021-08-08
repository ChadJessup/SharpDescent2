using System;

namespace SharpDescent2.Core.DataStructures
{
    //information for physics sim for an object
    public struct physics_info
    {
        public vms_vector velocity;        //velocity vector of this object
        public vms_vector thrust;          //constant force applied to this object
        public int mass;               //the mass of this object
        public int drag;               //how fast this slows down
        public int brakes;         //how much brakes applied
        public vms_vector rotvel;          //rotational velecity (angles)
        public vms_vector rotthrust;       //rotational acceleration
        public short turnroll;        //rotation caused by turn banking
        public PF flags;           //misc physics flags
    }

    [Flags]
    public enum PF
    {
        //physics flags
        TURNROLL = 0x01,// roll when turning
        LEVELLING = 0x02,   // level object with closest side
        BOUNCE = 0x04,  // bounce (not slide) when hit will
        WIGGLE = 0x08,  // wiggle while flying
        STICK = 0x10,   // object sticks (stops moving) when hits wall
        PERSISTENT = 0x20,// object keeps going even after it hits another object (eg, fusion cannon)
        USES_THRUST = 0x40,// this object uses its thrust
        BOUNCED_ONCE = 0x80,// Weapon has bounced once.
        FREE_SPINNING = 0x100,// Drag does not apply to rotation of this object
        BOUNCES_TWICE = 0x200,// This weapon bounces twice, then dies
    }
}
