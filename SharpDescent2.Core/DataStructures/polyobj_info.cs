namespace SharpDescent2.Core.DataStructures;

//structures for different kinds of rendering

public class polyobj_info
{
    public int model_num;                      //which polygon model
    public vms_angvec[] anim_angles = new vms_angvec[MAX.SUBMODELS];  //angles for each subobject
    public int subobj_flags;                   //specify which subobjs to draw
    public int tmap_override;                  //if this is not -1, map all face to this
    public int alt_textures;                   //if not -1, use these textures instead
}
