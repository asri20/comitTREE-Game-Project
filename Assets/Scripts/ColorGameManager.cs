using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ColorGameManager : MonoBehaviour
{
    [Header("Pengaturan UI & Papan Menang")]
    public TextMeshProUGUI teksKoin; 
    public GameObject panelMenangStage2; // 👈 Tarik objek PopUp_SelesaiStage2 ke sini di Inspector

    [Header("Pengaturan Audio Kupu-Kupu")]
    public AudioSource audioSourceKupu;
    public AudioClip suaraPembuka;           
    public AudioClip[] suaraPertanyaanWarna; 
    public AudioClip suaraBenar;             
    public AudioClip suaraSalah;             

    [Header("Sistem Koin & Target")]
    private int jumlahKoin = 0;
    public int targetKoinPenuh = 8; 

    [HideInInspector]
    public string warnaTarget; 
    private bool sedangMenunggu = false; 

    void Start()
    {
        jumlahKoin = 0;
        UpdateTampilanKoin();

        // 🔒 PENGAMAN: Memastikan panel menang benar-benar mati di awal game
        if (panelMenangStage2 != null)
        {
            panelMenangStage2.SetActive(false);
        }
        
        if (suaraPembuka != null)
        {
            audioSourceKupu.clip = suaraPembuka;
            audioSourceKupu.Play();
            Invoke("MulaiPertanyaanAcak", 5f);
        }
        else
        {
            MulaiPertanyaanAcak();
        }
    }

    public void MulaiPertanyaanAcak()
    {
        if (suaraPertanyaanWarna.Length == 0) return;

        sedangMenunggu = false; 

        int indeksAcak = Random.Range(0, suaraPertanyaanWarna.Length);
        
        audioSourceKupu.clip = suaraPertanyaanWarna[indeksAcak];
        audioSourceKupu.Play();

        warnaTarget = suaraPertanyaanWarna[indeksAcak].name;
        Debug.Log("Pertanyaan baru: " + warnaTarget);
    }

    public void TebakWarnaBunga(string warnaBungaYangDiklik)
    {
        if (sedangMenunggu) return;

        if (warnaBungaYangDiklik.ToLower() == warnaTarget.ToLower())
        {
            Debug.Log("Jawaban Benar!");
            sedangMenunggu = true;
            
            audioSourceKupu.clip = suaraBenar;
            audioSourceKupu.Play();
            
            TambahKoin();
        }
        else
        {
            Debug.Log("Jawaban Salah!");
            sedangMenunggu = true;
            
            audioSourceKupu.clip = suaraSalah;
            audioSourceKupu.Play();
            
            KurangiKoin();
        }
    }

    void TambahKoin()
    {
        jumlahKoin++;
        UpdateTampilanKoin();

        if (jumlahKoin >= targetKoinPenuh)
        {
            // Game selesai, kunci status agar tidak bisa ngeklik bunga lagi
            sedangMenunggu = true; 

            // 🌟 LOGIKA BARU: Hitung durasi file audio suaraBenar secara dinamis
            float durasiJeda = 2.0f; // Default cadangan jika audio clip kosong
            if (suaraBenar != null)
            {
                durasiJeda = suaraBenar.length;
            }

            // Panggil fungsi pemuncul papan setelah suara benar selesai berputar sepenuhnya
            Invoke("MunculkanPanelMenang", durasiJeda);
        }
        else
        {
            // Jika belum penuh, tunggu suara benar selesai baru lanjut pertanyaan berikutnya
            float jedaPertanyaan = 2.5f;
            if (suaraBenar != null) jedaPertanyaan = suaraBenar.length + 0.5f;

            Invoke("MulaiPertanyaanAcak", jedaPertanyaan);
        }
    }

    // 🌟 FUNGSI BARU: Dipanggil otomatis oleh Invoke saat suara selebrasi selesai sunyi
    void MunculkanPanelMenang()
    {
        if (panelMenangStage2 != null)
        {
            panelMenangStage2.SetActive(true);
            Debug.Log("🔊 Suara selesai! Papan PopUp_SelesaiStage2 sekarang aktif di layar.");
        }
        else
        {
            // Antisipasi darurat jika lupa drag drop di Inspector
            PindahKeGarden();
        }
    }

    void KurangiKoin()
    {
        if (jumlahKoin > 0)
        {
            jumlahKoin--;
            UpdateTampilanKoin();
        }

        float jedaSalah = 2.5f;
        if (suaraSalah != null) jedaSalah = suaraSalah.length + 0.5f;

        Invoke("MulaiPertanyaanAcak", jedaSalah);
    }

    void UpdateTampilanKoin()
    {
        if (teksKoin != null)
        {
            teksKoin.text = "Koin: " + jumlahKoin + " / " + targetKoinPenuh;
        }
    }

    // --- FUNGSI PUBLIC: DIHUBUNGKAN PADA ON CLICK EVENT BUTTON PAPAN PANAH ---
    public void PindahKeGarden()
    {
        // --- INTEGRASI POHON OTOMATIS (STAGE 2) ---
        // Sesuai skema matematika puncak di pohon 9, Stage 2 memaksa level melompat ke Level 4
        PlayerPrefs.SetInt("LevelPohon", 4); 

        // Kirim sinyal 1 agar PohonManager di GardenScene tahu pohon harus memutar transisi Tumbuh_2ke4
        PlayerPrefs.SetInt("PohonTumbuh", 1); 
        PlayerPrefs.Save();

        Debug.Log("🏆 STAGE 2 SELESAI VIA BUTTON: Pohon diset ke Level 4 & Sinyal pertumbuhan aktif!");

        // Berpindah kembali ke stasiun utama taman
        SceneManager.LoadScene("GardenScene");
    }
}