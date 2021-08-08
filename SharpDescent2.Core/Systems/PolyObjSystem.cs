using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharp.Platform.Interfaces;
using SharpDescent2.Core.DataStructures;
using SixLabors.ImageSharp.PixelFormats;

namespace SharpDescent2.Core.Systems
{
    public class PolyObjSystem : IGamePlatformManager
    {
        public bool IsInitialized { get; }
        public polymodel[] models = new polymodel[MAX.POLYGON_MODELS];	// = {&bot11,&bot17,&robot_s2,&robot_b2,&bot11,&bot17,&robot_s2,&robot_b2};

        public ValueTask<bool> Initialize()
        {
            return ValueTask.FromResult(true);
        }

        public void Dispose()
        {
        }
    }

    public class polymodel
    {
        public int n_models;
        public int model_data_size;
        public byte[] model_data = new byte[1];
        public int model_data_ptr;
        public int[] submodel_ptrs = new int[MAX.SUBMODELS];
        public vms_vector[] submodel_offsets = new vms_vector[MAX.SUBMODELS];
        public vms_vector[] submodel_norms = new vms_vector[MAX.SUBMODELS];       //norm for sep plane
        public vms_vector[] submodel_pnts = new vms_vector[MAX.SUBMODELS];        //point on sep plane 
        public int[] submodel_rads = new int[MAX.SUBMODELS];               //radius for each submodel
        public byte[] submodel_parents = new byte[MAX.SUBMODELS];      //what is parent for each submodel
        public vms_vector[] submodel_mins = new vms_vector[MAX.SUBMODELS];
        public vms_vector[] submodel_maxs = new vms_vector[MAX.SUBMODELS];
        public vms_vector mins, maxs;                          //min,max for whole model
        public int rad;
        public byte n_textures;
        public ushort first_texture;
        public byte simpler_model;        //alternate model with less detail (0 if none, model_num+1 else)
                                          //	vms_vector min,max;
    }
}
