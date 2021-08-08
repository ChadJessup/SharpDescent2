using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDescent2.Core.DataStructures;

namespace SharpDescent2.Core
{
    public static class VectorMath
    {
        //macro to set a vector to zero.  we could do this with an in-line assembly 
        //macro, but it's probably better to let the compiler optimize it.
        //Note: NO RETURN VALUE
        public static void vm_vec_zero(vms_vector v)
        {
            v.x = 0;
            v.y = 0;
            v.z = 0;
        }

        public static void vm_angvec_zero(vms_angvec v)
        {
            v.p = 0;
            v.b = 0;
            v.h = 0;
        }

    }
}
