using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [SerializeField] private Transform background;
    [SerializeField] private Transform foreground;

    private Vector3 bufferPosition = Vector3.zero;

    private float ferogroundHorizontalSpeed = .7f;
    private float ferogroundVerticalSpeed = .3f;

    private Vector3 lastCameraPosition = Vector3.zero;

    private void Update()
    {
        this.ParallaxBackground();
        this.ParallaxForeground();

        lastCameraPosition = this.transform.position;
    }

    private void ParallaxBackground()
    {
        bufferPosition = background.localPosition;

        bufferPosition.x = Mathf.Lerp(11f, -11f, (this.transform.position.x + 100f) / 200f);
        bufferPosition.y = Mathf.Lerp(15f, -15f, (this.transform.position.y + 70f) / 140f);

        background.localPosition = bufferPosition;
    }

    private void ParallaxForeground()
    {
        bufferPosition = foreground.localPosition;

        bufferPosition.x -= ferogroundHorizontalSpeed * Time.deltaTime + (this.transform.position.x - lastCameraPosition.x);
        if (bufferPosition.x <= -28) { bufferPosition.x *= -1; }

        bufferPosition.y += ferogroundVerticalSpeed * Time.deltaTime - (this.transform.position.y - lastCameraPosition.y);
        if (bufferPosition.y >= 28) { bufferPosition.y *= -1; }

        foreground.localPosition = bufferPosition;
    }
}
