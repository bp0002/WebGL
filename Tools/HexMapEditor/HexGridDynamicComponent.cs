using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HexMapEditor
{
    class HexGridDynamicComponent : MonoBehaviour
    {
        public static int counter = 0;
        public string Name = "HexGridDynamic" + counter++;

        public List<HexCellDynamicComponent> getHexCells()
        {
            var arr = gameObject.GetComponentsInChildren<HexCellDynamicComponent>();

            return new List<HexCellDynamicComponent>(arr);
        }

        public void CreateCellDynamic(int x, int y, int z, Boolean isHex, Boolean isRotate, float cellSize, Vector3 position, HexCellDynamicTemplate templateCellDyn)
        {

            CellListComponent cellListComp = gameObject.GetComponentInChildren<CellListComponent>();

            Transform parentTransform = gameObject.transform;

            if (cellListComp != null)
            {
                parentTransform = cellListComp.transform;
            }

            if (isHex)
            {
                GameObject go = new GameObject();
                var comp = go.AddComponent<HexCellDynamicComponent>();
                comp.initData(x, y, z, templateCellDyn);
                comp.CreatePlane();
                comp.UpdateFromTemplate(templateCellDyn);
                comp.UpdateHexPlane();
                go.transform.parent = parentTransform;

                go.transform.localScale = new Vector3(templateCellDyn.cellSize - 0.05f, templateCellDyn.cellSize - 0.05f, templateCellDyn.cellSize - 0.05f);
                go.transform.position = new Vector3(position.x, position.y, position.z);
                if (isRotate)
                {
                    go.transform.localRotation = Quaternion.Euler(0, 30, 0);
                }
                else
                {
                    go.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }

                Undo.RegisterCreatedObjectUndo(go, go.name);
                EditorUtility.SetDirty(go);
            } else
            {
                GameObject go = new GameObject();
                var comp = go.AddComponent<HexCellDynamicComponent>();
                comp.initData(x, y, z, templateCellDyn);
                comp.CreatePlane();
                comp.UpdateFromTemplate(templateCellDyn);
                comp.UpdatePlane();

                go.transform.parent = parentTransform;
                go.transform.position = new Vector3(position.x, position.y, position.z);

                if (isRotate)
                {
                    if (Parame.planIndex == 0)
                    {
                        go.transform.position = new Vector3(position.x, position.y, position.z);
                    }
                    else if (Parame.planIndex == 1)
                    {
                        go.transform.position = new Vector3(position.x - (templateCellDyn.cellSize - cellSize) / 2.0f, position.y, position.z - (templateCellDyn.cellSize - cellSize) / 2.0f);
                    }
                    else if (Parame.planIndex == 2)
                    {
                        go.transform.position = new Vector3(position.x + (templateCellDyn.cellSize - cellSize) / 2.0f, position.y, position.z + (templateCellDyn.cellSize - cellSize) / 2.0f);
                    }
                } else
                {
                    if (Parame.diamondIndex == 0)
                    {
                        go.transform.position = new Vector3(position.x, position.y, position.z);
                    }
                    else if (Parame.diamondIndex == 1)
                    {
                        go.transform.position = new Vector3(position.x, position.y, position.z - (templateCellDyn.cellSize - cellSize) * HexMetrics.squrt2 / 2.0f);
                    }
                    else if (Parame.diamondIndex == 2)
                    {
                        go.transform.position = new Vector3(position.x, position.y, position.z + (templateCellDyn.cellSize - cellSize) * HexMetrics.squrt2 / 2.0f);
                    }
                }
                //if (isRotate)
                //{
                //    go.transform.position = new Vector3(position.x - (templateCellDyn.cellSize - cellSize) / 2.0f, position.y, position.z - (templateCellDyn.cellSize - cellSize) / 2.0f);
                //} else
                //{
                //    go.transform.position = new Vector3(position.x - (templateCellDyn.cellSize - cellSize) / 2.0f, position.y, position.z - (templateCellDyn.cellSize - cellSize) / 2.0f);
                //}
                go.transform.localScale = new Vector3(templateCellDyn.cellSize - 0.05f, templateCellDyn.cellSize - 0.05f, templateCellDyn.cellSize - 0.05f);
                if (isRotate)
                {
                    go.transform.localRotation = Quaternion.Euler(0, 0, 0);
                } else
                {
                    go.transform.localRotation = Quaternion.Euler(0, 45, 0);
                }

                Undo.RegisterCreatedObjectUndo(go, go.name);
                EditorUtility.SetDirty(go);
            }
        }
    }
}
