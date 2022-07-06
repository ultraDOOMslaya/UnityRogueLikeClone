using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;


public class Globals
{
    //for raycasting phantom buildings
    public static int TERRAIN_LAYER_MASK = 1 << 6;
    public static int UNIT_MASK = 1 << 7;
    public static int RESOURCE_MASK = 1 << 8;
    public static BuildingData[] BUILDING_DATA;
    public static ResourceData[] RESOURCE_DATA;
    public static NavMeshSurface NAV_MESH_SURFACE;
    //public static BuildingData[] BUILDING_DATA = new BuildingData[]
    //{
    //    new BuildingData("Hovel", 100, new Dictionary<string, int>()
    //    {
    //        { "gold", 100 },
    //        { "wood", 120 }
    //    }),
    //    new BuildingData("Tower", 100, new Dictionary<string, int>()
    //    {
    //        { "gold", 80 },
    //        { "wood", 80 },
    //        { "stone", 100 }
    //    })
    //};


    public static Dictionary<string, GameResource> GAME_RESOURCES = new Dictionary<string, GameResource>()
    {
        { "gold", new GameResource("Gold", 0) },
        { "wood", new GameResource("Wood", 0) },
        { "stone", new GameResource("Stone", 0) }
    };

    public static List<UnitManager> SELECTED_UNITS = new List<UnitManager>();
    public static List<HarvestResourceManager> SELECTED_RESOURCES = new List<HarvestResourceManager>();

    public static void UpdateNavMeshSurface()
    {
        NAV_MESH_SURFACE.UpdateNavMesh(NAV_MESH_SURFACE.navMeshData);
    }
}
