using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public bool UsedPortal;
    [HideInInspector] public int TargetPortalIndex;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        SceneManager.activeSceneChanged += CheckPortals;
        DontDestroyOnLoad(this);
    }

    private void CheckPortals(Scene current, Scene next)
    {
        GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
        if (UsedPortal == true)
        {
            UsedPortal = false;
            GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");

            for (int i = 0; i < portals.Length; ++i)
            {
                if (portals[i].GetComponent<Portal>().Index == TargetPortalIndex)
                {
                    player.transform.position = (Vector2)portals[i].transform.position + portals[i].GetComponent<Portal>().PlayerOffset;
                    player.GetComponent<SpriteRenderer>().flipX = player.transform.position.x < portals[i].transform.position.x;
                    break;
                }
            }
        }
    }
}
