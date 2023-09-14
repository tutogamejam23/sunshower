using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ParallaxScrollingBackground : MonoBehaviour
{
    [SerializeField] GameObject camera_object = null;

    [SerializeField] Transform background_leftPoint = null, background_rightPoint = null;
    [SerializeField] Transform ground_leftPoint = null, ground_rightPoint = null;
    [SerializeField] Transform camera_leftPoint = null, camera_rightPoint = null;

    float ground_sideSpace = 0f, background_sideSpace = 0f;

    void Start()
    {
        float camera_width = camera_leftPoint.position.x - camera_rightPoint.position.x;
        ground_sideSpace = ground_rightPoint.position.x - ground_leftPoint.position.x;
        background_sideSpace = background_leftPoint.position.x - background_rightPoint.position.x - camera_width * 0.5f;
    }

    void Update()
    {
        SetPosition();
    }

    void SetPosition()
    {
        float background_xPos = camera_object.transform.position.x + ((camera_object.transform.position.x - ground_leftPoint.position.x) / ground_sideSpace - 0.5f) * background_sideSpace;

        transform.position = new Vector2(background_xPos, transform.position.y);
    }
}
