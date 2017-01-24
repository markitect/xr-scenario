﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;
using UnityEngine.Windows.Speech;
using UnityEngine.Networking;

public class ArControls : NetworkBehaviour
{
    KeywordRecognizer m_KeywordRecognizer = null;
    GestureRecognizer m_GestureReconizer = null;

    GameObject Wall1;
    GameObject Wall2;
    GameObject RedMirror;

    bool RedMirrorMoved = false;
    bool WallTracking = true;
    bool WallLimit = false;

    public string[] controlWords;
    private float m_currentTrackSpeed = 6.5f;
    float m_WallCoolDown = 0f;
    public float m_CoolDown = 10f;
    int m_WallCount = 0;
    public int m_AllowedWalls = 2;

    void Start()
    {
        m_KeywordRecognizer = new KeywordRecognizer(controlWords);
        m_KeywordRecognizer.OnPhraseRecognized += M_KeywordRecognizer_OnPhraseRecognized;
        m_KeywordRecognizer.Start();

        m_GestureReconizer = new GestureRecognizer();
        m_GestureReconizer.SetRecognizableGestures(GestureSettings.Tap
            | GestureSettings.NavigationX
            | GestureSettings.NavigationY
            | GestureSettings.NavigationZ);
        m_GestureReconizer.TappedEvent += M_GestureReconizer_TappedEvent;
        m_GestureReconizer.NavigationUpdatedEvent += M_GestureReconizer_NavigationUpdatedEvent;
        m_GestureReconizer.StartCapturingGestures();
    }

    private void M_GestureReconizer_NavigationUpdatedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        RaycastHit hit;
        if (Physics.Raycast(headRay, out hit, 10f))
        {
            if (hit.collider.name.Contains("LaserBlock") || hit.collider.name.Contains("Mirror"))
            {
                var wall = GameObject.Find(hit.collider.name);
                wall.transform.Rotate(normalizedOffset);
            }
        }
    }

    void Update()
    {
        //if (m_WallCoolDown < 0)
        //{
            if (WallTracking)
            {
                MoveWall();
            }
        //}
        
        if(m_WallCoolDown > 0)
        {
            m_WallCoolDown -= Time.deltaTime;
        }

        if (RedMirrorMoved)
        {
            RedMirror = GameObject.Find("RedMirror");
            MoveObject(RedMirror);
        }
    }
    
    private void M_GestureReconizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        if (!WallTracking)
        {
            WallTracking = true;
            m_WallCoolDown = m_CoolDown;
        }
        else if (WallTracking)
        {
            WallTracking = false;
            m_WallCoolDown = 0;
        }

        m_WallCount = tapCount;
    }

    private void M_KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if(args.text == "Wall")
        {
            if(m_WallCount > m_AllowedWalls)
            {
                WallTracking = false;
            }
            else if(m_WallCount < m_AllowedWalls)
            {
                WallTracking = true;
                m_WallCount += 1;
            }         
        }

        if (args.text == "Red")
        {
            RedMirrorMoved = true;
        }
    }

    public void MoveWall()
    {
        Wall1 = GameObject.Find("LaserBlock(1)");
        Wall2 = GameObject.Find("LaserBlock(2)");

        if (m_WallCount.Equals(1))
        {
            MoveObject(Wall1);
        }

        if (m_WallCount.Equals(2))
        {
            MoveObject(Wall2);
        }
    }

    public void MoveObject(GameObject obj)
    {
        var cam = Camera.main.transform;
        Vector3 move = cam.forward * 4f + cam.position;
        obj.transform.position = Vector3.Lerp(obj.transform.position, move, Time.deltaTime * m_currentTrackSpeed);
        obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, cam.rotation, Time.deltaTime * m_currentTrackSpeed);
    }
}
