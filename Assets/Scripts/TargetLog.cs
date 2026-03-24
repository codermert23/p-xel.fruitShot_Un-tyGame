using System.Collections;
using UnityEngine;

public class TargetLog : MonoBehaviour
{
    public GameObject dumanEfektiPrefab;
    public GameObject plusTenPrefab;
    public AudioClip patlamaSesi;
    public AudioClip hataSesi;

    public float ekrandaKalmaSuresi = 4f; // respawn süresi
    private float zamanSayaci = 0f;

    [Header("Meyve Çeþitleri")]
    public Sprite[] meyveResimleri;
    private int guncelPuan;
    private int benimindex;
    private SpriteRenderer sr;
    private Collider2D col;
    private Camera mainCam;
    private AudioSource audioSource;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        mainCam = Camera.main;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        RastgeleMeyveSec();
    }
    void Update()
    {
        
        if (sr.enabled)
        {
            zamanSayaci += Time.deltaTime;
            if (zamanSayaci >= ekrandaKalmaSuresi)
            {
                zamanSayaci = 0f; // Sayacý sýfýrla

                
                Vector2 camPos = mainCam.transform.position;
                float camHeight = mainCam.orthographicSize;
                float camWidth = camHeight * mainCam.aspect;

                float randomX = Random.Range(camPos.x - camWidth + 0.5f, camPos.x + camWidth - 0.5f);
                float randomY = Random.Range(camPos.y, camPos.y + camHeight - 0.5f);

                transform.position = new Vector3(randomX, randomY, transform.position.z);
                RastgeleMeyveSec();
            }
        }
    }
    void RastgeleMeyveSec()
    {
        if (meyveResimleri !=null && meyveResimleri.Length>0)
        {
            
            benimindex= Random.Range(0, meyveResimleri.Length);
            sr.sprite = meyveResimleri[benimindex];
            if (benimindex == ScoreManager.yasakliIndex)
            {
                guncelPuan = -15;
            }
            else if (benimindex<10)
            {
                guncelPuan = 10;
            }else
            {
                guncelPuan = 5;
            }
            Destroy(GetComponent<PolygonCollider2D>());
            col = gameObject.AddComponent<PolygonCollider2D>();

        }

    }

    public void HitTarget()
    {
      
        if (plusTenPrefab != null){
            Vector3 spawnPos = transform.position + Vector3.up * 1f;
            Instantiate(plusTenPrefab, spawnPos, Quaternion.identity); }


        if (ScoreManager.instance != null){          
            ScoreManager.instance.AddScore(guncelPuan);
        }
        else{
            Debug.LogWarning("ScoreManager bulunamadý! Puan eklenemedi.");
        }       
        StartCoroutine(RespawnRoutine()); 
}

    IEnumerator RespawnRoutine()
    {
        
        if (guncelPuan < 0){ if (hataSesi != null) audioSource.PlayOneShot(hataSesi); }
        else if (patlamaSesi != null){ audioSource.PlayOneShot(patlamaSesi); }

        sr.enabled = false;
        col.enabled = false;
       
        Vector3 efektKonumu = new Vector3(transform.position.x, transform.position.y, -1f);

        if (dumanEfektiPrefab != null){
            GameObject duman = Instantiate(dumanEfektiPrefab, efektKonumu, Quaternion.identity);
            Destroy(duman, 1f);
        }

        if (plusTenPrefab != null){
            
            Vector3 zorlaOneCek = new Vector3(transform.position.x, transform.position.y, -9f);
            GameObject artis = Instantiate(plusTenPrefab, zorlaOneCek, Quaternion.identity);

            var yaziBileseni = artis.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            // Eðer ceza yemiþsek eksi yazsýn ve rengi kýzarsýn
            if (guncelPuan < 0){
                yaziBileseni.text = guncelPuan.ToString(); 
                yaziBileseni.color = Color.red;
            }else{
                yaziBileseni.text = "+" + guncelPuan;
                yaziBileseni.color = Color.green;
            }
            artis.transform.localScale = Vector3.one * 5f;
            Destroy(artis, 1f);
        }

        yield return new WaitForSeconds(ekrandaKalmaSuresi);

     
        Vector2 camPos = mainCam.transform.position;
        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        float randomX = Random.Range(camPos.x - camWidth + 0.5f, camPos.x + camWidth - 0.5f);
        float randomY = Random.Range(camPos.y, camPos.y + camHeight - 0.5f);

        transform.position = new Vector3(randomX, randomY, transform.position.z);
        RastgeleMeyveSec();
        zamanSayaci = 0f;
        sr.enabled = true;
        col.enabled = true;
    }
}