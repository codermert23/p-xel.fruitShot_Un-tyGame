using UnityEngine;
using UnityEngine.SceneManagement; // Sahneler arasý geçiþ için þart!

public class MenuKontrol : MonoBehaviour
{
    public void OyunaBasla()
    {
        // Týrnak içindeki kýsma, kendi esas oyun sahnelerinin tam adýný yazmalýsýn!
        SceneManager.LoadScene("SampleScene");
    }

    public void OyundanCik()
    {
        Application.Quit();
        Debug.Log("Oyundan Çýkýldý!"); // Editörde çalýþtýðýný görmek için
    }
}