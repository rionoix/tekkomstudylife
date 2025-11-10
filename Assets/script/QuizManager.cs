using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct Question
{
    [TextArea]
    public string questionText;
    public string[] answers;
    public int correctAnswerIndex;
}

public class QuizManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject introPanel;
    public GameObject quizPanel;
    public GameObject endPanel;

    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI endText;

    [Header("Quiz Data")]
    public Question[] questions;
    private int currentQuestionIndex = 0;
    private int score = 0;

    void Start()
    {
        // Awal game: tampilkan panel intro
        introPanel.SetActive(true);
        quizPanel.SetActive(false);
        endPanel.SetActive(false);
    }

    // Dipanggil saat tombol PLAY ditekan
    public void StartQuiz()
    {
        score = 0;
        currentQuestionIndex = 0;

        introPanel.SetActive(false);
        endPanel.SetActive(false);
        quizPanel.SetActive(true);

        ShowQuestion();
    }

    void ShowQuestion()
    {
        if (currentQuestionIndex < questions.Length)
        {
            Question q = questions[currentQuestionIndex];
            questionText.text = q.questionText;

            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.answers[i];
                int index = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
                answerButtons[i].gameObject.SetActive(true);
            }

            if (scoreText != null)
                scoreText.text = "Skor: " + score;
        }
        else
        {
            EndQuiz();
        }
    }

    void CheckAnswer(int index)
    {
        if (index == questions[currentQuestionIndex].correctAnswerIndex)
            score++;

        currentQuestionIndex++;
        ShowQuestion();
    }

    void EndQuiz()
    {
        quizPanel.SetActive(false);
        endPanel.SetActive(true);

        endText.text = "Quiz Selesai!\nNilai kamu: " + score + "/" + questions.Length;
        if (scoreText != null)
            scoreText.text = "Skor Akhir: " + score;
    }

    public void RestartQuiz()
    {
        StartQuiz();
    }

    public void BackToIntro()
    {
        introPanel.SetActive(true);
        quizPanel.SetActive(false);
        endPanel.SetActive(false);
    }
}
