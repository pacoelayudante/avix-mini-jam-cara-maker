using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mezcladora : MonoBehaviour
{
    List<ControlCabeza> entra = new List<ControlCabeza>();
    public ControlCabeza sale;

    void Start()
    {
        sale.ClearAll();
    }

    private void OnTriggerEnter(Collider other)
    {
        var cab = other.GetComponentInChildren<ControlCabeza>();
        if (cab && !entra.Contains(cab))
        {
            entra.Add(cab);
            entra = entra.Where(e=>e).ToList();
            MezclarInst();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // var cab = other.GetComponentInChildren<ControlCabeza>();
        // if(cab) entra.Remove(cab);        
        // MezclarInst();
    }

    public static void Mezclar(List<ControlCabeza> entra, ControlCabeza sale)
    {
        if (entra != null && entra.Count > 0 && sale)
        {
            sale.ClearAll();

            for (int t = 0; t < sale.combinaciones.Count; t++)
            {
                var grupo = sale.combinaciones[t];

                var opciones = entra.Select(c => c.Indices[t]).Where(ind => ind >= 0).ToArray();
                if (opciones.Length > 0) sale.SetCombinacion(grupo, opciones[Random.Range(0, opciones.Length)]);
            }

        }
    }

    public void MezclarInst()
    {
        MezclarInst(entra, sale);
    }
    public void MezclarInst(List<ControlCabeza> entra, ControlCabeza sale)
    {
        Mezclar(entra, sale);
    }
}
