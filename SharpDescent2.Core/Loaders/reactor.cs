using SharpDescent2.Core.DataStructures;

namespace SharpDescent2.Core.Loaders
{
    public class reactor
    {
        public int model_num;
        public int n_guns;
        public vms_vector[] gun_points = new vms_vector[MAX.CONTROLCEN_GUNS];
        public vms_vector[] gun_dirs = new vms_vector[MAX.CONTROLCEN_GUNS];
    }
}
