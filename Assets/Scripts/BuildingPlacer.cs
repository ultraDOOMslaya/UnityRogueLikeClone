using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer instance;
    public Building prevPlacedBuilding = null;
    public BuildingData initialBuilding; // <-- temp data
    public ResourceData initialResource; 

    private Building _placedBuilding = null;
    private Ray _ray;
    private RaycastHit _raycastHit;
    private Vector3 _lastPlacementPosition;
    private UIManager _uiManager;

    private void Start()
    {
        instance = this;
        SpawnBuilding(
            initialBuilding,
            1,
            new Vector3(100, 0, 100)
        );
        SpawnBuilding(
            initialBuilding,
            0,
            new Vector3(100, 0, 200)
        );
        SpawnResource(
            initialResource,
            new Vector3(50, 0, 150)
        );
    }

    private void Awake()
    {
        _uiManager = GetComponent<UIManager>();
    }

    void Update()
    {
        if (_placedBuilding != null)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _CancelPlacedBuilding();
                return;
            }

            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(
                _ray,
                out _raycastHit,
                1000f,
                Globals.TERRAIN_LAYER_MASK
            ))
            {
                _placedBuilding.SetPosition(_raycastHit.point);
                if (_lastPlacementPosition != _raycastHit.point)
                {
                    _placedBuilding.CheckValidPlacement();
                }
                _lastPlacementPosition = _raycastHit.point;
            }

            if (_placedBuilding.HasValidPlacement && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                _PlaceBuilding();
            }
        }
    }

    void _PreparePlacedBuilding(int buildingDataIndex)
    {
        _PreparePlacedBuilding(Globals.BUILDING_DATA[buildingDataIndex]);
    }

    void _PreparePlacedBuilding(BuildingData buildingData)
    {
        // destroy the previous "phantom" if there is one
        if (_placedBuilding != null && !_placedBuilding.IsFixed)
        {
            Destroy(_placedBuilding.Transform.gameObject);
        }
        Building building = new Building(
            buildingData,
            GameManager.instance.gamePlayersParameters.myPlayerId
        );
        _placedBuilding = building;
        _lastPlacementPosition = Vector3.zero;
        EventManager.TriggerEvent("PlaceBuildingOn");
    }

    void _CancelPlacedBuilding()
    {
        // destroy the "phantom" building
        Destroy(_placedBuilding._transform.gameObject);
        _placedBuilding = null;
    }

    void _PlaceBuilding(bool canChain = true)
    {
        _placedBuilding.Place();
        if (canChain)
        {
            if (_placedBuilding.CanBuy())
                _PreparePlacedBuilding(_placedBuilding.DataIndex);
            else
            {
                EventManager.TriggerEvent("PlaceBuildingOff");
                _placedBuilding = null;
            }
        }

        if (_placedBuilding.CanBuy())
            _PreparePlacedBuilding(_placedBuilding.DataIndex);
        else
        {
            EventManager.TriggerEvent("PlaceBuildingOff");
            _placedBuilding = null;
        }
        EventManager.TriggerEvent("UpdateResourceTexts");
        EventManager.TriggerEvent("CheckBuildingButtons");

        // update the dynamic nav mesh
        Globals.UpdateNavMeshSurface();
    }

    void PlaceResource()
    {

    }

    public void SelectPlacedBuilding(int buildingDataIndex)
    {
        _PreparePlacedBuilding(buildingDataIndex);
    }
    public void SelectPlacedBuilding(BuildingData buildingData)
    {
        _PreparePlacedBuilding(buildingData);
    }

    public void SpawnBuilding(BuildingData data, int owner, Vector3 position)
    {
        SpawnBuilding(data, owner, position, new List<ResourceValue>() { });
    }
    public void SpawnBuilding(BuildingData data, int owner, Vector3 position, List<ResourceValue> production)
    {
        // keep a reference to the previously placed building, if there is one
        Building prevPlacedBuilding = _placedBuilding;

        // instantiate building
        _placedBuilding = new Building(data, owner);
        _placedBuilding.SetPosition(position);

        // ====> (we remove the Initialize() call)

        // finish up the placement
        _PlaceBuilding();
        // make sure we get rid of the placed building placeholder
        _CancelPlacedBuilding();

        // restore the previous state
        _placedBuilding = prevPlacedBuilding;
    }
    public void SpawnResource(ResourceData data, Vector3 position)
    {
        var resource = new Resource(data);
        resource.SetPosition(position);
    }
}
