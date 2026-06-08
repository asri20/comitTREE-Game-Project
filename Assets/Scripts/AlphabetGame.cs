using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class AlphabetGame : MonoBehaviour
{
    [System.Serializable]
    public struct DataAlfabet
    {
        public string namaBenda;      
        public Sprite gambarBenda;    
        public string hurufBenar;     
    }

    [Header("Referensi UI & Papan Menang")]
    public Image slotGambarPapan;     
    public TextMeshProUGUI textSkor; 
    public GameObject panelMenangStage3; // 👈 TAMBAHAN: Tarik Pop-Up Prefab kayu kamu ke sini

    [Header("Daftar Data Game")]
    public DataAlfabet[] semuaBenda;  

    [Header("Referensi Audio Gameplay")]
    public AudioSource audioBenar;
    public AudioSource audioSalah;

    [Header("Referensi Audio Intro Tupi")]
    public AudioSource introAudio1;   
    public AudioSource introAudio2;   
    public AudioSource introAudio3;   
    
    [Header("Pengaturan Game")]
    public int targetSkor = 8;        
    
    private int indeksSoalSekarang;
    private int skorSekarang = 0;
    private bool isIntroPlaying = true;
    private bool sedangMenungguSelesai = false; // 👈 KUNCI: Pengaman agar tidak bisa spam klik saat audio berputar

    void Start()
    {
        if (audioBenar != null) audioBenar.playOnAwake = false;
        if (audioSalah != null) audioSalah.playOnAwake = false;
        if (introAudio1 != null) introAudio1.playOnAwake = false;
        if (introAudio2 != null) introAudio2.playOnAwake = false;
        if (introAudio3 != null) introAudio3.playOnAwake = false;

        if (panelMenangStage3 != null) panelMenangStage3.SetActive(false);

        UpdateTampilanSkor(); 
        StartCoroutine(PlayIntroThenStartGame());
    }

    IEnumerator PlayIntroThenStartGame()
    {
        isIntroPlaying = true;

        if (introAudio1 != null && introAudio1.clip != null) {
            introAudio1.Play();
            yield return new WaitForSeconds(introAudio1.clip.length);
            yield return new WaitForSeconds(0.5f);
        }
        if (introAudio2 != null && introAudio2.clip != null) {
            introAudio2.Play();
            yield return new WaitForSeconds(introAudio2.clip.length);
            yield return new WaitForSeconds(0.5f);
        }
        if (introAudio3 != null && introAudio3.clip != null) {
            introAudio3.Play();
            yield return new WaitForSeconds(introAudio3.clip.length);
        }

        isIntroPlaying = false;
        GenerateSoalBaru();
    }

    public void GenerateSoalBaru()
    {
        if (isIntroPlaying) return;
        if (semuaBenda == null || semuaBenda.Length == 0) return;

        sedangMenungguSelesai = false; // Buka kunci interaksi tombol kembali

        indeksSoalSekarang = Random.Range(0, semuaBenda.Length);

        if (slotGambarPapan != null && semuaBenda[indeksSoalSekarang].gambarBenda != null) {
            slotGambarPapan.sprite = semuaBenda[indeksSoalSekarang].gambarBenda;
            slotGambarPapan.gameObject.SetActive(true);
        }
    }

    public void CekJawabanHuruf(string hurufTombol)
    {
        // Tolak input jika intro sedang main atau sistem sedang menjeda audio selebrasi
        if (isIntroPlaying || sedangMenungguSelesai) return;

        string jawabanBenar = semuaBenda[indeksSoalSekarang].hurufBenar;

        if (hurufTombol.ToUpper() == jawabanBenar.ToUpper()) {
            skorSekarang++; 
            UpdateTampilanSkor();
            
            sedangMenungguSelesai = true; // Kunci tombol agar tidak tumpang tindih
            PlaySound(audioBenar);

            // Hitung durasi audio benar secara dinamis
            float jedaSuara = 2.0f;
            if (audioBenar != null && audioBenar.clip != null) {
                jedaSuara = audioBenar.clip.length;
            }

            if (skorSekarang >= targetSkor) {
                // Tunggu suara selebrasi selesai, baru panggil panel menang
                Invoke("MenangGame", jedaSuara);
            } else {
                // Tunggu suara selebrasi selesai, baru ganti soal baru biar tidak tabrakan
                Invoke("GenerateSoalBaru", jedaSuara + 0.3f); 
            }
        } 
        else {
            sedangMenungguSelesai = true;
            PlaySound(audioSalah);

            float jedaSalah = 1.5f;
            if (audioSalah != null && audioSalah.clip != null) {
                jedaSalah = audioSalah.clip.length;
            }
            // Reset status tunggu setelah efek suara salah selesai berbunyi
            Invoke("BukaKunciTombol", jedaSalah);
        }
    }

    void BukaKunciTombol()
    {
        sedangMenungguSelesai = false;
    }

    void UpdateTampilanSkor()
    {
        if (textSkor != null) {
            textSkor.text = "Koin: " + skorSekarang + " / " + targetSkor;
        }
    }

    void PlaySound(AudioSource audioYangDiputar)
    {
        if (audioYangDiputar != null) {
            // Hentikan paksa audio gameplay apa pun yang sedang berjalan di channel ini agar tidak tabrakan
            if (audioBenar != null) audioBenar.Stop();
            if (audioSalah != null) audioSalah.Stop();
            
            audioYangDiputar.Play();
        }
    }

    void MenangGame()
    {
        // Tampilkan pop-up selebrasi seperti di Stage 2 jika dipasang di Inspector
        if (panelMenangStage3 != null) {
            panelMenangStage3.SetActive(true);
        } else {
            // Jika tidak ada panel, langsung lempar darurat ke taman pusat
            PindahKeGardenDanTumbuh();
        }
    }

    // Fungsi public yang ditempel di On Click Button Papan Panah Kayu Stage 3 kamu nanti
    public void PindahKeGardenDanTumbuh()
    {
        // 📈 SKEMA MATEMATIKA: Setelah Stage 3 sukses, pohon melompat naik ke LEVEL 6!
        PlayerPrefs.SetInt("LevelPohon", 6); 
        PlayerPrefs.SetInt("PohonTumbuh", 1); // Aktifkan sinyal agar memutar animasi Tumbuh_4ke6
        PlayerPrefs.Save();

        Debug.Log("🏆 STAGE 3 SELESAI: Pohon melompat menuju Level 6!");
        SceneManager.LoadScene("GardenScene"); 
    }
}