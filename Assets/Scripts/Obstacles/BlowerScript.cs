using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowerScript : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        SnakeHummanoid humm = other.GetComponent<SnakeHummanoid>();

        if (humm && humm.GetCurrentState != ObstacleType.Heaviness)
        {
            humm.SetBlowerMove(transform.forward);
        }
    }
}
