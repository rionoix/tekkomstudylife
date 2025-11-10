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
    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextMeshProUGUI scoreText;

    [Header("Quiz Data")]
    public Question[] questions;
    private int currentQuestionIndex = 0;
    private int score = 0;

    void Start()
    {
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
            }
        }
        else
        {
            EndQuiz();
        }
    }

    void CheckAnswer(int index)
    {
        if (index == questions[currentQuestionIndex].correctAnswerIndex)
        {
            score++;
        }

        currentQuestionIndex++;
        ShowQuestion();
    }

    void EndQuiz()
    {
        questionText.text = "Quiz Selesai!\nNilai kamu: " + score + "/" + questions.Length;
        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }
        if (scoreText != null)
            scoreText.text = "Skor Akhir: " + score;
    }
}
