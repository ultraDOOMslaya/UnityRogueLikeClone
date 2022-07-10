using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject selectionCircle;
    public int ownerMaterialSlotIndex = 0;
    public bool IsSelected { get => _selected; }
    public int SelectIndex { get => _selectIndex;  }

    private bool _hovered = false;
    private Transform _canvas;
    private GameObject _healthbar;
    private bool _selected = false;
    private int _selectIndex = -1;
    private Bounds _defaultBounds;

    protected BoxCollider _collider;
    protected Animator _animator;
    public virtual Unit Unit { get; set; }

    public void Initialize(Unit unit)
    {
        _collider = GetComponent<BoxCollider>();
        Unit = unit;
    }

    private void Awake()
    {
        _canvas = GameObject.Find("Canvas").transform;
        _defaultBounds = new Bounds(new Vector3(-0.08205697f, 0.01875179f, 8.98838e-05f),
                                    new Vector3(0.5657575f, 0.2172205f, 0.6085676f));
    }

    private void OnMouseEnter()
    {
        _hovered = true;
    }

    private void OnMouseExit()
    {
        _hovered = false;
    }

    private void OnMouseDown()
    {
        if (_hovered && Input.GetMouseButtonDown(0) && IsActive())
            Select(
                true,
                Input.GetKey(KeyCode.LeftShift) ||
                Input.GetKey(KeyCode.RightShift)
            );
    }

    private bool _IsMyUnit()
    {
        return Unit.Owner == GameManager.instance.gamePlayersParameters.myPlayerId;
    }

    private void _SelectUtil()
    {
        // abort if not active
        if (!IsActive()) return;
        // abort if already selected
        if (Globals.SELECTED_UNITS.Contains(this)) return;

        Globals.SELECTED_UNITS.Add(this);
        selectionCircle.SetActive(true);
        if (_healthbar == null)
        {
            _healthbar = GameObject.Instantiate(Resources.Load("Prefabs/UI/Healthbar")) as GameObject;
            _healthbar.transform.SetParent(_canvas);
            HealthBar h = _healthbar.GetComponent<HealthBar>();
            Rect boundingBox = Utils.GetBoundingBoxOnScreen(
                //transform.Find("Mesh").GetComponent<Renderer>().bounds,
                //_defaultBounds,
                _GetBounds(transform),
                Camera.main
            );
            h.Initialize(transform, boundingBox.height);
            h.SetPosition();
            _UpdateHealthbar();
        }
        EventManager.TriggerEvent("SelectUnit", Unit);
        _selected = true;
        _selectIndex = Globals.SELECTED_UNITS.Count - 1;
    }

    private static Bounds _GetBounds(Transform transform)
    {
        Bounds bounds = new Bounds();
        var renderers = transform.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            //Find first enabled renderer to start encapsulate from it
            foreach (Renderer renderer in renderers)
            {
                if (renderer.enabled)
                {
                    bounds = renderer.bounds;
                    break;
                }
            }
            //Encapsulate for all renderers
            foreach (Renderer renderer in renderers)
            {
                if (renderer.enabled)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
        }
        return bounds;
    }

    private void _Die()
    {
        if (_selected)
            Deselect();
        Destroy(gameObject);
    }

    public void Deselect()
    {
        if (!Globals.SELECTED_UNITS.Contains(this)) return;
        Globals.SELECTED_UNITS.Remove(this);

        selectionCircle.SetActive(false);
        Destroy(_healthbar);
        _healthbar = null;
        EventManager.TriggerEvent("DeselectUnit", Unit);
        _selected = false;
        _selectIndex = -1;
    }

    protected virtual bool IsActive()
    {
        return true;
    }

    public void Select() { Select(false, false); }
    public void Select(bool singleClick, bool holdingShift)
    {
        // basic case: using the selection box
        if (!singleClick)
        {
            _SelectUtil();
            return;
        }

        // single click: check for shift key
        if (!holdingShift)
        {
            List<UnitManager> selectedUnits = new List<UnitManager>(Globals.SELECTED_UNITS);
            foreach (UnitManager um in selectedUnits)
                um.Deselect();
            _SelectUtil();
        }
        else
        {
            if (!Globals.SELECTED_UNITS.Contains(this))
                _SelectUtil();
            else
                Deselect();
        }
    }

    public void SetOwnerMaterial(int owner)
    {
        Color playerColor;
        if (owner == 0)
        {
            playerColor = GameManager.instance.gamePlayersParameters.players[owner].color;
        }
        else
        {
            playerColor = new Color(240, 0, 0);
        }

        /* This will fail for the bought assets */
        //Material[] materials = transform.Find("Mesh").GetComponent<Renderer>().materials;
        
        //if (materials.Length > 2)
        //{
        //    materials[3].color = playerColor;
        //}
        //else
        //{
        //    materials[ownerMaterialSlotIndex].color = playerColor;
        //}
        //transform.Find("Mesh").GetComponent<Renderer>().materials = materials;
    }

    public void Attack(Transform target)
    {
        UnitManager um = target.GetComponent<UnitManager>();
        if (um == null) return;

        Vector3 deltaVec = target.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(deltaVec);
        transform.rotation = rotation;

        _animator.SetTrigger("Attack");
        um.TakeHit(Unit.Data.attackDamage);
    }

    public void Gather(Transform target)
    {
        HarvestResourceManager hrm = target.GetComponent<HarvestResourceManager>();
        if (hrm == null) return;

        Vector3 deltaVec = target.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(deltaVec);
        transform.rotation = rotation;

        _animator.SetTrigger("Attack");
<<<<<<< HEAD
        hrm.YieldResource(Unit.Data.harvestRate);

=======
>>>>>>> 7d1c822f120fa7fa5d5fe7ca40d064292e7907f9
    }

    public void TakeHit(int attackPoints)
    {
        Unit.HP -= attackPoints;
        _UpdateHealthbar();
        if (Unit.HP <= 0) _Die();
    }

    private void _UpdateHealthbar()
    {
        if (!_healthbar) return;
        Transform fill = _healthbar.transform.Find("Fill");
        fill.GetComponent<UnityEngine.UI.Image>().fillAmount = Unit.HP / (float)Unit.MaxHP;
    }
}