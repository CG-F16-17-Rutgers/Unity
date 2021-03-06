﻿using UnityEngine;
using System.Collections;

public class GoblinMove : MonoBehaviour {
    
    NavMeshObstacle obs;
    NavMeshAgent agent;
    bool isArrived = true;
    Vector3 target;
    public static float nbrInTarget = 0;
    public static int selectedTotal = 0;
    private Ray shootRay;
    bool isSelected;
    bool hasMoved;
    public void stop()
    {
        agent.Stop();
    }
    void OnTriggerEnter(Collider other)
    {
        if (!isArrived)
        {
            agent.Stop();
            Debug.Log("hitted by agent");
        }
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        obs = GetComponent<NavMeshObstacle>();
        hasMoved = false;
    }

	// Update is called once per frame
	void Update () {
        if (!hasMoved)
        {
            target = new Vector3(-12.8f,-1.0f, 1f);
            isArrived = false;
            agent.enabled = true;
            obs.enabled = true;
            Debug.Log(target);
            agent.SetDestination(target);
            hasMoved = true;
        }
        if (Vector3.Distance(transform.position, target) < 2.0f)
        {
            agent.enabled = false;
            obs.enabled = true;
        }
        else
        {
            agent.enabled = true;
            obs.enabled = false;
        }   
	}
    public void isNowSelected()
    {
        SkinnedMeshRenderer rend = this.gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        rend.material.shader = Shader.Find("Sprites/Diffuse");
        isSelected = true;
        selectedTotal += 1;
        Debug.Log (selectedTotal);
    }

    //used by director to toggle the agent being selected off
    public void isNowNotSelected()
    {
        SkinnedMeshRenderer rend = this.gameObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        rend.material.shader = Shader.Find("Standard");
        isSelected = false;
        selectedTotal -= 1;
    }

    public bool getIsSelected()
    {
        return isSelected;
    }
    //used by Director to set the location of travel
    public void setTarget(RaycastHit givenHit)
    {
        
        target = givenHit.point;
    
        isArrived = false;
        agent.enabled = true;
        obs.enabled = false;
        Debug.Log(target);
        agent.SetDestination(target);

    }
}
