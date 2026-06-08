using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ada untuk berpindah antar scene

public class MainMenuScript : MonoBehaviour
{
    // Fungsi ini akan dipanggil saat tombol Play diklik
    public void PindahKeProlog()
    {
        // "Prologue" adalah nama scene yang sudah kamu daftar di Build Profiles
        // Pastikan tulisannya sama persis (besar kecil hurufnya)
        SceneManager.LoadScene("Prologue"); 
    }

    // Fungsi Start dan Update bisa dihapus jika tidak digunakan agar kode lebih bersih
}