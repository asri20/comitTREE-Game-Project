using UnityEngine;
using TMPro;

public class TombolHuruf : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private AlphabetGame alphabetGameManager;
    private string hurufYangDibawa;

    void Awake()
    {
        // Mencari komponen teks di dalam tombol ini sendiri
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        
        // Mencari script utama AlphabetGame yang ada di scene
        alphabetGameManager = Object.FindFirstObjectByType<AlphabetGame>();

        if (textMeshPro != null)
        {
            // Mengambil karakter huruf yang tertulis di teks TMP (misal: "A", "B")
            hurufYangDibawa = textMeshPro.text.Trim();
        }
    }

    // Fungsi ini akan kita panggil lewat OnClick komponen Button
    public void SaatTombolDitekan()
    {
        if (alphabetGameManager != null)
        {
            // Kirim huruf yang di-klik ke script utama untuk dicek benar/salah
            alphabetGameManager.CekJawabanHuruf(hurufYangDibawa);
        }
        else
        {
            Debug.LogWarning("AlphabetGame manager tidak ditemukan di scene!");
        }
    }
}