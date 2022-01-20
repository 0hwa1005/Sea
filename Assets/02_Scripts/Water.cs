using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public static bool b_isWater = false;
    [SerializeField] private float waterDrag=5.0f;//���� �߷�
    [SerializeField] private Color waterColor;//���� ��
    [SerializeField] private float waterFogDensity=0.17f;//�� Ź�� ����

    private float originDrag;
    private Color originColor;
    private float originFogDensity;



    void Start()
    {
        originColor = RenderSettings.fogColor;
        originFogDensity = RenderSettings.fogDensity;

        originDrag = 0;
    }

    void Update()
    {
        if(b_isWater)
        {
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GetWater(other);
        Debug.Log("GetWater");
    }

    private void OnTriggerExit(Collider other)
    {
        GetOutWater(other);
        Debug.Log("GetOutWater");
    }

    private void GetWater(Collider _player)
    {
        b_isWater = true;
        _player.transform.GetComponent<Rigidbody>().drag = waterDrag;

        RenderSettings.fogColor = waterColor;
        RenderSettings.fogDensity = waterFogDensity;
    }

    private void GetOutWater(Collider _player)
    {
        b_isWater = false;
        _player.transform.GetComponent<Rigidbody>().drag = originDrag;

        RenderSettings.fogColor = originColor;
        RenderSettings.fogDensity = originFogDensity;
    }
}
