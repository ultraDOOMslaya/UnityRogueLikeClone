using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HarvestResourceManager : MonoBehaviour
{
    public GameObject selectionCircle;
    public ItemData itemdata;

    private bool _hovered = false;
    private bool _selected = false;
    private Transform _canvas;
    private GameObject _healthbar;

    public virtual Resource Resource { get; set; }

    public void Initialize(Resource resource)
    {
        Resource = resource;
    }

    private void Awake()
    {
        _canvas = GameObject.Find("Canvas").transform;
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
        if (_hovered && Input.GetMouseButtonDown(0))
            Select();
    }

    public void Select()
    {
        _SelectUtil();       
    }

    public void Deselect()
    {
        if (!Globals.SELECTED_RESOURCES.Contains(this)) return;
        Globals.SELECTED_RESOURCES.Remove(this);
        Destroy(_healthbar);
        _healthbar = null;
        _selected = false;
    }

    private void _SelectUtil()
    {
        if (Globals.SELECTED_RESOURCES.Contains(this)) return;

        Globals.SELECTED_RESOURCES.Add(this);
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
            _UpdateYieldbar();
        }
        _selected = true;
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

    private void _Deplete()
    {
        if (_selected)
            Deselect();
        Destroy(gameObject);
    }

    private void _UpdateYieldbar()
    {
        if (!_healthbar) return;
        Transform fill = _healthbar.transform.Find("Fill");
        fill.GetComponent<UnityEngine.UI.Image>().fillAmount =  Resource.Yield / (float)Resource.MaxYield;
    }

    public void YieldResource(int gatherRate)
    {
        Resource.Yield -= gatherRate;
        _UpdateYieldbar();
        if (Resource.Yield <= 0) _Deplete();
        InventoryManager.instance.Add(itemdata);
    }

}

