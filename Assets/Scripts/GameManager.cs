using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public GameGlobalParameters gameGlobalParameters;
    public GamePlayersParameters gamePlayersParameters;

    private Ray _ray;
    private RaycastHit _raycastHit;

    public void Start()
    {
        instance = this;
    }

    private void Awake()
    {
        DataHandler.LoadGameData();
        Globals.NAV_MESH_SURFACE = GameObject.Find("Terrain").GetComponent<NavMeshSurface>();
        Globals.UpdateNavMeshSurface();
    }

    private void Update()
    {
        //_CheckUnitsNavigation();
    }

    //TODO remove
    //private void _CheckUnitsNavigation()
    //{
    //    if (Globals.SELECTED_UNITS.Count > 0 && Input.GetMouseButtonUp(1))
    //    {
    //        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        if (Physics.Raycast(
    //            _ray,
    //            out _raycastHit,
    //            1000f,
    //            Globals.TERRAIN_LAYER_MASK
    //        ))
    //        {
    //            foreach (UnitManager um in Globals.SELECTED_UNITS)
    //                if (um.GetType() == typeof(CharacterManager))
    //                    ((CharacterManager)um).MoveTo(_raycastHit.point);
    //        }
    //    }
    //}
}
