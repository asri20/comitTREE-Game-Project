using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; // Wajib untuk Coroutine

public class NumberGame : MonoBehaviour
{
    [Header("Referensi Objek")]
    public GameObject apelPrefab;
    public Transform papanAngka;
    public TextMeshProUGUI textSkor; 

    private GameObject popUpButtonPohon; // Script akan mencari otomatis di dalam Canvas

    [Header("Referensi Audio Gameplay")]
    public AudioSource audioBenar;
    public AudioSource audioSalah;
    public AudioSource audioMenangSprout; // Untuk suara selebrasi / ajakan Sprout

    [Header("Referensi Audio Intro Owel")]
    public AudioSource introAudio1; // Sapaan Awal
    public AudioSource introAudio2; // Instruksi Utama
    public AudioSource introAudio3; // Peringatan Angka Acak
    
    [Header("Pengaturan Game")]
    public int targetSkor = 10;
    
    private int jumlahApelBenar;
    private int skorSekarang = 0;
    private bool isIntroPlaying = true; // Kunci permainan selama intro
    private bool isGameSelesai = false;  // Kunci pengaman agar tidak bentrok saat menang

    void Start()
    {
        // --- PROSES OTOMATIS: Mencari objek Canvasgame1 terlebih dahulu ---
        GameObject canvasGame = GameObject.Find("Canvasgame1");
        
        if (canvasGame != null) {
            // Menggunakan Transform.Find karena bisa mendeteksi objek anak yang sedang MATI/NONAKTIF
            Transform findTombol = canvasGame.transform.Find("PopUp_BtnPohon");
            if (findTombol != null) {
                popUpButtonPohon = findTombol.gameObject;
                popUpButtonPohon.SetActive(false); // Sembunyikan dengan aman di awal game
                Debug.Log("Sistem: Objek 'PopUp_BtnPohon' berhasil ditemukan dan disembunyikan.");
            } else {
                Debug.LogError("⚠️ ERROR: Tidak menemukan objek bernama 'PopUp_BtnPohon' di dalam Canvasgame1!");
            }
        } else {
            Debug.LogError("⚠️ ERROR: Tidak menemukan Canvas bernama 'Canvasgame1' di Hierarchy!");
        }

        // Pengaman agar tidak terjadi NullReferenceException jika ada komponen yang lupa dipasang
        if (audioBenar != null) audioBenar.playOnAwake = false;
        if (audioSalah != null) audioSalah.playOnAwake = false;
        if (audioMenangSprout != null) audioMenangSprout.playOnAwake = false; // Pengaman audio baru
        if (introAudio1 != null) introAudio1.playOnAwake = false;
        if (introAudio2 != null) introAudio2.playOnAwake = false;
        if (introAudio3 != null) introAudio3.playOnAwake = false;

        isGameSelesai = false;
        skorSekarang = 0;
        UpdateTampilanSkor(); 
        
        // Mulai jalankan sekuens penjelasan Owel
        StartCoroutine(PlayIntroThenStartGame());
    }

    IEnumerator PlayIntroThenStartGame()
    {
        isIntroPlaying = true;

        if (introAudio1 != null && introAudio1.clip != null) {
            introAudio1.Play();
            yield return new WaitForSeconds(introAudio1.clip.length);
        } else {
            yield return null;
        }

        if (introAudio2 != null && introAudio2.clip != null) {
            introAudio2.Play();
            yield return new WaitForSeconds(introAudio2.clip.length);
        } else {
            yield return null;
        }

        if (introAudio3 != null && introAudio3.clip != null) {
            introAudio3.Play();
            yield return new WaitForSeconds(introAudio3.clip.length);
        } else {
            yield return null;
        }

        isIntroPlaying = false;
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        if (isIntroPlaying || isGameSelesai) return;
        if (papanAngka == null) return;

        foreach (Transform child in papanAngka) {
            Destroy(child.gameObject);
        }

        jumlahApelBenar = Random.Range(1, 11);

        for (int i = 0; i < jumlahApelBenar; i++) {
            if (apelPrefab != null) {
                Instantiate(apelPrefab, papanAngka);
            }
        }
    }

    public void CekJawaban(int angkaTombol)
    {
        if (isIntroPlaying || isGameSelesai) return;

        if (audioBenar != null) { audioBenar.Stop(); }
        if (audioSalah != null) { audioSalah.Stop(); }

        if (angkaTombol == jumlahApelBenar) {
            skorSekarang++; 
            PlaySound(audioBenar);
            UpdateTampilanSkor();

            if (skorSekarang >= targetSkor) {
                MenangGame();
            } else {
                GenerateLevel(); 
            }
        } 
        else {
            if (skorSekarang > 0) {
                skorSekarang--; 
            }
            PlaySound(audioSalah);
            UpdateTampilanSkor();
        }
    }

    void UpdateTampilanSkor()
    {
        if (textSkor != null) {
            textSkor.text = skorSekarang + " / " + targetSkor;
        }
    }

    void PlaySound(AudioSource audioYangDiputar)
    {
        if (audioYangDiputar != null && audioYangDiputar.clip != null) {
            audioYangDiputar.Stop(); 
            audioYangDiputar.Play();
        }
    }

    void MenangGame()
    {
        isGameSelesai = true; 
        isIntroPlaying = true; 

        // --- SOLUSI LOMPATAN LEVEL DINAMIS (Puncak di Pohon 9) ---
        // Karena ini Stage 1, setelah menang kita paksa pohon naik ke Level 2
        PlayerPrefs.SetInt("LevelPohon", 2);

        // Kirim sinyal bahwa pohon berhak tumbuh pakai animasi transisi di GardenScene
        PlayerPrefs.SetInt("PohonTumbuh", 1); 
        PlayerPrefs.Save();
        
        Debug.Log("🏆 STAGE 1 MENANG: Pohon diset ke Level 2 & Sinyal pertumbuhan aktif!");

        // 1. Munculkan tombol pop-up ke layar secara instan
        if (popUpButtonPohon != null) {
            popUpButtonPohon.SetActive(true);
        }

        // 2. SISTEM AUDIO: Hentikan audio gameplay lama, lalu putar suara Sprout
        if (audioBenar != null) { audioBenar.Stop(); }
        if (audioSalah != null) { audioSalah.Stop(); }

        if (audioMenangSprout != null) {
            audioMenangSprout.Play();
            Debug.Log("Sistem: Memutar audio selebrasi Sprout!");
        }
    }

    public void KlikTombolPindahScene()
    {
        // Berpindah menuju stasiun pusat utama taman
        SceneManager.LoadScene("GardenScene"); 
    }
}