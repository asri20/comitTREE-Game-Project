# comitTREE - Game Edukasi & Habit-Tracking Anak berbasis Unity 2D
## 📌 1. Deskripsi Proyek
**comitTREE** adalah sebuah game edukasi 2D yang dirancang khusus untuk anak-anak usia sekolah dasar. Game ini menggabungkan konsep *learning* dengan fitur *habit-tracking*, di mana pertumbuhan pohon utama di kebun (`GardenScene`) terikat langsung dengan komitmen harian pengguna serta keberhasilan mereka dalam menyelesaikan tantangan akademis di setiap Stage pembelajaran.

---

## 🎮 2. Alur Pembelajaran & Level Design (5 Stage Linier)
Game ini memanfaatkan penyimpanan state permanen menggunakan `PlayerPrefs` ("LevelPohon") untuk melacak progres pemain secara linier:
1. **Stage 1 (`Stage1_NumberOrchard`):** Tantangan kognitif belajar berhitung dasar bersama Sprout menggunakan objek buah apel. Selesai stage, nilai `LevelPohon` naik ke 2.
2. **Stage 2 (`Stage2_ColorFlower`):** Edukasi pengenalan ragam warna melalui mekanik tebak warna kelopak bunga. Selesai stage, nilai `LevelPohon` naik ke 4.
3. **Stage 3 (`Stage3_Alphabet Canopy`):** Pengenalan huruf, menyusun alfabet dasar, dan interaksi interaktif merawat daun. Selesai stage, nilai `LevelPohon` naik ke 6.
4. **Stage 4 (`Stage4_Kolam Teratai Geometri`):** Pengenalan pola dan bentuk geometri dasar (segitiga, lingkaran, persegi) di area kolam teratai bersama karakter Koko Katak. Selesai stage, nilai `LevelPohon` naik ke 8.
5. **Stage 5 (`Stage5_Magical Forest Maze`):** Tantangan problem-solving akhir di mana pemain mengendalikan karakter menelusuri labirin untuk mengumpulkan kristal kolektif. Selesai stage, nilai `LevelPohon` mencapai puncaknya di angka 9.

---

## 🛠️ 3. Implementasi Teknis & Arsitektur Kode
Proyek ini mengimplementasikan prinsip-prinsip utama rekayasa perangkat lunak game pada Unity Engine:
* **Game Engine:** Unity 6 (6000.0.74f1)
* **Bahasa Pemrograman:** C# Scripting melalui VS Code.
* **Dynamic Tree Growth System (`PohonManager.cs`):** Mengelola 9 tingkatan visual pohon secara dinamis di `GardenScene` (dari bibit kecil hingga pohon raksasa megah yang berbunga lebat).
* **State & Animation Synchronization:** Transisi antar-wujud pohon dipicu menggunakan parameter bertipe `Trigger` (*Tumbuh*) pada *Animator Controller*.
* **Coroutine Controller (Anti-Lag/Flicker):** Menggunakan fungsi `IEnumerator` (`yield return`) untuk memberikan jeda waktu rendering yang stabil saat transisi akhir (Pohon Level 8 mekar ke Level 9). Hal ini mencegah animasi "melompat instan" sekaligus memperbarui balon teks dialog Sprout secara *real-time* menjadi kalimat ucapan selamat tamat.

---

## 📂 4. Struktur Direktori Utama Proyek (Repository Layout)
Berikut adalah susunan berkas utama yang ada di dalam repository ini:
```text
comitTREE/
├── Assets/
│   ├── Audio/               # Kumpulan BGM kebun, suara sapaan intro, SFX Benar/Salah
│   ├── Prefabs/             # Objek UI Panel Dialog Sprout, Kristal, dan ApelPrefab
│   ├── Scenes/              # MainMenu, Prologue, GardenScene, dan Stage 1 s.d 5 utuh
│   ├── Scripts/
│   │   ├── Animations_/     # Controller pohon, klip gerak mekar (Tumbuh_8ke9), & Animasi Sprout
│   │   ├── AlphabetGame.cs  # Logika utama mini-game pembelajaran huruf di Stage 3
│   │   ├── ColorGameManager # Logika tebak warna bunga di Stage 2
│   │   ├── NumberGame.cs    # Logika berhitung buah di Stage 1
│   │   ├── ShapeGame.cs     # Logika tebak bentuk geometri kolam di Stage 4
│   │   ├── PergerakanPemain # Logika pergerakan input karakter di Stage 5 Labirin
│   │   └── PohonManager.cs  # Manajemen inti level pertumbuhan 9 wujud pohon utama
│   └── Sprites/             # Aset visual 2D (Karakter Sprout, Owel, BG Garden, dsb.)
├── ProjectSettings/         # Konfigurasi input system, tag manager, dan URP Graphics Unity
└── README.md                # Dokumentasi utama proyek