using UnityEngine;

public class TrashInteract : Interactable
{
    [Header("Trash Sprites")]
    public Sprite dirtySprite;    // Sprite sampah kotor
    public Sprite cleanSprite;    // Sprite setelah dibersihkan

    [Header("Floating Text")]
    public GameObject floatingTextPrefab; // prefab text "+1pt"

    private SpriteRenderer spriteRenderer;
    public bool IsClean { get; private set; } = false; // public read-only

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        IsClean = false;

        if (spriteRenderer != null && dirtySprite != null)
            spriteRenderer.sprite = dirtySprite;
    }

    public override void Interact()
    {
        if (!IsClean)
        {
            CleanTrash();
        }
    }

    public void CleanTrash()
    {
        if (IsClean) return; // jangan double

        IsClean = true;

        if (spriteRenderer != null && cleanSprite != null)
            spriteRenderer.sprite = cleanSprite;

        // Tambah skor ke UIManager
        if (UIManager.Instance != null)
            UIManager.Instance.AddCleaned(1);

        // Spawn floating text
        if (floatingTextPrefab != null)
        {
            // spawn sedikit di atas posisi sampah
            Vector3 spawnPos = transform.position + Vector3.up * 1f;
            GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);

            // biar tidak ikut-ikut hierarchy sampah
            textObj.transform.SetParent(null);
        }
    }

    public void ResetTrash()
    {
        IsClean = false;

        if (spriteRenderer != null && dirtySprite != null)
            spriteRenderer.sprite = dirtySprite;
    }
}
