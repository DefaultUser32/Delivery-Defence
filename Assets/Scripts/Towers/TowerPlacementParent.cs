/* 1. description: script for logic of tower placement
 * 2. @author: Linden/Matthew
 * 3. @date: 30-12-24
 * 4. @version: 1.0
 */
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    public Tilemap placementTilemap;
    public GameObject towerPrefab;
    public LayerMask towerLayer;
    public float cost;

    protected UIStateManager stateManager;
    [SerializeField] GameObject towerPrePrefab;
    public Camera mainCam;
    public bool isPlacementMode;
    protected GameObject towerPre;
    Renderer rend;

    // desc: base unity function called on start
    // pre: none
    // post: none
    protected virtual void Start(){
        mainCam = Camera.main;
        stateManager = FindObjectsByType<UIStateManager>(FindObjectsSortMode.None)[0];
        GameObject tilemapObject = GameObject.FindWithTag("Placeable");
        placementTilemap = tilemapObject.GetComponent<Tilemap>();
        isPlacementMode = false;
        rend = towerPrePrefab.GetComponent<Renderer>();
    }

    // desc: base unity function called once per frame
    // pre: none
    // post: none
    protected virtual void Update(){
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)){
            isPlacementMode = false;
            if (towerPre != null){
                Destroy(towerPre);
                stateManager.CloseAll();
            }
        }

        if (isPlacementMode){
            TowerPreview();
            stateManager.CloseAll();
            if (Input.GetMouseButtonDown(0)){
                TryTowerPlace();
                stateManager.CloseAll();
            }
        }
    }


    // desc: starts placing the tower
    // pre: none
    // post: none
    public void ActivatePlacementMode(){
        if (!isPlacementMode){
            isPlacementMode = true;

            if(towerPre == null && towerPrePrefab != null){
                towerPre = Instantiate(towerPrePrefab);
                towerPre.SetActive(false);
            }
        }
    }

    // desc: creates the tower preview of the given tower
    // pre: none
    // post: none
    protected void TowerPreview(){
        if (towerPrePrefab == null) return;

        UnityEngine.Vector3 worldPoint = mainCam.ScreenToWorldPoint(Input.mousePosition);
        worldPoint.z = 0;

        Collider2D existingTower = Physics2D.OverlapCircle(worldPoint, 0.5f, towerLayer);
        UnityEngine.Vector3Int cellpos = placementTilemap.WorldToCell(worldPoint);
        TileBase tile = placementTilemap.GetTile(cellpos);

        towerPre.SetActive(true);
        towerPre.transform.position = worldPoint;

        if (placementTilemap.HasTile(cellpos) && tile != null && existingTower == null && cost <= GameManager.money){
            rend.sharedMaterial.color = Color.white;
        } else {
            rend.sharedMaterial.color = Color.red;
        }
    }

    // desc: attempts to place tower if the location is valid
    // pre: none
    // post: none
    protected void TryTowerPlace(){
        if (towerPre == null || !towerPre.activeSelf) return;

        UnityEngine.Vector3 worldPoint = mainCam.ScreenToWorldPoint(Input.mousePosition);
        worldPoint.z = 0;
        
        Collider2D existingTower = Physics2D.OverlapCircle(worldPoint, 0.5f, towerLayer);
        UnityEngine.Vector3Int cellpos = placementTilemap.WorldToCell(worldPoint);
        TileBase tile = placementTilemap.GetTile(cellpos);

        if (placementTilemap.HasTile(cellpos) && tile != null && GameManager.money >= cost && !existingTower){
            GameManager.money -= cost;

            UnityEngine.Vector3 towerPos = worldPoint;
            Instantiate(towerPrefab, towerPos, UnityEngine.Quaternion.identity);
            isPlacementMode = false;
            Destroy(towerPre);
        }
    }
}
