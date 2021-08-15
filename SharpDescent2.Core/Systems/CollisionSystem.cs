using Microsoft.Extensions.Logging;
using Sharp.Platform.Interfaces;

namespace SharpDescent2.Core.Systems;

public class CollisionSystem : IGamePlatformManager
{
    private readonly ILogger<CollisionSystem> logger;

    public CollisionSystem(ILogger<CollisionSystem> logger)
    {
        this.logger = logger;
    }

    public bool IsInitialized { get; }

    public RESULT[,] CollisionResult { get; } = new RESULT[MAX.OBJECT_TYPES, MAX.OBJECT_TYPES];

    public ValueTask<bool> Initialize()
    {
        for (int i = 0; i < MAX.OBJECT_TYPES; i++)
        {
            for (int j = 0; j < MAX.OBJECT_TYPES; j++)
            {
                this.CollisionResult[i, j] = RESULT.NOTHING;
            }
        }

        this.ENABLE_COLLISION(OBJ.WALL, OBJ.ROBOT);
        this.ENABLE_COLLISION(OBJ.WALL, OBJ.WEAPON);
        this.ENABLE_COLLISION(OBJ.WALL, OBJ.PLAYER);
        this.DISABLE_COLLISION(OBJ.FIREBALL, OBJ.FIREBALL);

        this.ENABLE_COLLISION(OBJ.ROBOT, OBJ.ROBOT);
        //	DISABLE_COLLISION( OBJ.ROBOT, OBJ.ROBOT );	//	ALERT: WARNING: HACK: MK = RESPONSIBLE! TESTING!!

        this.DISABLE_COLLISION(OBJ.HOSTAGE, OBJ.HOSTAGE);
        this.ENABLE_COLLISION(OBJ.PLAYER, OBJ.PLAYER);
        this.ENABLE_COLLISION(OBJ.WEAPON, OBJ.WEAPON);
        this.DISABLE_COLLISION(OBJ.CAMERA, OBJ.CAMERA);
        this.DISABLE_COLLISION(OBJ.POWERUP, OBJ.POWERUP);
        this.DISABLE_COLLISION(OBJ.DEBRIS, OBJ.DEBRIS);
        this.DISABLE_COLLISION(OBJ.FIREBALL, OBJ.ROBOT);
        this.DISABLE_COLLISION(OBJ.FIREBALL, OBJ.HOSTAGE);
        this.DISABLE_COLLISION(OBJ.FIREBALL, OBJ.PLAYER);
        this.DISABLE_COLLISION(OBJ.FIREBALL, OBJ.WEAPON);
        this.DISABLE_COLLISION(OBJ.FIREBALL, OBJ.CAMERA);
        this.DISABLE_COLLISION(OBJ.FIREBALL, OBJ.POWERUP);
        this.DISABLE_COLLISION(OBJ.FIREBALL, OBJ.DEBRIS);
        this.DISABLE_COLLISION(OBJ.ROBOT, OBJ.HOSTAGE);
        this.ENABLE_COLLISION(OBJ.ROBOT, OBJ.PLAYER);
        this.ENABLE_COLLISION(OBJ.ROBOT, OBJ.WEAPON);
        this.DISABLE_COLLISION(OBJ.ROBOT, OBJ.CAMERA);
        this.DISABLE_COLLISION(OBJ.ROBOT, OBJ.POWERUP);
        this.DISABLE_COLLISION(OBJ.ROBOT, OBJ.DEBRIS);
        this.ENABLE_COLLISION(OBJ.HOSTAGE, OBJ.PLAYER);
        this.ENABLE_COLLISION(OBJ.HOSTAGE, OBJ.WEAPON);
        this.DISABLE_COLLISION(OBJ.HOSTAGE, OBJ.CAMERA);
        this.DISABLE_COLLISION(OBJ.HOSTAGE, OBJ.POWERUP);
        this.DISABLE_COLLISION(OBJ.HOSTAGE, OBJ.DEBRIS);
        this.ENABLE_COLLISION(OBJ.PLAYER, OBJ.WEAPON);
        this.DISABLE_COLLISION(OBJ.PLAYER, OBJ.CAMERA);
        this.ENABLE_COLLISION(OBJ.PLAYER, OBJ.POWERUP);
        this.DISABLE_COLLISION(OBJ.PLAYER, OBJ.DEBRIS);
        this.DISABLE_COLLISION(OBJ.WEAPON, OBJ.CAMERA);
        this.DISABLE_COLLISION(OBJ.WEAPON, OBJ.POWERUP);
        this.ENABLE_COLLISION(OBJ.WEAPON, OBJ.DEBRIS);
        this.DISABLE_COLLISION(OBJ.CAMERA, OBJ.POWERUP);
        this.DISABLE_COLLISION(OBJ.CAMERA, OBJ.DEBRIS);
        this.DISABLE_COLLISION(OBJ.POWERUP, OBJ.DEBRIS);
        this.ENABLE_COLLISION(OBJ.POWERUP, OBJ.WALL);
        this.ENABLE_COLLISION(OBJ.WEAPON, OBJ.CNTRLCEN);
        this.ENABLE_COLLISION(OBJ.WEAPON, OBJ.CLUTTER);
        this.ENABLE_COLLISION(OBJ.PLAYER, OBJ.CNTRLCEN);
        this.ENABLE_COLLISION(OBJ.ROBOT, OBJ.CNTRLCEN);
        this.ENABLE_COLLISION(OBJ.PLAYER, OBJ.CLUTTER);
        this.ENABLE_COLLISION(OBJ.PLAYER, OBJ.MARKER);

        return ValueTask.FromResult(true);
    }

    private void DISABLE_COLLISION(OBJ type1, OBJ type2)
        => this.CollisionResult[(int)type1, (int)type2] = RESULT.NOTHING;

    private void ENABLE_COLLISION(OBJ type1, OBJ type2)
        => this.CollisionResult[(int)type1, (int)type2] = RESULT.CHECK;

    public void Dispose()
    {
    }
}

public enum RESULT
{
    //Result types
    NOTHING = 0, //Ignore this collision
    CHECK = 1, //Check for this collision
}
