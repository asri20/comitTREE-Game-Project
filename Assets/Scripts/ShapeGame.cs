using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ShapeGame : MonoBehaviour
{
    [System.Serializable]
    public struct DataBentuk
    {
        public string namaBentuk;       
        public AudioSource audioSoal;   
        public Sprite gambarBentuk;     
    }

    [Header("Referensi UI & Papan Menang")]
    public TextMeshProUGUI textInstruksiPapan; 
    public Image slotGambarBentukPapan;        
    public TextMeshProUGUI textSkor;           
    public GameObject panelMenangStage4; // 👈 Tarik Pop-Up Prefab kayu kamu ke sini di Inspector
    public GameObject karakterKatak;           

    [Header("Daftar 8 Bentuk Geometri")]
    public DataBentuk[] semuaBentuk;           

    [Header("Referensi Audio Gameplay")]
    public AudioSource audioLompatBenar;       
    public AudioSource audioByurSalah;         

    [Header("Referensi Audio Intro Koko")]
    public AudioSource introAudio1;            
    public AudioSource introAudio2;            
    public AudioSource introAudio3;            
    
    [Header("Pengaturan Game")]
    public int targetSkor = 8;
    
    private int indeksSoalSekarang;
    private string bentukTargetSekarang;
    private int skorSekarang = 0;
    private bool isIntroPlaying = true;
    private bool sedangMenunggu = false; // 🔒 KUNCI: Pengaman anti-spam klik saat audio diputar
    private Vector3 posisiAwalKatak;

    void Start()
    {
        if (audioLompatBenar != null) audioLompatBenar.playOnAwake = false;
        if (audioByurSalah != null) audioByurSalah.playOnAwake = false;
        if (introAudio1 != null) introAudio1.playOnAwake = false;
        if (introAudio2 != null) introAudio2.playOnAwake = false;
        if (introAudio3 != null) introAudio3.playOnAwake = false;

        // Pastikan panel menang tersembunyi di awal permainan
        if (panelMenangStage4 != null) 
        {
            panelMenangStage4.SetActive(false);
        }

        if (karakterKatak != null) {
            posisiAwalKatak = karakterKatak.transform.position;
        }

        UpdateTampilanSkor(); 
        StartCoroutine(PlayIntroThenStartGame());
    }

    IEnumerator PlayIntroThenStartGame()
    {
        isIntroPlaying = true;

        if (introAudio1 != null && introAudio1.clip != null) {
            introAudio1.Play();
            yield return new WaitForSeconds(introAudio1.clip.length + 0.3f);
        }
        if (introAudio2 != null && introAudio2.clip != null) {
            introAudio2.Play();
            yield return new WaitForSeconds(introAudio2.clip.length + 0.3f);
        }
        if (introAudio3 != null && introAudio3.clip != null) {
            introAudio3.Play();
            yield return new WaitForSeconds(introAudio3.clip.length);
        }

        isIntroPlaying = false;
        GenerateSoalBentuk();
    }

    public void GenerateSoalBentuk()
    {
        if (isIntroPlaying) return;
        if (semuaBentuk == null || semuaBentuk.Length == 0) return;

        // Mengembalikan posisi Koko Katak ke posisi awal
        if (karakterKatak != null) {
            karakterKatak.transform.position = posisiAwalKatak;
        }

        indeksSoalSekarang = Random.Range(0, semuaBentuk.Length);
        bentukTargetSekarang = semuaBentuk[indeksSoalSekarang].namaBentuk;

        if (textInstruksiPapan != null) {
            textInstruksiPapan.text = "Bentuk Apa?";
        }

        if (slotGambarBentukPapan != null && semuaBentuk[indeksSoalSekarang].gambarBentuk != null) {
            slotGambarBentukPapan.sprite = semuaBentuk[indeksSoalSekarang].gambarBentuk;
            slotGambarBentukPapan.gameObject.SetActive(true);
        }

        // Putar audio pertanyaan bentuk baru
        if (semuaBentuk[indeksSoalSekarang].audioSoal != null) {
            semuaBentuk[indeksSoalSekarang].audioSoal.playOnAwake = false;
            semuaBentuk[indeksSoalSekarang].audioSoal.Play();
        }

        sedangMenunggu = false; // Buka kembali kunci interaksi tombol teratai
    }

    public void CekJawabanBentuk(string namaBentukTombol, Vector3 posisiTeratai)
    {
        // Tolak klik jika intro masih berjalan atau sedang menjeda suara respons
        if (isIntroPlaying || sedangMenunggu) return;

        sedangMenunggu = true; // Kunci tombol seketika agar suara tidak tabrakan

        // Hentikan suara pertanyaan bentuk agar tidak menabrak suara benar/salah
        if (semuaBentuk[indeksSoalSekarang].audioSoal != null) {
            semuaBentuk[indeksSoalSekarang].audioSoal.Stop();
        }

        if (namaBentukTombol.ToUpper() == bentukTargetSekarang.ToUpper()) {
            skorSekarang++;
            UpdateTampilanSkor();
            
            PlaySound(audioLompatBenar);

            // Pindahkan Koko ke posisi daun teratai yang diklik
            if (karakterKatak != null) {
                karakterKatak.transform.position = posisiTeratai;
            }

            // Hitung durasi suara melompat benar secara otomatis
            float jedaSuaraBenar = 1.5f; 
            if (audioLompatBenar != null && audioLompatBenar.clip != null) {
                jedaSuaraBenar = audioLompatBenar.clip.length;
            }

            if (skorSekarang >= targetSkor) {
                // Tunggu suara lompat selesai penuh baru panggil papan menang kayu
                Invoke("MenangGame", jedaSuaraBenar);
            } else {
                // Tunggu suara lompat selesai + jeda nafas sedikit baru ganti soal baru
                Invoke("GenerateSoalBentuk", jedaSuaraBenar + 0.3f); 
            }
        } 
        else {
            PlaySound(audioByurSalah);

            // Hitung durasi suara byur basah secara otomatis
            float jedaSuaraSalah = 1.5f;
            if (audioByurSalah != null && audioByurSalah.clip != null) {
                jedaSuaraSalah = audioByurSalah.clip.length;
            }

            // Jalankan ulang suara pertanyaan awal setelah efek tercebur selesai berbunyi
            StartCoroutine(UlangiSuaraSoal(jedaSuaraSalah));
        }
    }

    IEnumerator UlangiSuaraSoal(float jedaWaktu)
    {
        yield return new WaitForSeconds(jedaWaktu); 
        if (semuaBentuk[indeksSoalSekarang].audioSoal != null && skorSekarang < targetSkor) {
            semuaBentuk[indeksSoalSekarang].audioSoal.Play();
        }
        sedangMenunggu = false; // Buka kembali kunci interaksi setelah suara soal diulang
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
            // Hentikan paksa audio efek gameplay lain agar tidak menumpuk berbarengan
            if (audioLompatBenar != null) audioLompatBenar.Stop();
            if (audioByurSalah != null) audioByurSalah.Stop();
            
            audioYangDiputar.Play();
        }
    }

    void MenangGame()
    {
        if (panelMenangStage4 != null) {
            panelMenangStage4.SetActive(true);
            Debug.Log("🔓 SUKSES: Papan kemenangan Stage 4 diaktifkan!");
        } else {
            // Jika lupa ditarik objeknya, langsung amankan lompat ke taman pusat
            PindahKeGardenDanTumbuhRimbun();
        }
    }

    // Fungsi utama yang diikatkan pada Event On Click tombol panah papan kayu baru kamu nanti
    public void PindahKeGardenDanTumbuhRimbun()
    {
        // 📈 SKEMA LOMPATAN MATEMATIKA: Setelah Stage 4 beres, pohon melompat naik ke LEVEL 8!
        PlayerPrefs.SetInt("LevelPohon", 8); 

        // Kirim sinyal 1 agar PohonManager di GardenScene tahu pohon harus memutar transisi Tumbuh_6ke8
        PlayerPrefs.SetInt("PohonTumbuh", 1); 
        PlayerPrefs.Save();

        Debug.Log("🏆 STAGE 4 SELESAI VIA BUTTON: Pohon diset ke Level 8 & Sinyal pertumbuhan aktif!");
        SceneManager.LoadScene("GardenScene"); 
    }
}