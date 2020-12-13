using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TodoLoDemas : MonoBehaviour
{
    public CinemachineVirtualCamera camFrasco, camTeles;
    public Agarre agarre;
    public ControlCabeza objetivo;

    public CodigoGenetico prefabCodigoGenetico;
    public float limiteYCaida;

    public Transform posAgarreFrasco, posSaleGenes;
    Vector3 posAgarreOriginal;
    public float factorAgarreMov = 30f;

    public float alturaCambioCam = 0.1f;
    public float anchoCambioCam = 0.3f;

    public int generarCodigosGeneticos = 30;
    public float tirarCada = .5f;

    void Start()
    {
        if (agarre) posAgarreOriginal = agarre.transform.position;
        objetivo.RandomAll();

        StartCoroutine(GenCodigos());
    }

    IEnumerator GenCodigos()
    {
        var cont = 0;
        while (cont < generarCodigosGeneticos)
        {
            cont++;
            GenerarCodigoGenetico();
            yield return new WaitForSeconds(tirarCada);
        }
    }

    void GenerarCodigoGenetico()
    {
        var nuevoGen = Instantiate(prefabCodigoGenetico, posSaleGenes.transform.position, Quaternion.Euler(Random.value * 360f, Random.value * 360f, Random.value * 360f));
        nuevoGen.StartCoroutine(ReiniciarAlCaer(nuevoGen));
    }
    IEnumerator ReiniciarAlCaer(CodigoGenetico gen)
    {
        while (gen)
        {
            if (gen.transform.position.y < limiteYCaida)
            {
                gen.Rigid.velocity = Vector3.zero;
                gen.transform.position = posSaleGenes.transform.position;
                gen.GenForm();
            }
            yield return new WaitForSeconds(.3f);
        }
    }

    void Update()
    {
        if (Input.mousePosition.y > Screen.height * (1f - alturaCambioCam))
        {

            if (Input.mousePosition.x < Screen.width * anchoCambioCam)
            {
                camFrasco.enabled = true;
                camTeles.enabled = false;
            }
            else if (Input.mousePosition.x > Screen.width * (1f - anchoCambioCam))
            {
                camTeles.enabled = true;
                camFrasco.enabled = false;
            }
            else
            {
                camTeles.enabled = false;
                camFrasco.enabled = false;
            }

        }
        else
        {
            camTeles.enabled = false;
            camFrasco.enabled = false;
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        var agarreGoTo = camFrasco.enabled ? posAgarreFrasco.position : posAgarreOriginal;
        agarre.transform.position = Vector3.Lerp(agarre.transform.position, agarreGoTo, factorAgarreMov * Time.deltaTime);
    }
}
