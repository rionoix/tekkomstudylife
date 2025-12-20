using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Tambahan untuk Restart Game

public class SisterGameManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform gamePanel;       
    public GameObject intermissionObj; 
    public TextMeshProUGUI descText;   
    public GameObject nextButton;      
    public GameObject runButton;
    public TextMeshProUGUI timerText; // (Opsional) Jika ingin menampilkan waktu berjalan

    [Header("Konfigurasi Level")]
    // Kita gabungkan Visual (Hierarchy) dan Data (Project) jadi satu paket
    public List<LevelScenario> scenarios; 

    private int currentIndex = 0;
    private float startTime;
    private bool isGameActive = false;

    [System.Serializable]
    public struct LevelScenario
    {
        public string name; // Cuma untuk label biar rapi
        public GameObject levelObject; // Tarik objek Level dari Hierarchy ke sini
        public SisterLevelData levelData; // Tarik data jawaban dari Project ke sini
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        currentIndex = 0;
        startTime = Time.time; // Mulai hitung waktu
        isGameActive = true;
        LoadScenario(0);
    }

    void Update()
    {
        // Debugging: Tekan R untuk Restart cepat
        if (Input.GetKeyDown(KeyCode.R)) RestartGame();
    }

    public void LoadScenario(int index)
    {
        // 1. Matikan Level Sebelumnya (jika ada)
        if (currentIndex < scenarios.Count && scenarios[currentIndex].levelObject != null)
            scenarios[currentIndex].levelObject.SetActive(false);

        currentIndex = index;

        // 2. Cek apakah sudah tamat (Melebihi jumlah level)
        if (currentIndex >= scenarios.Count)
        {
            // Harusnya sudah ditangani di level terakhir (Level 8), tapi untuk jaga-jaga:
            Debug.Log("Selesai total!");
            return;
        }

        LevelScenario currentScenario = scenarios[currentIndex];

        // 3. Nyalakan Level Baru
        if (currentScenario.levelObject != null) 
            currentScenario.levelObject.SetActive(true);

        // 4. Reset UI
        SisterLevelData data = currentScenario.levelData;
        
        // Reset Teks & Tombol
        if(descText) descText.text = "";
        nextButton.SetActive(false);
        
        // Jika Data kosong (misal halaman terakhir/awal banget), anggap text only
        if (data == null) 
        {
            runButton.SetActive(false);
            nextButton.SetActive(true);
            intermissionObj.SetActive(false); 
            return; 
        }

        // Logic Tampilan UI (Text Only vs Puzzle)
        if (data.isTextOnly)
        {
            runButton.SetActive(false);
            intermissionObj.SetActive(true);
            var textComp = intermissionObj.GetComponentInChildren<TextMeshProUGUI>();
            if(textComp) textComp.text = data.systemDescription;
            nextButton.SetActive(true);
        }
        else
        {
            runButton.SetActive(true);
            intermissionObj.SetActive(false);
        }
    }

    public void OnRunButtonPressed()
    {
        SisterLevelData data = scenarios[currentIndex].levelData;
        if (CheckAnswer(data))
        {
            descText.text = "BENAR! \n" + data.systemDescription;
            nextButton.SetActive(true);
            runButton.SetActive(false);
        }
        else
        {
            descText.text = "Koneksi masih salah. Coba cek lagi!";
        }
    }

    // Fungsi Cek Jawaban (Sudah diperbaiki logikanya)
    bool CheckAnswer(SisterLevelData data)
    {
        LineManager lineMgr = FindObjectOfType<LineManager>();
        if (lineMgr == null) return false;

        List<ConnectionPair> userConns = lineMgr.currentUserConnections;
        List<ConnectionPair> correctConns = data.correctConnections;

        // 1. Jumlah garis harus sama
        if (userConns.Count != correctConns.Count) return false;

        // 2. Cek setiap kunci jawaban apakah ada di list user
        int correctCount = 0;
        foreach (var correct in correctConns)
        {
            bool found = false;
            foreach (var user in userConns)
            {
                // Cek bolak balik (A ke B atau B ke A dianggap sama)
                if ((user.pinID_A == correct.pinID_A && user.pinID_B == correct.pinID_B) ||
                    (user.pinID_A == correct.pinID_B && user.pinID_B == correct.pinID_A))
                {
                    found = true;
                    break;
                }
            }
            if (found) correctCount++;
        }

        return correctCount == correctConns.Count;
    }

    public void NextLevel()
    {
        // Bersihkan garis-garis lama sebelum pindah level
        LineManager lineMgr = FindObjectOfType<LineManager>();
        if (lineMgr) lineMgr.ClearLines();

        // Jika ini level terakhir (Screen 8), jangan load next, tapi tampilkan skor
        if (currentIndex == scenarios.Count - 1)
        {
            RestartGame(); // Atau kembali ke menu utama
        }
        else
        {
            LoadScenario(currentIndex + 1);
            
            // Khusus Logic untuk Level 8 (Halaman Akhir)
            // Jika level selanjutnya adalah index terakhir, hitung waktu
            if (currentIndex == scenarios.Count - 1)
            {
                float totalTime = Time.time - startTime;
                
                // Cari teks di objek level terakhir untuk menampilkan waktu
                // Asumsi di Level 8 ada component TextMeshProUGUI
                TextMeshProUGUI endText = scenarios[currentIndex].levelObject.GetComponentInChildren<TextMeshProUGUI>();
                if(endText)
                {
                    // Format waktu menjadi Menit:Detik
                    string minutes = Mathf.Floor(totalTime / 60).ToString("00");
                    string seconds = (totalTime % 60).ToString("00");
                    endText.text = $"SELAMAT!\nGame Selesai.\nWaktu: {minutes}:{seconds}";
                }
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}