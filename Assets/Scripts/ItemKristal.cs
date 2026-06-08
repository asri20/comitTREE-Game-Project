using UnityEngine;

public class ItemKristal : MonoBehaviour
{
    private UIManager uiManager;

    void Start()
    {
        // Mencari otomatis UIManager yang ada di SistemManager
        uiManager = FindFirstObjectByType<UIManager>();
    }

    // MENGGUNAKAN VERSI 3D: Fungsi otomatis aktif saat objek 3D saling bersentuhan
    void OnTriggerEnter(Collider collision)
    {
        // Memastikan objek yang menabrak adalah KarakterPemain dengan Tag "Player"
        if (collision.CompareTag("Player"))
        {
            if (uiManager != null)
            {
                uiManager.TambahSkor(); // Panggil fungsi tambah skor di UI
            }

            // Hancurkan objek kristal dari labirin karena sudah diambil
            Destroy(gameObject);
        }
    }
}