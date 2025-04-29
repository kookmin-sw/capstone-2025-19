using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGetStatus : MonoBehaviour
{
    public void GetExpFromEnemy(int exp)
    {
        PlayerStatusController.Instance.getExp(exp);
    }
}
