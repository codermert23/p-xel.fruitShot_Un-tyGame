using UnityEngine;
using TMPro; // TextMeshPro için gerekli

public class FloatingTextControl : MonoBehaviour
{
    public float moveSpeed = 2f; // Yukarý süzülme hýzý
    public float fadeDuration = 1f; // Saydamlaþma süresi
    public float destroyTime = 1.2f; // Objenin tamamen yok olma süresi

    private TextMeshProUGUI textMesh; // Yazý bileþeni referansý
    private Color originalColor;
    private float timer;

    void Start()
    {
        // Ebeveyn obje veya çocuk obje üzerindeki TextMeshPro componentini buluyoruz
        textMesh = GetComponentInChildren<TextMeshProUGUI>();

        if (textMesh != null)
        {
            originalColor = textMesh.color;
        }

        // Obje yaratýldýktan kýsa süre sonra kendini yok edecek
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // 1. Yazýyý her karede biraz yukarý kaydýr
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // 2. Zamanla saydamlaþtýrýp yok edelim
        timer += Time.deltaTime;
        if (textMesh != null)
        {
            // Saydamlýk (Alpha) deðerini zamanla düþürüyoruz
            float alpha = 1f - Mathf.Clamp01(timer / fadeDuration);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
    }
}
