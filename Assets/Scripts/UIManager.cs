using UnityEngine;
using TMPro; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI teksSkor; 
    public Image iconKristalUI;      

    [Header("Aset Sprite Kristal")]
    public Sprite kristalBiru;  
    public Sprite kristalMerah; 

    [Header("Sistem Speaker Komponen")]
    public AudioSource speakerSFX;       // Untuk suara instan (Ambil kristal / Lempar umpan)
    public AudioSource speakerLaguNgejar; // KHUSUS Untuk memutar musik dikejar rubah (Bisa di-Stop)

    [Header("Aset Kaset Audio (Clip)")]
    public AudioClip suaraDapatKristal;  
    public AudioClip suaraLemparUmpan;   
    public AudioClip suaraDikejarRubah;   

    private int jumlahKristal = 0;
    private bool playlistBahayaAktif = false; 

    public void TambahSkor()
    {
        jumlahKristal++;
        teksSkor.text = jumlahKristal + " / 10";
        Debug.Log("Kristal bertambah! Skor sekarang: " + jumlahKristal);

        PutarSuaraInstan(suaraDapatKristal);

        if (jumlahKristal >= 10)
        {
            MenangDanPindahScene();
        }
    }

    public void KurangiSkor()
    {
        if (jumlahKristal > 0)
        {
            jumlahKristal--; 
            teksSkor.text = jumlahKristal + " / 10"; 
            Debug.Log("Kristal berkurang untuk umpan! Skor sekarang: " + jumlahKristal);

            PutarSuaraInstan(suaraLemparUmpan);
        }
    }

    public int GetJumlahKristal() { return jumlahKristal; }

    // Fungsi Pengatur Transisi Suara saat Dikejar vs Aman
    public void SetStatusBahaya(bool isDalamBahaya, PergerakanPemain pemain)
    {
        if (pemain == null) return;

        if (isDalamBahaya)
        {
            iconKristalUI.sprite = kristalMerah; 
            pemain.kecepatanJalan = 250f;        

            // 🎵 JIKA BARU MASUK RADAR BAHAYA:
            if (!playlistBahayaAktif)
            {
                playlistBahayaAktif = true;

                // Putar musik dikejar menggunakan speaker khusus agar bisa kita kontrol
                if (speakerLaguNgejar != null && suaraDikejarRubah != null)
                {
                    speakerLaguNgejar.clip = suaraDikejarRubah;
                    speakerLaguNgejar.loop = true; // Set loop biar kalau kasetnya pendek, dia ngulang terus pas dikejar
                    speakerLaguNgejar.Play();
                }
            }
        }
        else
        {
            iconKristalUI.sprite = kristalBiru;  
            pemain.kecepatanJalan = 500f;        

            // 🎵 JIKA SUDAH AMAN / RUBAH BERHENTI NGEJAR:
            if (playlistBahayaAktif)
            {
                playlistBahayaAktif = false; 

                // PAKSA BERHENTI: Matikan musik tegang saat itu juga secara instan!
                if (speakerLaguNgejar != null)
                {
                    speakerLaguNgejar.Stop();
                }
            }
        }
    }

    private void PutarSuaraInstan(AudioClip kasetSuara)
    {
        if (speakerSFX != null && kasetSuara != null)
        {
            speakerSFX.PlayOneShot(kasetSuara);
        }
    }

    void MenangDanPindahScene()
    {
        SceneManager.LoadScene("GardenScene"); 
    }
}