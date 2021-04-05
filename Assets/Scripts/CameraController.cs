using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public Tilemap currentMap;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    private float halfHeight;
    private float halfWidth;

    public int musicToPlay;
    private bool musicStarted;

    // Start is called before the first frame update
    void Start()
    {
        //target = PlayerController.instance.transform;
        target = PlayerController.instance.transform;

        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;

        bottomLeftLimit = currentMap.localBounds.min + new Vector3(halfWidth, halfHeight, 0);
        topRightLimit = currentMap.localBounds.max + new Vector3(-halfWidth, -halfHeight, 0);
        PlayerController.instance.SetBounds(currentMap.localBounds.min, currentMap.localBounds.max);
    }

    // Late Update is called once per frame after Update
    void LateUpdate()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        //keep camera in bounds

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z);
        if(!musicStarted){
            musicStarted = true;
            AudioManager.instance.PlayMusic(musicToPlay);
        }
    }
}
