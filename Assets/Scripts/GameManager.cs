using MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject cubeExample;
    public TableScript Table;
    public GameObject CubeSpace;
    public GamePreset nowPreset;
    public string ColorSetupName;
    private SerializableDictionary<int, Color> ColorSetupValue;
    public int maxHorizontalCubes;
    public int maxVerticalCubes;
    public int maxDepthCubes;
    public float cubeSeparationValue;
    public float cubeUpScaler = 1;
    public float timeoutSec = 1f;
    public float ShootForce = 500;
    [Space(10)]
    public bool autoGeneratePreset;
    public SerializableDictionary<int, Color> autoColors = new SerializableDictionary<int, Color>();
    public SerializableDictionary<GameObject, int> CubeDB = new SerializableDictionary<GameObject, int>();
    private int MaxCountCubeValue {
        get
        {
            if (CubeDB.Count == 0)
                return 2;
            Dictionary<int, int> valueCount = new Dictionary<int, int>();
            foreach(var cubePair in CubeDB)
            {
                if(valueCount.TryGetValue(cubePair.Value, out _))
                {
                    valueCount[cubePair.Value]++;
                }
                else
                {
                    valueCount[cubePair.Value] = 1;
                }
            }
            return valueCount.Where((x)=> x.Value == valueCount.Max((x) => x.Value)).First().Key;
        }
    }
    public CubeColorsData cubeColorsDB;
    public GameSettings gameSettings;
    public GameObject CameraObj;
    public Camera CameraComponent;

    private Vector3 shootableCubePosition;
    private GameObject ShootCube;
    private float MaxCubeMovementBoundLeft = 0f;
    private float MaxCubeMovementBoundRight = 0f;
    public bool Timeout = false;
    public bool GameLoaded = false;
    private float MaxCubeMovementBoundDistance => MaxCubeMovementBoundRight - MaxCubeMovementBoundLeft;
    private Vector3 CubeBounds => cubeExample.GetComponent<MeshFilter>().sharedMesh.bounds.size * cubeExample.transform.localScale.x;

    public delegate void Handler(int data = 0);
    public event Handler? OnSummingEvent;
    public event Handler? OnShotEvent;

    public Text StatsText;

    public int Statistics = 0;

    public void Start()
    {
        ColorSetupValue = cubeColorsDB.ColorsSetup[ColorSetupName];
        OnSummingEvent += UpdateStatistic;
        OnShotEvent += BannerShow;
        Timeout = true;

        shootableCubePosition = new Vector3(0f, Table.TableBottom.transform.localScale.y /2 + CubeBounds.y / 2, Table.ShootCubeSpawnZone.transform.position.z < 0? Table.ShootCubeSpawnZone.transform.position.z + CubeBounds.z + cubeSeparationValue : Table.ShootCubeSpawnZone.transform.position.z - CubeBounds.z - cubeSeparationValue);
    }

    IEnumerator TimeoutWaiter()
    {
        yield return new WaitForSeconds(timeoutSec);
        Timeout = false;
    }
    public void GenerateGame()
    {
        foreach(var oldCube in CubeDB.Keys)
        {
            EasyObjectPool.instance.ReturnObjectToPool(oldCube);
        }
        CubeDB.ClearEx();
        Statistics = 0;
        UpdateStatistic(0);
        if (autoGeneratePreset)
            GeneratePreset();
        Table.RecalculateTable(nowPreset.xCubeSize, nowPreset.zCubeSize);
        this.CameraObj.transform.position = Table.CameraPoint.transform.position;
        #region calculate movement distance
        MaxCubeMovementBoundLeft = CameraComponent.WorldToScreenPoint(Table.MaxCubeMovementLeft.transform.position).x;
        MaxCubeMovementBoundRight = CameraComponent.WorldToScreenPoint(Table.MaxCubeMovementRight.transform.position).x;
        cacheLeftCubeBoundPositon = Table.MaxCubeMovementLeft.transform.position;
        cacheRightCubeBoundPositon = Table.MaxCubeMovementRight.transform.position;
        #endregion

        ShowPresetGame();
        Timeout = true;
        if (Timeout)
        {
            StartCoroutine("TimeoutWaiter");
        }
        ShowNewShootCube();
        GameLoaded = true;
    }

    void Update()
    {
        GameInput();
    }

    private Vector3 cacheLeftCubeBoundPositon;
    private Vector3 cacheRightCubeBoundPositon;
    private bool pressed = false;
    private void GameInput()
    {
        if (!GameLoaded)
            return;
#if UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
            pressed = true;
        if (pressed && ShootCube != null)
        {
            var nowMouse = Input.mousePosition;

            var screenWidth = (float)Screen.width;
            var screenHeight = (float)Screen.height;

            Vector2 pos = nowMouse;
            if (pos.x >= MaxCubeMovementBoundLeft && pos.x <= MaxCubeMovementBoundRight)
            {
                float coefDistance = (pos.x - MaxCubeMovementBoundLeft) / MaxCubeMovementBoundDistance;
                var procentPosition = coefDistance < 0.5f ? (0.5f - coefDistance) * -1 : coefDistance - 0.5f;
                shootableCubePosition = new Vector3(Vector3.Lerp(cacheLeftCubeBoundPositon, cacheRightCubeBoundPositon, coefDistance).x, shootableCubePosition.y, shootableCubePosition.z);
            }

            ShootCube.transform.position = shootableCubePosition;
        }

        if (!Timeout && Input.GetMouseButtonUp(0) && ShootCube != null)
        {
            ShootCube.GetComponent<Rigidbody>().AddForce(Vector3.forward * ShootForce);
            ShootCube = null;
            Timeout = true;
            pressed = false;
            StartCoroutine("TimeoutWaiter");
        }
#endif

#if UNITY_ANDROID
        if (Input.touchCount > 0 && ShootCube != null)
        {
            Touch touch = Input.GetTouch(0);

            var screenWidth = (float)Screen.width;
            var screenHeight = (float)Screen.height;
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began)
            {
                var nowMouse = touch.position;

                Vector2 pos = nowMouse;
                if (pos.x >= MaxCubeMovementBoundLeft && pos.x <= MaxCubeMovementBoundRight)
                {
                    float coefDistance = (pos.x - MaxCubeMovementBoundLeft) / MaxCubeMovementBoundDistance;
                    var procentPosition = coefDistance < 0.5f ? (0.5f - coefDistance) * -1 : coefDistance - 0.5f;
                    shootableCubePosition = new Vector3(Vector3.Lerp(cacheLeftCubeBoundPositon, cacheRightCubeBoundPositon, coefDistance).x, shootableCubePosition.y, shootableCubePosition.z);
                }

                ShootCube.transform.position = shootableCubePosition;
            }

            if(!Timeout && touch.phase == TouchPhase.Ended)
            {
                ShootCube.GetComponent<Rigidbody>().AddForce(Vector3.forward * ShootForce);
                ShootCube = null;
                Timeout = true;
                pressed = false;
                StartCoroutine("TimeoutWaiter");
            }
        }
