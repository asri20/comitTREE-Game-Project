using UnityEngine;
using UnityEngine.UI;

public class TombolTeratai : MonoBehaviour
{
    [Header("Pengaturan Bentuk Teratai")]
    public string namaBentukTeratai; 

    private ShapeGame shapeGameManager;
    private Button komponenButton;

    void Awake()
    {
        shapeGameManager = Object.FindFirstObjectByType<ShapeGame>();
        komponenButton = GetComponent<Button>();
    }

    void Start()
    {
        if (komponenButton != null)
        {
            komponenButton.onClick.RemoveAllListeners();
            komponenButton.onClick.AddListener(SaatTerataiDitekan);
        }
    }

    public void SaatTerataiDitekan()
    {
        if (shapeGameManager != null) {
            // SEKARANG: Mengirim nama bentuk SEKALIGUS posisi teratai ini ke manager
            shapeGameManager.CekJawabanBentuk(namaBentukTeratai, transform.position);
        }
    }
}