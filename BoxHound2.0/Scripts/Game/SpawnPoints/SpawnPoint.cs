using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

    public static readonly string PlayerObjectTag = "Target";

    [SerializeField]
    private bool m_HasObstacleInArea = false;

    public bool SpawnPointAvailable {
        get { return !m_HasObstacleInArea; }
    }

    void OnTriggerStay(Collider other) {
        if ( other.CompareTag("Target"))
            m_HasObstacleInArea = true;
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Target"))
            m_HasObstacleInArea = false;
    }
}
