using UnityEngine;

[System.Serializable]
public class BackgroundElement
{
    public SpriteRenderer background;
    [Range(0, 1)] public float scrollSpeed;

    [HideInInspector] public MaterialPropertyBlock propertyBlock;
}

public class BackgroundScrolling : MonoBehaviour
{
    private const float SCROLL_MULTIPLIER = 0.01f;

    [SerializeField] private BackgroundElement[] backgroundElements;

    private void Start()
    {
        foreach (BackgroundElement element in backgroundElements)
        {
            element.propertyBlock = new MaterialPropertyBlock();
        }
    }

    private void Update()
    {
        foreach (BackgroundElement element in backgroundElements)
        {
            element.background.GetPropertyBlock(element.propertyBlock);

            float offsetX = transform.position.x * element.scrollSpeed * SCROLL_MULTIPLIER;

            // scale = (1,1), offset = (offsetX, 0)
            element.propertyBlock.SetVector("_MainTex_ST", new Vector4(1, 1, offsetX, 0));

            element.background.SetPropertyBlock(element.propertyBlock);
        }
    }
}
