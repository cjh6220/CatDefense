using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] BoxCollider2D collider;
    [SerializeField] Transform[] UnitPosition;
    List<GameObject> UnitList = new List<GameObject>();
    float startPosx;
    float startPosY;
    bool isBeingHeld = false;

    private async void Start() 
    {
        var ran = Random.Range(1, UnitPosition.Length);
        for (int i = 0; i < ran; i++)
        {
            await AddUnit();
        }    
    }

    public async UniTask AddUnit()
    {
        var newUnit = await ResourcePoolManager.GetAsync<UnitBase>("Unit_Base", true, UnitPosition[UnitList.Count]);
        newUnit.transform.localPosition = Vector3.zero;
        newUnit.transform.localRotation = Quaternion.identity;
        newUnit.transform.localScale = Vector3.one;
        newUnit.SetUnit("Unit_1", UnitList.Count);
        UnitList.Add(newUnit.gameObject);

        SetColliderSize(UnitList.Count);
    }

    private void Update()
    {
        if (isBeingHeld)
        {
            Vector2 mousePos;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            gameObject.transform.position = new Vector2(mousePos.x - startPosx, gameObject.transform.position.y);
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPosx = mousePos.x - this.transform.position.x;
            startPosY = mousePos.y - this.transform.position.y;
            isBeingHeld = true;
        }
    }

    private void OnMouseUp()
    {
        isBeingHeld = false;
    }

    public void SetColliderSize(int unitCount)
    {
        float sizeX = 0f;
        float sizeY = 0f;
        float offsetY = 0f;
        switch (unitCount)
        {
            case 1:
                {
                    sizeX = 1f;
                    sizeY = 1f;
                    offsetY = 0f;
                }
                break;
            case 2:
            case 3:
                {
                    sizeX = 3f;
                    sizeY = 1f;
                    offsetY = 0f;
                }
                break;
            case 4:
            case 5:
                {
                    sizeX = 3f;
                    sizeY = 2f;
                    offsetY = 0.5f;
                }
                break;
            default:
                {
                    sizeX = 5f;
                    sizeY = 2f;
                    offsetY = 0.5f;
                }
                break;
        }
        collider.size = new Vector2(sizeX, sizeY);
        collider.offset = new Vector2(0, offsetY);
    }
}