#endif

        if (!Timeout && ShootCube == null)
        {
            ShowNewShootCube();
            OnShotEvent();
        }
    }

    public void ShowPresetGame()
    {
        if(nowPreset != null)
        {
            var cubeBounds = cubeExample.GetComponent<MeshFilter>().sharedMesh.bounds.size * cubeExample.transform.localScale.x;
            CubeSpace.transform.localPosition = Vector3.zero;
            CubeSpace.transform.localPosition = new Vector3(CubeSpace.transform.localPosition.x, CubeSpace.transform.localPosition.y + cubeBounds.y/2 + Table.TableBottom.transform.localScale.y/2, Table.TableBorderForward.transform.localPosition.z - (nowPreset.zCubeSize * cubeBounds.z + cubeSeparationValue * nowPreset.zCubeSize));
            //Table.TableBottom.transform.
            int floor = 0;
            foreach (var floorPreset in (nowPreset.Reverse ? nowPreset.FloorPresets.ReverseEx() : nowPreset.FloorPresets))
            {
                int row = 0;
                foreach (var rowPreset in (nowPreset.Reverse ? floorPreset.RowPresets.ReverseEx() : floorPreset.RowPresets))
                {
                    int cube = 0;
                    foreach (var cubePoint in (nowPreset.Reverse ? rowPreset.CubePreset.ReverseEx() : rowPreset.CubePreset))
                    {
                        if (cubePoint == 0)
                        {
                            cube++;
                            continue;
                        }
                        var cubeObj = EasyObjectPool.instance.GetObjectFromPool("CubePool", Vector3.zero, Quaternion.identity);
                        cubeObj.transform.SetParent(CubeSpace.transform);
                        cubeObj.transform.localPosition = new Vector3(cube * cubeBounds.x + cubeSeparationValue * cube, floor * cubeBounds.y + cubeSeparationValue * floor, row * cubeBounds.z + cubeSeparationValue * row);
                        cubeObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        cubeObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                        var cubeScript = cubeObj.GetComponent<CubeScript>();
                        cubeScript.gameManager = this;
                        cubeScript.color = this.GetCubeColor(cubePoint);
                        cubeScript.value = cubePoint;
                        cubeScript.UpdateCube();
                        CubeDB.AddEx(cubeScript.gameObject, cubePoint);
                        cube++;
                    }
                    row++;
                }
                floor++;
            }
        }
    }

    public Color GetCubeColor(int cubePoint)
    {
        Color cubecolor;
        if (ColorSetupValue.TryGetValue(cubePoint, out cubecolor))
            return cubecolor;
        else
        {
            if (!autoColors.TryGetValue(cubePoint, out cubecolor))
            {
                var colorList = ColorSetupValue.Values.ToList();
                var newColor = Color.Lerp(colorList[Random.Range(0, ColorSetupValue.Count)], colorList[Random.Range(0, ColorSetupValue.Count)], Random.Range(0.2f, 0.8f));
                autoColors.AddEx(cubePoint, newColor);
                return newColor;
            }
            return cubecolor;
        }
    }

    private void ShowNewShootCube()
    {
        var cubeObj = EasyObjectPool.instance.GetObjectFromPool("CubePool", Vector3.zero, Quaternion.identity);
        cubeObj.transform.SetParent(CubeSpace.transform);
        cubeObj.transform.position = new Vector3(shootableCubePosition.x, shootableCubePosition.y + 0.1f, shootableCubePosition.z);
        cubeObj.transform.rotation = Quaternion.identity;
        var cubeScript = cubeObj.GetComponent<CubeScript>();
        cubeScript.gameManager = this;
        var maxCountCubeValue = MaxCountCubeValue;
        cubeScript.color = this.GetCubeColor(maxCountCubeValue);
        cubeScript.value = maxCountCubeValue;
        cubeScript.includeFirstCollise = true;
        cubeScript.GetComponent<Rigidbody>().velocity = Vector3.zero;
        cubeScript.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        cubeScript.UpdateCube();
        CubeDB.AddEx(cubeScript.gameObject, maxCountCubeValue);
        ShootCube = cubeScript.gameObject;
    }

    public void GeneratePreset()
    {
        int[,,] rawPreset = new int[Random.Range(3, maxVerticalCubes), Random.Range(3, maxDepthCubes), Random.Range(3, maxHorizontalCubes)];
    }

    public void UpdateStatistic(int sum)
    {
        Statistics += sum;
        StatsText.text = Statistics.ToString();
    }

    public void ColliseCubes(CubeScript cubeScript, CubeScript cubeScript1)
    {
        cubeScript.transform.position = Vector3.Lerp(cubeScript.transform.position, cubeScript1.transform.position, 0.5f);
        cubeScript.value += cubeScript.value;
        cubeScript.color = GetCubeColor(cubeScript.value);
        cubeScript.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.up * cubeUpScaler, cubeScript.transform.position);
        cubeScript.UpdateCube();
        CubeDB[cubeScript.gameObject] = cubeScript.value;
        CubeDB.RemoveEx(cubeScript1.gameObject);
        EasyObjectPool.instance.ReturnObjectToPool(cubeScript1.gameObject);
        OnSummingEvent(cubeScript.value);
    }

    public int shotsToBannerShow = 0;
    public void BannerShow(int nul = 0)
    {
        if (shotsToBannerShow == (int)gameSettings.GameSettingsDB["AdMob"]["frequencyShowBanner"].numValue)
        {
            //showbanner
            shotsToBannerShow = 0;
        }
        else
            shotsToBannerShow++;
    }
}
#region GamePreset
[System.Serializable]
public class GamePreset
{
    public bool Reverse = false;
    public int xCubeSize;
    public int zCubeSize;
    public List<FloorPreset> FloorPresets = new List<FloorPreset>();
}

[System.Serializable]
public class FloorPreset
{
    public List<RowPreset> RowPresets = new List<RowPreset>();
}

[System.Serializable]
public class RowPreset
{
    public List<int> CubePreset = new List<int>();
}

#endregion
