using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agarre : MonoBehaviour
{
    public CodigoGenetico cabezaApuntada;
    public CodigoGenetico cabezaAgarrada;
    public LayerMask mascaraAgarre;

    Rigidbody _rigid;
    Rigidbody Rigid => _rigid ? _rigid : _rigid = GetComponent<Rigidbody>();

    public Camera watchCam;
    Vector3 watchCamOriginalPos;
    Quaternion watchCamOriginalRot;
    public Vector3 watchCamDist = new Vector3(0, 2, 6);
    Joint _agarre;
    Joint AgarreJoint => _agarre ? _agarre : _agarre = GetComponent<Joint>();


    void Start()
    {
        if (watchCam)
        {
            watchCamOriginalPos = watchCam.transform.position;
            watchCamOriginalRot = watchCam.transform.rotation;
        }
    }

    void Agarrar(CodigoGenetico codigo)
    {
        // if (agarre) Destroy(agarre);
        cabezaAgarrada = codigo;

        if (AgarreJoint && codigo && codigo.Rigid)
        {
            cabezaAgarrada.Agarrada = true;
            AgarreJoint.connectedBody = codigo.Rigid;
        }
    }
    void Soltar()
    {
        if (watchCam)
        {
            watchCam.transform.position = watchCamOriginalPos;
            watchCam.transform.rotation = watchCamOriginalRot;
        }
        if (AgarreJoint)
        {
            if (cabezaAgarrada) cabezaAgarrada.Agarrada = false;
            AgarreJoint.connectedBody = null;
            cabezaAgarrada = null;
        }
    }

    void Update()
    {
        if (cabezaAgarrada)
        {
            if (!Input.GetMouseButton(0))
            {
                Soltar();
            }
        }
        else
        {
            var rayo = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hits;
            if (Physics.Raycast(rayo, out hits, 1000, mascaraAgarre, QueryTriggerInteraction.Collide))
            {
                cabezaApuntada = hits.transform.GetComponentInParent<CodigoGenetico>();
            }
            else cabezaApuntada = null;

            if (watchCam)
            {
                if (cabezaApuntada)
                {
                    watchCam.transform.position = cabezaApuntada.CabPropiaTransform.TransformPoint(watchCamDist);
                    watchCam.transform.rotation = cabezaApuntada.CabPropiaTransform.rotation * Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    watchCam.transform.position = watchCamOriginalPos;
                    watchCam.transform.rotation = watchCamOriginalRot;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Agarrar(cabezaApuntada);
            }
        }
    }
}
