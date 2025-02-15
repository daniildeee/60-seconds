using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;

    [Header("Health Bar")]
    public float maxHealth = 100;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public TextMeshProUGUI healthText;

    [Header("Damage Overlay")]
    public Image overlay;
    public float duration;
    public float fadeSpeed;
    private float durationTimer;

    void Start()
    {
        health = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();

        if (health <= 0)
        {
            GameOver();
        }

        if (overlay.color.a > 0)
        {
            if (health < 30)
            {
                return;
            }
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
    }

    public void UpdateHealthUI()
    {
        Debug.Log(health);
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        
        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        
        if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }

        healthText.text = health + "/" + maxHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        durationTimer = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

    void GameOver()
    {
        Debug.Log("You DIED");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;  // Остановка в редакторе
        #else
            Application.Quit(); // Закрытие в билде
        #endif
    }
}
