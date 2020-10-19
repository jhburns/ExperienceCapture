using UnityEngine;

public class LoadingCircle : MonoBehaviour
{
    private RectTransform rect;
    private float speed = 200f;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            rect.Rotate(0f, 0f, speed * Time.unscaledDeltaTime);
        }

    }
}