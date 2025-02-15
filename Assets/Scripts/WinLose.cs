using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Текст таймера
    public TextMeshProUGUI winText;   // Текст победы

    private float timer = 60f; // Время в секундах
    private bool gameEnded = false; 

    void Start()
    {
        winText.gameObject.SetActive(false); // Скрываем текст победы
        UpdateTimerUI();
    }

    void Update()
    {
        if (!gameEnded)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                EndGame();
            }
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(timer);
        timerText.text = "Time: " + seconds.ToString(); // Обновляем таймер
    }

    void EndGame()
    {
        gameEnded = true;
        winText.gameObject.SetActive(true);
        winText.text = "You Won!";
        winText.color = Color.green;
        timerText.gameObject.SetActive(false); // Скрываем таймер
        Time.timeScale = 0f; // Останавливаем время

        Debug.Log("You Won! Game Over.");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Остановка игры в редакторе
        #else
            Application.Quit(); // Закрытие приложения в билде
        #endif
    }
}
