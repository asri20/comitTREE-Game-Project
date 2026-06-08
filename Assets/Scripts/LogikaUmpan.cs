using UnityEngine;

public class LogikaUmpan : MonoBehaviour
{
    [Header("Aset Kristal Kuning Asli")]
    public GameObject prefabKristalKuning; // Slot untuk memasukkan Prefab kristal kuning asli

    void Start()
    {
        // Jalankan fungsi PerubahanKristal setelah 4 detik (waktu rubah makan umpan)
        Invoke("PerubahanKristal", 4f);
    }

    void PerubahanKristal()
    {
        // Pastikan prefab kristal kuning asli sudah dimasukkan di Inspector
        if (prefabKristalKuning != null)
        {
            // Munculkan kembali kristal kuning asli di koordinat posisi umpan ini berada
            Instantiate(prefabKristalKuning, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Prefab Kristal Kuning belum dimasukkan ke dalam script LogikaUmpan!");
        }

        // Hancurkan objek kristal umpan ungu ini
        Destroy(gameObject);
    }
}