using System.Collections;
using System.Collections.Generic;

using Unity.AI.Navigation;
using UnityEditor.AI;
using UnityEngine;

public class AINavManager : Singleton<AINavManager>
{
    public NavMeshSurface navMeshSurface;
    public NavMeshBuilder navMeshBuilder;

    void Start()
    {
 
    }

    public void UpdateNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
   
}