using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodigoGenetico : MonoBehaviour
{
    List<FlotaFlota> flotasFlotas = new List<FlotaFlota>();

    public Vector2 impulsoCada = new Vector2(1, 5);
    public Vector2 duracionImpulso = new Vector2(1, 2);
    public Vector2Int randomGenCount = new Vector2Int(1, 3);
    public float fuerzaImpulso = 1;

    public float gravedadFlotando = 0.1f;

    public float dragEnLiquido = 10;
    public float dragEnAire = 1;
    public float dragAgarrada = 10;

    public float valorAgitado = 9f;

    bool _agarrada = false;
    public bool Agarrada
    {
        get => _agarrada;
        set
        {
            if (_agarrada != value)
            {
                if (value) cabezaPropia.velCicloRespiracion *= valorAgitado;
                else cabezaPropia.velCicloRespiracion /= valorAgitado;
            }
            _agarrada = value;
            ActualizarFlotaFlota();
        }
    }

    public float rotVel = 180f;

    Rigidbody _rigid;
    public Rigidbody Rigid => _rigid ? _rigid : _rigid = GetComponent<Rigidbody>();

    public Transform CabPropiaTransform => cabezaPropia ? cabezaPropia.transform : transform;

    public ControlCabeza prefabCabeza;
    ControlCabeza cabezaPropia;
    float respiraOffset;

    void Start()
    {
        ActualizarFlotaFlota();

        StartCoroutine(Impulso());
        if (prefabCabeza)
        {
            cabezaPropia = Instantiate(prefabCabeza, transform);
            GenForm();
        }

        respiraOffset = Random.value * Mathf.PI * 2;
    }
    public void GenForm()
    {
        cabezaPropia.ClearAll();
        cabezaPropia.RandomSome(Random.Range(randomGenCount[0], randomGenCount[1] + 1));
    }

    void ActualizarFlotaFlota()
    {
        Rigid.drag = flotasFlotas.Count == 0 ? dragEnAire : dragEnLiquido;
        if (_agarrada) Rigid.drag = dragAgarrada;
    }

    void LateUpdate()
    {
        if (cabezaPropia && Rigid.velocity.magnitude > 0.1f)
        {
            cabezaPropia.transform.rotation = Quaternion.RotateTowards(cabezaPropia.transform.rotation, Quaternion.LookRotation(Rigid.velocity, transform.up), rotVel * Time.deltaTime);
            // cabezaPropia.transform
        }
    }

    IEnumerator Impulso()
    {
        while (gameObject)
        {
            yield return new WaitForSeconds(Random.Range(impulsoCada[0], impulsoCada[1]));
            var dir = Random.onUnitSphere;
            dir.y = Mathf.Abs(dir.y);
            var t = Random.Range(impulsoCada[0], impulsoCada[1]);
            while (t > 0)
            {
                t -= Time.deltaTime;
                if (flotasFlotas.Count > 0) Rigid.AddForce(dir * fuerzaImpulso, ForceMode.Acceleration);
                yield return null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var ff = other.GetComponent<FlotaFlota>();
        if (ff) flotasFlotas.Add(ff);
        ActualizarFlotaFlota();
    }
    private void OnTriggerExit(Collider other)
    {
        var ff = other.GetComponent<FlotaFlota>();
        if (ff) flotasFlotas.Remove(ff);
        ActualizarFlotaFlota();
    }
}
