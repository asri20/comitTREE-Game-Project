using UnityEngine;

public class PergerakanPemain : MonoBehaviour
{
    public float kecepatanJalan = 500f; // Kecepatan dasar (bisa 500f atau 250f saat lambat)
    public float kecepatanLari = 800f;  // Kecepatan ekstra saat menekan Shift untuk kabur

    [Header("Sistem Umpan Kristal")]
    public GameObject prefabKristalUmpan; // Tempat menaruh prefab KristalUmpan di Inspector

    private float kecepatanAktif; 
    private Rigidbody rb;
    private Animator anim;
    private Vector3 arahInput;
    private UIManager uiManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        uiManager = FindFirstObjectByType<UIManager>();
        kecepatanAktif = kecepatanJalan; 
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        arahInput = new Vector3(inputX, inputY, 0f).normalized;

        // 1. FITUR SPRINT (SHIFT KIRI)
        if (Input.GetKey(KeyCode.LeftShift) && arahInput != Vector3.zero)
        {
            kecepatanAktif = kecepatanLari;
        }
        else
        {
            kecepatanAktif = kecepatanJalan;
        }

        // 2. FITUR UTAMA: MELEMPAR UMPAN KRISTAL (TOMBOL SPASI)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LemparUmpan();
        }

        // 3. Mengatur saklar animasi isWalking
        if (arahInput != Vector3.zero)
        {
            anim.SetBool("isWalking", true);
            if (inputX > 0) transform.localScale = new Vector3(100, 100, 1);
            else if (inputX < 0) transform.localScale = new Vector3(-100, 100, 1);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(arahInput.x * kecepatanAktif * 0.1f, arahInput.y * kecepatanAktif * 0.1f, 0f);
    }

    public void SetKecepatanJalanDasar(float speed)
    {
        kecepatanJalan = speed;
    }

    void LemparUmpan()
    {
        // Pastikan UIManager ada dan objek prefab KristalUmpan sudah diisi di Inspector
        if (uiManager != null && prefabKristalUmpan != null)
        {
            // Ambil angka skor langsung dari memori internal UIManager agar akurat
            int skorSekarang = uiManager.GetJumlahKristal();

            // Umpan hanya bisa dilempar kalau kamu punya minimal 1 kristal
            if (skorSekarang > 0)
            {
                // A. Kurangi skor secara resmi lewat UIManager (Memori internal & Teks UI otomatis sinkron)
                uiManager.KurangiSkor();

                // B. Munculkan kristal umpan di koordinat posisi si anak daun berdiri
                GameObject umpanBaru = Instantiate(prefabKristalUmpan, transform.position, Quaternion.identity);

                // C. HIPNOTIS RUBAH: Paksa rubah untuk mengejar objek kristal umpan baru
                NPC_HewanPenjaga rubah = FindFirstObjectByType<NPC_HewanPenjaga>();
                if (rubah != null)
                {
                    rubah.pemain = umpanBaru.transform; 
                }

                // D. Beri waktu 3 detik bagi kamu untuk lari sebelum rubah sadar ditipu
                Invoke("KembalikanTargetRubah", 3f);

                Debug.Log("Umpan berhasil dilempar! Skor berkurang 1 dan sinkron.");
            }
            else
            {
                Debug.Log("Tidak bisa melempar umpan, jumlah kristal kamu kosong (0)!");
            }
        }
    }
    
    void KembalikanTargetRubah()
    {
        // Kembalikan pandangan rubah untuk mengejar kamu lagi setelah 3 detik
        NPC_HewanPenjaga rubah = FindFirstObjectByType<NPC_HewanPenjaga>();
        if (rubah != null)
        {
            rubah.pemain = this.transform;
        }
    }
}