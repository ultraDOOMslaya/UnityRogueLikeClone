using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Transform buildingMenu;
    public GameObject buildingButtonPrefab;
    public Transform resourcesUIParent;
    public GameObject gameResourceDisplayPrefab;
    public Transform selectedUnitsListParent;
    public GameObject selectedUnitDisplayPrefab;
    public Transform selectionGroupsParent;
    public GameObject gameResourceCostPrefab;
    public GameObject selectedUnitMenu;
    public GameObject unitSkillButtonPrefab;
    public GameObject craftingMenu;
    public Transform craftingParent;
    public GameObject craftingOptionPrefab;

    private Unit _selectedUnit;
    private BuildingPlacer _buildingPlacer;
    private Dictionary<string, Text> _resourceTexts;
    private Dictionary<string, Button> _buildingButtons;
    private Dictionary<string, GameObject> _craftingOptions;
    private RectTransform _selectedUnitContentRectTransform;
    private RectTransform _selectedUnitButtonsRectTransform;
    private Text _selectedUnitTitleText;
    private Text _selectedUnitLevelText;
    private Transform _selectedUnitResourcesProductionParent;
    private Transform _selectedUnitActionButtonsParent;

    private void Awake()
    {
        // create texts for each in-game resource (gold, wood, stone...)
        _resourceTexts = new Dictionary<string, Text>();
        foreach (KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            GameObject display = Instantiate(gameResourceDisplayPrefab);
            display.name = pair.Key;
            _resourceTexts[pair.Key] = display.transform.Find("Text").GetComponent<Text>();
            _SetResourceText(pair.Key, pair.Value.Amount);
            display.transform.SetParent(resourcesUIParent);
        }

        _buildingPlacer = GetComponent<BuildingPlacer>();

        // create buttons for each building type
        _buildingButtons = new Dictionary<string, Button>();
        for (int i = 0; i < Globals.BUILDING_DATA.Length; i++)
        {
            BuildingData data = Globals.BUILDING_DATA[i];
            GameObject button = Instantiate(buildingButtonPrefab);
            button.name = data.unitName;

            button.transform.Find("Text").GetComponent<Text>().text = data.unitName;

            
            Button b = button.GetComponent<Button>();
            _AddBuildingButtonListener(b, i);
            _buildingButtons[data.code] = b;
            button.transform.SetParent(buildingMenu);
        }

        for (int i = 1; i <= 9; i++)
            ToggleSelectionGroupButton(i, false);



        Transform selectedUnitMenuTransform = selectedUnitMenu.transform;
        _selectedUnitContentRectTransform = selectedUnitMenuTransform.Find("Content").GetComponent<RectTransform>();
        _selectedUnitButtonsRectTransform = selectedUnitMenuTransform.Find("Buttons").GetComponent<RectTransform>();
        _selectedUnitTitleText = selectedUnitMenuTransform.Find("Content/Title").GetComponent<Text>();
        _selectedUnitTitleText = selectedUnitMenuTransform.Find("Content/Title").GetComponent<Text>();
        //_selectedUnitLevelText = selectedUnitMenuTransform.Find("Content/Level").GetComponent<Text>();
        //_selectedUnitResourcesProductionParent = selectedUnitMenuTransform.Find("Content/ResourcesProduction");
        //_selectedUnitActionButtonsParent = selectedUnitMenuTransform.Find("SpecificActions");
        _selectedUnitActionButtonsParent = selectedUnitMenuTransform.Find("Buttons");

        _ShowSelectedUnitMenu(false);
    }

    private void _SetResourceText(string resource, int value)
    {
        _resourceTexts[resource].text = value.ToString();
    }

    private void OnEnable()
    {
        EventManager.AddListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.AddListener("CheckBuildingButtons", _OnCheckBuildingButtons);
        EventManager.AddListener("HoverBuildingButton", _OnHoverBuildingButton);
        EventManager.AddListener("UnhoverBuildingButton", _OnUnhoverBuildingButton);
        EventManager.AddListener("SelectUnit", _OnSelectUnit);
        EventManager.AddListener("DeselectUnit", _OnDeselectUnit);
        EventManager.AddListener("SelectBuilding", _OnSelectBuilding);
        EventManager.AddListener("DeselectBuilding", _OnDeselectBuilding);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.RemoveListener("CheckBuildingButtons", _OnCheckBuildingButtons);
        EventManager.RemoveListener("HoverBuildingButton", _OnHoverBuildingButton);
        EventManager.RemoveListener("UnhoverBuildingButton", _OnUnhoverBuildingButton);
        EventManager.RemoveListener("SelectUnit", _OnSelectUnit);
        EventManager.RemoveListener("DeselectUnit", _OnDeselectUnit);
        EventManager.RemoveListener("SelectBuilding", _OnSelectBuilding);
        EventManager.RemoveListener("DeselectBuilding", _OnDeselectBuilding);
    }

    private void _OnUpdateResourceTexts()
    {
        // not doing resources yet
        //foreach (KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        //    SetResourceText(pair.Key, pair.Value.Amount);
    }

    private void _OnCheckBuildingButtons()
    {
        foreach (BuildingData data in Globals.BUILDING_DATA)
            _buildingButtons[data.code].interactable = data.CanBuy();
    }

    private void _OnHoverBuildingButton(object data)
    {
        //SetInfoPanel((UnitData)data);
        //ShowInfoPanel(true);
    }

    private void _OnUnhoverBuildingButton()
    {
        //ShowInfoPanel(false);
    }

    private void _OnSelectUnit(object data)
    {
        Unit unit = (Unit)data;
        AddSelectedUnitToUIList(unit);
        _SetSelectedUnitMenu(unit);
        _ShowSelectedUnitMenu(true);

        

        if (unit.GetType() == typeof(Building))
        {
            _OnSelectBuilding(data);
        }
    }

    private void _OnDeselectUnit(object data)
    {
        Unit unit = (Unit)data;
        RemoveSelectedUnitFromUIList(unit.Code);
        if (Globals.SELECTED_UNITS.Count == 0)
            _ShowSelectedUnitMenu(false);
        else
            _SetSelectedUnitMenu(Globals.SELECTED_UNITS[Globals.SELECTED_UNITS.Count - 1].Unit);

        if (unit.GetType() == typeof(Building))
        {
            _OnDeselectBuilding();
        }
    }

    private void _OnSelectBuilding(object data)
    {
        Building building = (Building)data;
        _showSelectedBuildingMenu(building);
    }

    private void _OnDeselectBuilding()
    {
        _hideSelectedBuildingMenu();
    }

    private void _SetSelectedUnitMenu(Unit unit)
    {
        _selectedUnit = unit;
        bool unitIsMine = unit.Owner == GameManager.instance.gamePlayersParameters.myPlayerId;
        // adapt content panel heights to match info to display
        //int contentHeight = 60 + unit.Production.Count * 16;
        int contentHeight = 60;
        _selectedUnitContentRectTransform.sizeDelta = new Vector2(64, contentHeight);
        _selectedUnitButtonsRectTransform.anchoredPosition = new Vector2(0, -contentHeight - 20);
        _selectedUnitButtonsRectTransform.sizeDelta = new Vector2(70, Screen.height - contentHeight - 20);
        // update texts
        _selectedUnitTitleText.text = unit.Data.unitName;

        //TODO not doing anything w/ resources yet
        //_selectedUnitLevelText.text = $"Level NOT YET IMPLEMENTED";
        // clear resource production and reinstantiate new one
        //foreach (Transform child in _selectedUnitResourcesProductionParent)
        //    Destroy(child.gameObject);
        //if (unit.Production.Count > 0)
        //{
        //    GameObject g; Transform t;
        //    foreach (ResourceValue resource in unit.Production)
        //    {
        //        g = Instantiate(gameResourceCostPrefab) as GameObject;
        //        t = g.transform;
        //        t.Find("Text").GetComponent<Text>().text = $"+{resource.amount}";
        //        t.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/GameResources/{resource.code}");
        //        t.SetParent(_selectedUnitResourcesProductionParent);
        //    }
        //}

        // clear skills and reinstantiate new ones
        
        foreach (Transform child in _selectedUnitActionButtonsParent)
            Destroy(child.gameObject);
        if (unit.SkillManagers.Count > 0)
        {
            GameObject g; Transform t; Button b;
            for (int i = 0; i < unit.SkillManagers.Count; i++)
            {
                g = Instantiate(unitSkillButtonPrefab) as GameObject;
                t = g.transform;
                b = g.GetComponent<Button>();
                unit.SkillManagers[i].SetButton(b);
                t.Find("Text").GetComponent<Text>().text = unit.SkillManagers[i].skill.skillName;
                t.SetParent(_selectedUnitActionButtonsParent);
                _AddUnitSkillButtonListener(b, i);
            }
        }
    }

    private void _AddUnitSkillButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => _selectedUnit.TriggerSkill(i));
    }

    private void _ShowSelectedUnitMenu(bool show)
    {
        selectedUnitMenu.SetActive(show);
    }

    private void _showSelectedBuildingMenu(Building building)
    {
        craftingMenu.SetActive(true);

        _craftingOptions = new Dictionary<string, GameObject>();
        for (int i = 0; i < building.Data.skills.Count; i++)
        {
            SkillData data = building.Data.skills[i];
            GameObject craftingOption = Instantiate(craftingOptionPrefab);

            craftingOption.transform.Find("CraftInfo").transform.Find("Text").GetComponent<Text>().text = data.name;
            craftingOption.transform.Find("Icon").GetComponent<Image>().sprite = data.sprite;

            craftingOption.transform.SetParent(craftingParent);
        }
    }

    private void _hideSelectedBuildingMenu()
    {
        craftingMenu.SetActive(false);

        foreach (Transform child in craftingParent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void AddSelectedUnitToUIList(Unit unit)
    {
        // if there is another unit of the same type already selected,
        // increase the counter
        Transform alreadyInstantiatedChild = selectedUnitsListParent.Find(unit.Code);
        if (alreadyInstantiatedChild != null)
        {
            Text t = alreadyInstantiatedChild.Find("Count").GetComponent<Text>();
            int count = int.Parse(t.text);
            t.text = (count + 1).ToString();
        }
        // else create a brand new counter initialized with a count of 1
        else
        {
            GameObject g = Instantiate(selectedUnitDisplayPrefab);
            g.name = unit.Code;
            Transform t = g.transform;
            t.Find("Count").GetComponent<Text>().text = "1";
            t.Find("Name").GetComponent<Text>().text = unit.Data.unitName;
            t.SetParent(selectedUnitsListParent, false);
        }
    }

    public void RemoveSelectedUnitFromUIList(string code)
    {
        Transform listItem = selectedUnitsListParent.Find(code);
        if (listItem == null) return;
        Text t = listItem.Find("Count").GetComponent<Text>();
        int count = int.Parse(t.text);
        count -= 1;
        if (count == 0)
            DestroyImmediate(listItem.gameObject);
        else
            t.text = count.ToString();
    }

    public void UpdateResourceTexts()
    {
        foreach (KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            _SetResourceText(pair.Key, pair.Value.Amount);
        }
    }

    private void _AddBuildingButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => _buildingPlacer.SelectPlacedBuilding(i));
    }

    public void ToggleSelectionGroupButton(int groupIndex, bool on)
    {
        selectionGroupsParent.Find(groupIndex.ToString()).gameObject.SetActive(on);
    }
}