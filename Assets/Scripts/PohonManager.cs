using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PohonManager : MonoBehaviour
{
    private Animator anim;

    [Header("Referensi UI Sprout & Tombol")]
    public GameObject panelDialogSprout;    // Tarik objek DialogSprout_Panel ke sini
    public TextMeshProUGUI txtDialogSprout;  // Tarik objek Txt_DialogSprout ke sini
    public TextMeshProUGUI textTombolLanjut; // Tarik komponen Text (TMP) milik tombol ke sini
    public AudioSource audioSproutSelamat;   // Tarik objek Audio_SproutSelamat ke sini

    private string sceneTujuanBerikutnya;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("⚠️ ERROR: Komponen Animator tidak ditemukan di comitTREE_Controller!");
        }

        // Sembunyikan panel di awal agar tidak berkedip sebelum animasi mekar selesai
        if (panelDialogSprout != null)
        {
            panelDialogSprout.SetActive(false);
        }

        // Jalankan kalkulasi visual awal secara jujur berdasarkan progress bermain
        HitungProgresStageDanScene();
        UpdatePohonVisual();
    }

    void HitungProgresStageDanScene()
    {
        int levelSekarang = PlayerPrefs.GetInt("LevelPohon", 1);

        // --- BERPINDAH DINAMIS & OTOMATISASI TEKS DIALOG SAMA TOMBOL ---
        if (levelSekarang <= 1)
        {
            sceneTujuanBerikutnya = "Stage1_NumberOrchard";
            SetTeksDialogDanTombol("Selamat! Petualangan dimulai! Ayo bantu Sprout di Stage 1 menghitung buah-buahan!", "Main Stage 1");
        }
        else if (levelSekarang == 2)
        {
            sceneTujuanBerikutnya = "Stage2_ColorFlower"; 
            SetTeksDialogDanTombol("Selamat! Pohon kamu sudah tumbuh! Yuk, lanjut petualangan kita ke Stage 2 untuk tebak warna!", "Lanjut ke Stage 2");
        }
        else if (levelSekarang == 4)
        {
            sceneTujuanBerikutnya = "Stage3_Alphabet Canopy"; 
            SetTeksDialogDanTombol("Wah hebat! Pohonnya berbunga sangat cantik! Mari kita bantu merawat daunnya sambil belajar huruf di Stage 3!", "Lanjut ke Stage 3");
        }
        else if (levelSekarang == 6)
        {
            sceneTujuanBerikutnya = "Stage4_Kolam Teratai Geometri"; 
            SetTeksDialogDanTombol("Luar biasa! Daun pohon kita semakin rimbun! Yuk, kita hias area kolam dan belajar bentuk di Stage 4!", "Lanjut ke Stage 4");
        }
        else if (levelSekarang == 8)
        {
            // 🎯 FIXED: Ketika level = 8, ini artinya kamu baru saja menang Stage 4!
            // Kita arahkan tombol ke Stage 5 Labirin secara benar.
            sceneTujuanBerikutnya = "Stage5_Magical Forest Maze"; 
            SetTeksDialogDanTombol("Keren sekali! Taman kita sudah hampir sempurna! Selesaikan tantangan terakhir di Stage 5 mencari jalan keluar!", "Lanjut ke Stage 5");
        }
        else if (levelSekarang >= 9)
        {
            // 🌸 KUNCI TAMAT MUTLAK: Hanya aktif jika benar-benar lolos dari Stage 5 (Level 9)!
            sceneTujuanBerikutnya = "MainMenu"; 
            SetTeksDialogDanTombol("Horeee! Pohon comitTREE kita sudah tumbuh menjadi pohon raksasa yang megah! Terima kasih ya, anak hebat!", "Kembali ke Menu");
        }
    }

    // Fungsi pembantu untuk set kedua teks sekaligus agar kodingan lebih rapi
    void SetTeksDialogDanTombol(string dialogSprout, string pesanTombol)
    {
        if (txtDialogSprout != null) { txtDialogSprout.text = dialogSprout; }
        if (textTombolLanjut != null) { textTombolLanjut.text = pesanTombol; }
    }

    void UpdatePohonVisual()
    {
        int levelSekarang = PlayerPrefs.GetInt("LevelPohon", 1);
        int baruMenang = PlayerPrefs.GetInt("PohonTumbuh", 0);

        // Menghitung level sebelumnya agar pas memanggil State lompatan di Animator
        int levelSebelumnya = 1;
        if (levelSekarang == 4) levelSebelumnya = 2;
        else if (levelSekarang == 6) levelSebelumnya = 4;
        else if (levelSekarang == 8) levelSebelumnya = 6;
        else if (levelSekarang == 9) levelSebelumnya = 8; 
        else levelSebelumnya = levelSekarang - 1;

        if (levelSebelumnya < 1) levelSebelumnya = 1;

        if (anim != null)
        {
            if (baruMenang == 1 && levelSekarang > 1)
            {
                // 🔥 TRANSISI SINKRONISASI MEKAR FINAL STAGE 5 (KE LEVEL 9 RAKSASA)
                if (levelSekarang == 9)
                {
                    SetStatusSatuObjekAnakSaja(7); // Tampilkan Pohon Level 8 (Index 7) dulu sebagai modal awal mekar
                    anim.Play("Idle8");           // Standby tenang di posisi Idle8
                    anim.SetTrigger("Tumbuh");    // Pemicu gerbang panah menggunakan parameter Trigger 'Tumbuh' pilihan Asri
                }
                else
                {
                    // Untuk Stage 1 sampai Stage 4: Tetap gunakan sistem panggil nama string bawaanmu yang sudah rapi
                    SetStatusSatuObjekAnakSaja(levelSebelumnya - 1);
                    string namaAnimasiTumbuh = "Tumbuh_" + levelSebelumnya + "ke" + levelSekarang;
                    anim.Play(namaAnimasiTumbuh);
                }

                // Jalankan Coroutine penunggu durasi mekar pohon
                StartCoroutine(TungguPohonMekarLaluSproutNgomong(levelSekarang));

                // Reset status menang agar tidak looping animasi tumbuh berulang kali saat bolak-balik scene kebun
                PlayerPrefs.SetInt("PohonTumbuh", 0);
                PlayerPrefs.Save();
            }
            else
            {
                // Memanggil status standby biasa (Idle) jika hanya berkunjung ke kebun
                SetStatusSatuObjekAnakSaja(levelSekarang - 1);

                string namaAnimasiIdle = "Idle" + levelSekarang;
                anim.Play(namaAnimasiIdle);
                
                if (panelDialogSprout != null)
                {
                    panelDialogSprout.SetActive(true);
                }
            }
        }
    }

    void SetStatusSatuObjekAnakSaja(int indexYangAktif)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject pohonAnak = transform.GetChild(i).gameObject;
            pohonAnak.SetActive(i == indexYangAktif);
        }
    }

    IEnumerator TungguPohonMekarLaluSproutNgomong(int levelTargetBaru)
    {
        // Jeda durasi mekar pohon sesuai panjang timeline animasi masing-masing
        float jedaMekar = 2.5f;
        if (levelTargetBaru == 9) jedaMekar = 3.5f; 

        yield return new WaitForSeconds(jedaMekar); 

        // Setelah klip mekar selesai diputar penuh, barulah aktifkan wujud pohon rimbun yang baru!
        SetStatusSatuObjekAnakSaja(levelTargetBaru - 1);
        string namaAnimasiIdleBaru = "Idle" + levelTargetBaru;
        if (anim != null) anim.Play(namaAnimasiIdleBaru);

        // 🔥 HITUNG ULANG PROGRES DI DETIK INI:
        // Memaksa Sprout memperbarui balon teksnya secara real-time mengikuti angka level target baru
        HitungProgresStageDanScene();

        if (panelDialogSprout != null)
        {
            panelDialogSprout.SetActive(true);
        }

        if (audioSproutSelamat != null)
        {
            audioSproutSelamat.Play();
        }
    }

    public void KlikTombolLanjutStage()
    {
        SceneManager.LoadScene(sceneTujuanBerikutnya); 
    }
}