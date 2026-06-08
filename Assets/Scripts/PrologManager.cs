using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PrologManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI teksDialog;   // Tarik objek Teks dari dalam papanteks ke sini
    public Button tombolLanjut;         // Tarik objek tombol Lanjut ke sini

    [Header("Audio Components")]
    public AudioSource audioSource;     // Tarik komponen AudioSource ke sini
    public AudioClip[] suaraProlog;     // Masukkan 3 file rekaman suara Sprout di sini

    [Header("Settings")]
    [TextArea(3, 5)]
    public string[] daftarCerita;       // Cerita prolog (diisi via Inspector)
    public float kecepatanKetik = 0.04f; // Kecepatan memunculkan huruf

    private int indexHalaman = 0;
    private bool sedangNgetik = false;

    void Start()
    {
        // Hubungkan tombol secara otomatis ke fungsi TeksBerikutnya saat diklik
        if (tombolLanjut != null)
        {
            tombolLanjut.onClick.AddListener(TeksBerikutnya);
        }

        // Siapkan teks cerita prolog langsung dari script agar rapi
        DaftarkanCeritaProlog();

        // Mulai jalankan prolog halaman pertama
        MulaiProlog();
    }

    void DaftarkanCeritaProlog()
    {
        daftarCerita = new string[3];
        daftarCerita[0] = "Hai, Teman Pintar! Aku Sprout. Selamat datang di Hutan comitTREE!";
        daftarCerita[1] = "Lihat, Pohon Ajaib kita sedang layu. Ia butuh bantuanmu untuk tumbuh besar lagi.";
        daftarCerita[2] = "Ayo bermain dan selesaikan tantangannya agar pohon kita tumbuh tinggi. Selamat belajar!";
    }

    void MulaiProlog()
    {
        indexHalaman = 0;
        if (daftarCerita.Length > 0)
        {
            StartCoroutine(KetikTeks(daftarCerita[indexHalaman]));
            PutarSuara(indexHalaman);
        }
    }

    IEnumerator KetikTeks(string kalimat)
    {
        sedangNgetik = true;
        teksDialog.text = ""; // Kosongkan papan teks di awal

        // Efek typewriter: Munculkan huruf satu per satu
        foreach (char huruf in kalimat.ToCharArray())
        {
            teksDialog.text += huruf;
            yield return new WaitForSeconds(kecepatanKetik);
        }

        sedangNgetik = false;
    }

    void PutarSuara(int halaman)
    {
        // Pastikan komponen audio dan file suaranya sudah dimasukkan di Inspector
        if (audioSource != null && suaraProlog != null && halaman < suaraProlog.Length)
        {
            if (suaraProlog[halaman] != null)
            {
                audioSource.clip = suaraProlog[halaman];
                audioSource.Play();
            }
        }
    }

    public void TeksBerikutnya()
    {
        // Jika pemain mengklik saat teks masih mengetik, langsung munculkan semua teks halaman itu
        if (sedangNgetik)
        {
            StopAllCoroutines();
            teksDialog.text = daftarCerita[indexHalaman];
            sedangNgetik = false;
            return;
        }

        // Pindah ke halaman berikutnya
        indexHalaman++;

        if (indexHalaman < daftarCerita.Length)
        {
            StartCoroutine(KetikTeks(daftarCerita[indexHalaman]));
            PutarSuara(indexHalaman);
        }
        else
        {
            // Jika penjelasan sudah halaman terakhir dan diklik lagi, papan penjelasan tertutup
            Debug.Log("Penjelasan game selesai!");
            
            // Opsional: Matikan papan teks agar pemain bisa langsung klik "Mulai Petualangan"
            // transform.gameObject.SetActive(false); 
        }
    }
}