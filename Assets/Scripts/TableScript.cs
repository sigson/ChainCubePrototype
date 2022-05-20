using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableScript : MonoBehaviour
{
    public GameObject CubeExample;
    public GameManager gameManager;
    public GameObject TableBottom;
    public GameObject TableBorderLeft;
    public GameObject TableBorderLeftConnectPoint;
    public GameObject TableBorderRight;
    public GameObject TableBorderRightConnectPoint;
    public GameObject TableBorderForward;
    public GameObject TableBorderForwardConnectPoint;
    public GameObject CameraPoint;
    public GameObject ShootCubeSpawnZone;
    public GameObject MaxCubeMovementLeft;
    public GameObject MaxCubeMovementRight;
    private Vector3 cacheLeftBorder;
    private Vector3 cacheRightBorder;
    public void Start()
    {
        cacheLeftBorder = this.TableBorderLeft.transform.localPosition;
        cacheRightBorder = this.TableBorderRight.transform.localPosition;
    }

    public void RecalculateTable(int xMaxCube, int zMaxCube)
    {
        TableBottom.transform.localPosition = Vector3.zero;
        this.TableBorderLeft.transform.localPosition = cacheLeftBorder;
        this.TableBorderRight.transform.localPosition = cacheRightBorder;
        TableBorderForward.transform.localPosition = new Vector3(0, TableBorderForward.transform.localPosition.y, TableBorderForward.transform.localPosition.z);
        var cubeBounds = CubeExample.GetComponent<MeshFilter>().sharedMesh.bounds.size * CubeExample.transform.localScale.x;

        TableBottom.transform.localScale = new Vector3(cubeBounds.x * xMaxCube + gameManager.cubeSeparationValue * xMaxCube, TableBottom.transform.localScale.y, TableBottom.transform.localScale.z);
        TableBottom.transform.localPosition = new Vector3(TableBottom.transform.localPosition.x + cubeBounds.x * xMaxCube / 2 + gameManager.cubeSeparationValue * xMaxCube / 2 - cubeBounds.x / 2 - gameManager.cubeSeparationValue / 2, TableBottom.transform.localPosition.y, TableBottom.transform.localPosition.z);


        TableBorderLeft.transform.localScale = new Vector3(TableBorderLeft.transform.localScale.x, TableBorderLeft.transform.localScale.y, TableBottom.transform.localScale.z);
        TableBorderLeft.transform.position = new Vector3(TableBorderLeftConnectPoint.transform.position.x , TableBorderLeft.transform.position.y, TableBorderLeftConnectPoint.transform.position.z);
        TableBorderLeft.transform.localPosition = new Vector3(TableBorderLeft.transform.localPosition.x < 0f ? TableBorderLeft.transform.localPosition.x - TableBorderLeft.transform.localScale.x/2 : TableBorderLeft.transform.localPosition.x + TableBorderLeft.transform.localScale.x / 2, TableBorderLeft.transform.localPosition.y, TableBorderLeftConnectPoint.transform.localPosition.z);

        TableBorderRight.transform.localScale = new Vector3(TableBorderRight.transform.localScale.x, TableBorderRight.transform.localScale.y, TableBottom.transform.localScale.z);
        TableBorderRight.transform.position = new Vector3(TableBorderRightConnectPoint.transform.position.x, TableBorderLeft.transform.position.y, TableBorderRightConnectPoint.transform.position.z);
        TableBorderRight.transform.localPosition = new Vector3(TableBorderRight.transform.localPosition.x < 0f ? TableBorderRight.transform.localPosition.x - TableBorderRight.transform.localScale.x / 2 : TableBorderRight.transform.localPosition.x + TableBorderRight.transform.localScale.x / 2, TableBorderRight.transform.localPosition.y, TableBorderLeftConnectPoint.transform.localPosition.z);

        TableBorderForward.transform.localScale = new Vector3(cubeBounds.x * xMaxCube + gameManager.cubeSeparationValue * xMaxCube + TableBorderRight.transform.localScale.x + TableBorderLeft.transform.localScale.x, TableBorderForward.transform.localScale.y, TableBorderForward.transform.localScale.z);
        TableBorderForward.transform.position = new Vector3(TableBorderForwardConnectPoint.transform.position.x, TableBorderForward.transform.position.y, TableBorderForwardConnectPoint.transform.position.z);
    }
}
