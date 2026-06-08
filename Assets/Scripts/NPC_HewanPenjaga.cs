using UnityEngine;

public class NPC_HewanPenjaga : MonoBehaviour
{
    [Header("Target Pemain")]
    public Transform pemain;          // Tarik objek KarakterPemain ke sini di Inspector
    public PergerakanPemain scriptPemain; // Tarik objek KarakterPemain ke sini juga

    [Header("Sistem Patroli (Soal 4)")]
    public Transform titikA;          // Koordinat penanda batas patroli ujung A
    public Transform titikB;          // Koordinat penanda batas patroli ujung B
    public float kecepatanPatroli = 100f;
    private Transform titikTarget;

    [Header("Sistem Radar Kejar (Soal 5)")]
    public float jarakDeteksi = 150f; // Jarak radius hewan mulai mendeteksi pemain
    public float kecepatanMengejar = 50f;

    private UIManager uiManager;
    private bool sedangMengejar = false;

    void Start()
    {
        // Menggunakan fungsi terbaru Unity 6 agar performa maksimal dan bebas warning (Soal 2)
        uiManager = FindFirstObjectByType<UIManager>();
        
        // Mengeset arah jalan awal patroli menuju Titik A
        titikTarget = titikA;
    }

    void Update()
    {
        if (pemain == null || scriptPemain == null) return;

        // Hitung jarak realtime antara koordinat hewan dengan koordinat pemain
        float jarakKePemain = Vector3.Distance(transform.position, pemain.position);

        // TRIGGER SOAL 5: Jika pemain masuk ke dalam jarak radius deteksi, NPC mengejar
        if (jarakKePemain <= jarakDeteksi)
        {
            if (!sedangMengejar)
            {
                sedangMengejar = true;
                // REALTIME IMPACT: Mengubah ikon UI jadi merah dan memperlambat jalan pemain (Soal 2 & 3)
                if (uiManager != null) uiManager.SetStatusBahaya(true, scriptPemain);
            }

            MengejarPemain();
        }
        // SOAL 4: Jika pemain berhasil kabur menjauh keluar radius, NPC kembali patroli santai
        else
        {
            if (sedangMengejar)
            {
                sedangMengejar = false;
                // Mengembalikan status UI menjadi AMAN (Kristal biru & kecepatan normal kembali)
                if (uiManager != null) uiManager.SetStatusBahaya(false, scriptPemain);
            }

            AksiPatroli();
        }
    }

    void AksiPatroli()
    {
        if (titikA == null || titikB == null) return;

        // Bergerak mendekati titik target saat ini secara konstan
        transform.position = Vector3.MoveTowards(transform.position, titikTarget.position, kecepatanPatroli * Time.deltaTime);

        // Jika NPC sudah sampai di titik tujuan, balikkan targetnya (bolak-balik)
        if (Vector3.Distance(transform.position, titikTarget.position) < 1f)
        {
            titikTarget = (titikTarget == titikA) ? titikB : titikA;
            
            // Membalikkan arah visual sprite (hadap kanan/kiri) agar jalannya natural
            Vector3 skala = transform.localScale;
            skala.x = (titikTarget == titikA) ? -Mathf.Abs(skala.x) : Mathf.Abs(skala.x);
            transform.localScale = skala;
        }
    }

    void MengejarPemain()
    {
        // Berlari cepat langsung memburu koordinat posisi pemain secara realtime
        transform.position = Vector3.MoveTowards(transform.position, pemain.position, kecepatanMengejar * Time.deltaTime);
        
        // Membalikkan arah visual sprite agar selalu menghadap ke posisi pemain saat mengejar
        Vector3 skala = transform.localScale;
        skala.x = (pemain.position.x < transform.position.x) ? -Mathf.Abs(skala.x) : Mathf.Abs(skala.x);
        transform.localScale = skala;
    }

    // Menggambar indikator lingkaran merah di jendela Scene biar mempermudah dosen menguji jarak radar
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, jarakDeteksi);
    }
}