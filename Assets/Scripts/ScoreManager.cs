  using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TMP kullanýyoruz

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // Diðer kodlardan kolayca ulaþmak için (Singleton)
    public TextMeshProUGUI scoreTextDisplay;


    [Header("Baþlangýç Sekansý")]

    public GameObject baslangicPaneli;
    public Image buyukYasakliMeyve;
    public TextMeshProUGUI sayacYazisi;
    public TextMeshProUGUI yasakYazisi;
    public static bool oyunBasladi = false;
    public AudioClip geriSayimSesi;

    [Header("Yasaklý Meyve Sistemi")]
    public Image yasakliMeyveGorseli; // meyvenin resmi
    public Sprite[] tumMeyveResimleri;
    public static int yasakliIndex;

    [Header("Süre ve Adrenalin Sistemi")]
    public float macSuresi = 30f;
    public TextMeshProUGUI sureYazisi;
    public AudioClip bipSesi;

    private float kalanSure;
    private bool gerilimSesiCalindi = false;
    private AudioSource audioSource;

    [Header("Oyun Sonu Sistemi")]
    public GameObject oyunBittiPaneli;
    public TextMeshProUGUI oyunBittiYazisi;
    public TextMeshProUGUI finalSkorYazisi;
    public AudioClip bitirmeSesi;

    private int totalScore = 0; // Toplam puanýmýzý tutan deðiþken

    void Awake()
    {
       
        if (instance == null)
        {
            instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        kalanSure = macSuresi;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        Time.timeScale = 0f;
        oyunBasladi = false;

        yasakliIndex = Random.Range(0, tumMeyveResimleri.Length);

        if (yasakliMeyveGorseli != null && tumMeyveResimleri.Length > 0)
        {
            yasakliMeyveGorseli.sprite = tumMeyveResimleri[yasakliIndex];
        }
        if (buyukYasakliMeyve != null) buyukYasakliMeyve.sprite = tumMeyveResimleri[yasakliIndex];

        UpdateScoreUI();
        StartCoroutine(BaslangicSayaci());
    }
    IEnumerator BaslangicSayaci()
    {
        baslangicPaneli.SetActive(true);
        if (geriSayimSesi != null) audioSource.PlayOneShot(geriSayimSesi);

        for (int i = 3; i > 0; i--)
        {          
            sayacYazisi.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        sayacYazisi.text = "BASLA!";
        yield return new WaitForSecondsRealtime(0.5f);

       
        baslangicPaneli.SetActive(false);
        Time.timeScale = 1f;
        oyunBasladi = true;
    }
    public void AddScore(int amount) {
        totalScore += amount;    
        UpdateScoreUI();
    }
    void OyunBittiSekansi()
    {
        oyunBittiPaneli.SetActive(true);
        finalSkorYazisi.text = "SKORUN: " + totalScore.ToString(); // Kendi puan deðiþkenini yaz

        // Zaman donukken çalýþan özel animasyonu tetikle
        StartCoroutine(YaziAnimasyonu());      
    }
    IEnumerator YaziAnimasyonu()
    {
        float gecenSure = 0f;
        Vector3 orijinalBoyut = oyunBittiYazisi.transform.localScale;
        Vector3 buyukBoyut = orijinalBoyut * 1.25f; // Yazý %50 büyüyecek

        // 2 saniye boyunca kalbi attýrýyoruz
        while (gecenSure < 2f)
        {
            // timeScale 0 olduðu için unscaledDeltaTime KULLANMAK ZORUNDAYIZ!
            gecenSure += Time.unscaledDeltaTime;

            // PingPong metodu deðeri 0 ile 1 arasýnda yumuþakça git-gel yaptýrýr
            float oran = Mathf.PingPong(gecenSure * 3f, 1f);

            oyunBittiYazisi.transform.localScale = Vector3.Lerp(orijinalBoyut, buyukBoyut, oran);

            yield return null;
        }

        // 2 saniye bitince yazýyý orijinal boyuta sabitle
        oyunBittiYazisi.transform.localScale = orijinalBoyut;
    }
    void Update()
    {
       
        if (oyunBasladi && kalanSure > 0)
        {
            kalanSure -= Time.deltaTime; 

           
            if (kalanSure <= 0)
            {
                kalanSure = 0;
                oyunBasladi = false;
                Time.timeScale = 0f; 
                Debug.Log("SÜRE BÝTTÝ!");
                audioSource.PlayOneShot(bitirmeSesi);
                OyunBittiSekansi();
            }

      
            int guncelSaniye = Mathf.CeilToInt(kalanSure);
            sureYazisi.text = guncelSaniye.ToString();

            
            if (guncelSaniye <= 10 && guncelSaniye > 0)
            {
                sureYazisi.color = Color.red; // Rengi kan kýrmýzý yap

               
                if (!gerilimSesiCalindi)
                {
                    if (bipSesi != null) audioSource.PlayOneShot(bipSesi);
                    gerilimSesiCalindi = true; 
                }
            }
            else
            {
                sureYazisi.color = Color.white;
            }
        }
    }
    private void UpdateScoreUI()
    {
       
        if (scoreTextDisplay != null)
        {
            scoreTextDisplay.text = "Puan: " + totalScore;
        }
        else
        {
            
            Debug.LogWarning("ScoreManager: scoreTextDisplay (Puan Yazýsý) kutusu boþ! Puan eklenemez.");
        }
    }
}