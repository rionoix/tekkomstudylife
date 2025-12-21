using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

public class SisterGameManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform gamePanel;       
    public GameObject intermissionObj; 
    public TextMeshProUGUI descText;   
    public GameObject nextButton;      
    public GameObject runButton;
    public TextMeshProUGUI timerText; // (Opsional)
    
    // --- TAMBAHAN BARU ---
    public GameObject restartButton; // Masukkan Tombol Restart (UI) ke sini di Inspector
    // ---------------------

    [Header("Konfigurasi Level")]
    public List<LevelScenario> scenarios; 

    private int currentIndex = 0;
    private float startTime;
    private bool isGameActive = false;

    [System.Serializable]
    public struct LevelScenario
    {
        public string name; 
        public GameObject levelObject; 
        public SisterLevelData levelData; 
    }

    void Start()
    {
        // Pastikan tombol restart punya fungsi saat diklik
        if (restartButton != null)
        {
            restartButton.SetActive(false); // Sembunyikan di awal
            // Menambahkan fungsi klik secara otomatis agar tidak perlu setting di Inspector Button
            restartButton.GetComponent<Button>().onClick.AddListener(RestartGame);
        }

        StartGame();
    }

    public void StartGame()
    {
        currentIndex = 0;
        startTime = Time.time; 
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
        // 1. Matikan Level Sebelumnya
        if (currentIndex < scenarios.Count && scenarios[currentIndex].levelObject != null)
            scenarios[currentIndex].levelObject.SetActive(false);

        currentIndex = index;

        // 2. Cek apakah index di luar batas (Safety check)
        if (currentIndex >= scenarios.Count) return;

        LevelScenario currentScenario = scenarios[currentIndex];

        // 3. Nyalakan Level Baru
        if (currentScenario.levelObject != null) 
            currentScenario.levelObject.SetActive(true);

        // --- LOGIKA LEVEL TERAKHIR (SELESAI) ---
        // Jika ini adalah level terakhir di list scenarios (misal: 08_Selesai)
        if (currentIndex == scenarios.Count - 1)
        {
            // Hitung Waktu Total
            float totalTime = Time.time - startTime;
            string minutes = Mathf.Floor(totalTime / 60).ToString("00");
            string seconds = (totalTime % 60).ToString("00");

            // Cari Teks di object level tersebut (Title/Desc) untuk menampilkan skor
            TextMeshProUGUI endText = currentScenario.levelObject.GetComponentInChildren<TextMeshProUGUI>();
            if(endText)
            {
                endText.text = $"SELAMAT!\nGame Selesai.\nWaktu Kamu: {minutes}:{seconds}";
            }

            // Atur Tombol
            nextButton.SetActive(false); // Sembunyikan Next
            runButton.SetActive(false);  // Sembunyikan Run
            intermissionObj.SetActive(false);
            
            if(restartButton) restartButton.SetActive(true); // TAMPILKAN TOMBOL RESTART
            
            return; // Keluar dari fungsi, tidak perlu load logika puzzle
        }

        // --- LOGIKA LEVEL BIASA ---
        SisterLevelData data = currentScenario.levelData;
        
        // Reset Teks & Tombol
        if(descText) descText.text = "";
        nextButton.SetActive(false);
        if(restartButton) restartButton.SetActive(false); // Pastikan restart mati di level biasa
        
        if (data == null) 
        {
            runButton.SetActive(false);
            nextButton.SetActive(true);
            intermissionObj.SetActive(false); 
            return; 
        }

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

    bool CheckAnswer(SisterLevelData data)
    {
        LineManager lineMgr = FindObjectOfType<LineManager>();
        if (lineMgr == null) return false;

        List<ConnectionPair> userConns = lineMgr.currentUserConnections;
        List<ConnectionPair> correctConns = data.correctConnections;

        if (userConns.Count != correctConns.Count) return false;

        int correctCount = 0;
        foreach (var correct in correctConns)
        {
            bool found = false;
            foreach (var user in userConns)
            {
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
        LineManager lineMgr = FindObjectOfType<LineManager>();
        if (lineMgr) lineMgr.ClearLines();

        // Pindah ke level selanjutnya
        // Jika belum level terakhir, load index+1
        if (currentIndex < scenarios.Count - 1)
        {
            LoadScenario(currentIndex + 1);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}