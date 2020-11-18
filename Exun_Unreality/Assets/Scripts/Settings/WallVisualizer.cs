using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings
{
    [ExecuteInEditMode]
    public class WallVisualizer : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Bounds bounds = new Bounds();
            for (int i = 0; i < transform.childCount; i++)
            {
                bounds.Encapsulate(transform.GetChild(i).position);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}

