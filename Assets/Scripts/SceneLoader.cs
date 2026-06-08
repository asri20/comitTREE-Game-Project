using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneLoader : MonoBehaviour
{
    // Fungsi untuk tombol "Mulai Petualangan"
    public void StartGame()
    {
        SceneManager.LoadScene("Stage1_NumberOrchard");
    }

    // Fungsi BARU untuk tombol "Pohon Ajaib" (Btn_lihatpohon)
    public void LihatPohon()
    {
        // Pastikan nama "GardenScene" sama persis dengan nama file di folder Scenes
        SceneManager.LoadScene("GardenScene");
    }

    // Fungsi untuk tombol kembali ke Prologue (bisa ditaruh di GardenScene nanti)
    public void KembaliKePrologue()
    {
        SceneManager.LoadScene("Prologue");
    }
}