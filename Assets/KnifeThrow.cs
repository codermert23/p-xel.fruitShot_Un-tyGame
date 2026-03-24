using UnityEngine;

public class KnifeThrow : MonoBehaviour
{

    public SpriteRenderer glowRenderer;
    public float throwPower = 5f;
    private Vector2 dragStartPos;
    private Rigidbody2D rb;
    private Vector2 spawnPosition;
    private Quaternion spawnRotation;
    private bool isThrown = false; // Fırlatılma durumu
    public AudioClip okSesi; // Müfettiş (Inspector) panelinden sesi buraya koyacağız
    private AudioSource audioSource;
    // germe
    public float maxMesafe = 3f;
    private Color baslangicRengi = new Color(0, 1, 0, 0.6f); 
    private Color bitisRengi = new Color(1, 0, 0, 0.6f);
    public float titremeSiddeti= 0.1f;
    private Vector3 orijinalPozisyon;

    void Start()
    {
       
        rb = GetComponent<Rigidbody2D>();
        // İlk konumu hafızaya alıyoruz
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // Oyun başlar başlamaz çalmasın
    }
   
    void OnMouseDown()
    {
        if (!ScoreManager.oyunBasladi) return;
        if (isThrown) return; 
        dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
       
    }

    void OnMouseDrag()
    {
        if (!ScoreManager.oyunBasladi) return;
        if (isThrown) return;

        Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mevcutMesafe = Vector2.Distance(dragStartPos, currentPos);
        float oran = Mathf.Clamp01(mevcutMesafe / maxMesafe);

        if (glowRenderer != null)
        {
            glowRenderer.color = Color.Lerp(baslangicRengi, bitisRengi, oran);
        }


        Vector2 aimDirection = dragStartPos - currentPos;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

       
        if (oran >= 1.0f)
        {
            Vector3 sarsinti = Random.insideUnitCircle * titremeSiddeti;
            transform.position = (Vector3)spawnPosition + sarsinti;
        }
        else
        {
           
            transform.position = spawnPosition;
        }
    }

    void OnMouseUp()

    {
      
        if (okSesi != null)
        {
            audioSource.PlayOneShot(okSesi);
        }
        if (isThrown) return;
        Vector2 dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 throwDirection = dragStartPos - dragEndPos;
       
        if (throwDirection.magnitude > 0.5f)
        {
            isThrown = true; 
            rb.AddForce(throwDirection * throwPower, ForceMode2D.Impulse);
        }
        if (!isThrown) transform.position = spawnPosition;
        glowRenderer.color = baslangicRengi;
    }


    // Bıçak bir şeye çarptığında otomatik çalışır
    void OnCollisionEnter2D(Collision2D collision)
    {
        // GÜVENLİK KİLİDİ: Eğer bıçak fırlatılmadıysa çarpışmayı görmezden gel
        if (!isThrown) return;

        if (collision.gameObject.CompareTag("Target"))
        {
            collision.gameObject.GetComponent<TargetLog>().HitTarget();
            ResetKnife();
        }
    }

    // Işınlanma kodunu tek bir yerde topladık
    void ResetKnife()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        isThrown = false;
        glowRenderer.color = baslangicRengi;
      
    }

    void OnBecameInvisible()
    {
        if (isThrown)
        {
            ResetKnife();
        }
    }
}