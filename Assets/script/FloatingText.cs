using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float fadeDuration = 1f;

    private float timer;
    private TextMeshProUGUI tmp;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        timer = fadeDuration;
    }

    void Update()
    {
        // gerak ke atas
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // countdown untuk fade
        timer -= Time.deltaTime;
        if (timer > 0)
        {
            canvasGroup.alpha = timer / fadeDuration;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
