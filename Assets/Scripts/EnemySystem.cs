using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySystem : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    private float distance;
    private bool playerHasSeen;
    private Vector3 mainLocation;
    
    public float acceleration = 2f;
    public float deceleration = 100f;
    public float closeEnoughMeters = 4f;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        mainLocation = transform.position;
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, player.position);
        
        if (distance < 12)
        {
            playerHasSeen = true;
            agent.SetDestination(player.position);
        }

        if (distance > 20)
        {
            playerHasSeen = false;
            agent.SetDestination(mainLocation);
        }
    }
}
