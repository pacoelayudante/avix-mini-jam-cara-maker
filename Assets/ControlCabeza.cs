using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ControlCabeza : MonoBehaviour
{
    public List<Transform> combinaciones;
    List<int> _indices;
    public List<int> Indices
    {
        get
        {
            if (_indices == null) _indices = new List<int>();
            if (_indices.Count < combinaciones.Count) _indices.AddRange(Enumerable.Repeat(-1, combinaciones.Count - _indices.Count));
            return _indices;
        }
    }

    public Vector3 escalaRespiracionAmp = Vector3.up;
    public float velCicloRespiracion = 360;
    float respiraOffset;
    void Start() {
        respiraOffset = Random.value*Mathf.PI*2f;
    }
    void Update() {
        respiraOffset += Mathf.Deg2Rad*velCicloRespiracion*Time.deltaTime;
        var ampResp = Vector3.one + escalaRespiracionAmp* Mathf.Sin(respiraOffset);
        transform.localScale = ampResp;

    }

    public void SetCombinacion(int t, int indice)
    {
        if (combinaciones == null || combinaciones.Count == 0) return;
        t = (t % combinaciones.Count + combinaciones.Count) % combinaciones.Count;
        SetCombinacion(combinaciones[t], indice);
    }
    public void SetCombinacion(Transform t, int indice)
    {
        if (t.childCount == 0) return;
        indice %= t.childCount;
        if (indice < -1) indice = t.childCount - 1;

        var pos = combinaciones.IndexOf(t);
        if (pos >= 0 && pos < Indices.Count) Indices[pos] = indice;

        for (int i = 0; i < t.childCount; i++)
        {
            t.GetChild(i).gameObject.SetActive(i == indice);
        }
    }

    public void RandomAll()
    {
        for (int t = 0; t < combinaciones.Count; t++)
        {
            var grupo = combinaciones[t];
            SetCombinacion(grupo, Random.Range(0, grupo.childCount));
        }
    }
    public void ClearAll(){
        for (int t = 0; t < combinaciones.Count; t++)
        {
            var grupo = combinaciones[t];
            SetCombinacion(grupo, -1);
        }
    }
    public void RandomSome(int count) {
        if (count < 0 || combinaciones==null) return;
        var sel = Enumerable.Repeat(0,combinaciones.Count).Select((x,ind)=>ind).ToList();
        while (sel.Count > count) {
            sel.RemoveAt(Random.Range(0,sel.Count));
        }
        foreach(var ind in sel) {
            var grupo = combinaciones[ind];
            SetCombinacion(grupo, Random.Range(0, grupo.childCount));
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ControlCabeza))]
    public class EstoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            foreach (ControlCabeza c in targets)
            {
                GUILayout.Label(c.name);
                if (c.combinaciones != null)
                {
                    if (GUILayout.Button("Random All"))
                    {
                        c.RandomAll();
                    }
                    for (int t = 0; t < c.combinaciones.Count; t++)
                    {

                        var grupo = c.combinaciones[t];
                        var actual = c.Indices[t];
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label($"{grupo.name}({actual})");
                        if (GUILayout.Button("<"))
                        {
                            actual--;
                            actual = (actual % grupo.childCount + grupo.childCount) % grupo.childCount;
                            c.SetCombinacion(grupo, actual);
                        }
                        if (GUILayout.Button(">"))
                        {
                            actual++;
                            actual = (actual % grupo.childCount + grupo.childCount) % grupo.childCount;
                            c.SetCombinacion(grupo, actual);
                        }
                        if (GUILayout.Button("X"))
                        {
                            c.SetCombinacion(grupo, -1);
                        }
                        if (GUILayout.Button("R"))
                        {
                            c.SetCombinacion(grupo, Random.Range(0, grupo.childCount));
                        }
                        EditorGUILayout.EndHorizontal();

                    }
                }
            }
        }
    }
#endif
}
