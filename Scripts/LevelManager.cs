using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public void LevelCleared()
    {
        GameManager.Instance.OnLevelCleared();
    }
}
