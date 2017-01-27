﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LaserShooter : NetworkBehaviour
{
    private enum FireState
    {
        None,
        Charging,
        Firing
    }
    
    private FireState m_FireState;
    private GameObject m_Laserbeam;
    private LineRenderer m_Beam;
    private ShootLaser m_BeamScript;
    private float m_BeamLength;
    private float m_BeamSpeed;
    
    private bool b_CanChangeColor;
    private int m_CurrentColorIndex;
    private float m_ColorChangeTimer;

    [SerializeField] private GameObject m_LaserBeamPrefab;
    [SerializeField] private Transform m_BarrelTipPosition;
    [SerializeField] private Text m_LaserHUD;

    [SerializeField] private float m_MaxBeamLength;
    [SerializeField] private float m_MaxChargeTime;
    [SerializeField] private float m_MaxBeamSpeed;
    [SerializeField] private float m_MinBeamSpeed;

    [SerializeField] private ParticleSystem m_ChargingEffect;
    [SerializeField] private Color[] m_AvailableColors;
    [SerializeField] private float m_ColorChangeTimeLimit;

    void Start()
    {
        b_CanChangeColor = true;
    }

    void Update()
    {
        m_ColorChangeTimer += Time.deltaTime;

        switch (m_FireState)
        {
            case FireState.None:
                if (Input.GetButtonDown("Fire1"))
                {
                    m_Laserbeam = Instantiate(m_LaserBeamPrefab);
                    m_Beam = m_Laserbeam.GetComponent<LineRenderer>();
                    m_BeamScript = m_Laserbeam.GetComponent<ShootLaser>();
                    m_BeamScript.laserColor = m_AvailableColors[m_CurrentColorIndex];
                    m_BeamSpeed = m_MaxBeamSpeed;

                    if (m_ChargingEffect != null)
                        m_ChargingEffect.gameObject.SetActive(true);

                    m_FireState = FireState.Charging;
                }
                break;

            case FireState.Charging:
                if (Input.GetButtonDown("Fire1") || m_BeamLength >= m_MaxBeamLength)
                {
                    // m_BeamScript.Length = m_BeamLength
                    m_FireState = FireState.Firing;

                    // set beam length back to zero for next beam creation.
                    m_BeamLength = 0;
                    break;
                }

                m_BeamLength += m_MaxBeamLength * Time.deltaTime / m_MaxChargeTime;
                m_LaserHUD.text = m_BeamLength.ToString("n1");
               
                break;

            case FireState.Firing:
                if (true)//(Input.GetButtonDown("Fire1") || m_BeamSpeed <= m_MinBeamSpeed)  bypassing speed charging for now.
                {
                    if (m_ChargingEffect != null)
                        m_ChargingEffect.gameObject.SetActive(false);

                    m_BeamScript.speed = m_BeamSpeed;
                    m_Laserbeam.transform.position = m_BarrelTipPosition.position;
                    m_Laserbeam.transform.rotation = m_BarrelTipPosition.rotation;

                    m_BeamScript.FireLaser();

                    m_LaserHUD.text = "0.0";

                    m_FireState = FireState.None;
                    break;
                }

                // increase or decrease beam speed?
                m_BeamSpeed -= .1f;
                break;
        }

        if (Input.GetAxis("Vertical") > .5 && b_CanChangeColor && m_FireState != FireState.Charging)
        {
            ChangeColor(1);
        }

        if (Input.GetAxis("Vertical") < -.5 && b_CanChangeColor && m_FireState != FireState.Charging)
        {
            ChangeColor(-1);
        }

        if (m_ColorChangeTimer > m_ColorChangeTimeLimit)
            b_CanChangeColor = true;
    }

    public void ChangeColor(int indexAmount)
    {
        m_CurrentColorIndex = (m_CurrentColorIndex + indexAmount) % m_AvailableColors.Length;

        m_LaserHUD.color = m_AvailableColors[m_CurrentColorIndex];

        b_CanChangeColor = false;
        m_ColorChangeTimer = 0;
    }
}
