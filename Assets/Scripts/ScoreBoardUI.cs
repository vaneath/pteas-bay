using TMPro;
using UnityEngine;

public class ScoreBoardUI : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI currentScoreText;
  [SerializeField] private TextMeshProUGUI highScoreText;

  private void UpdateScoreDisplay()
  {
    if (GamePlayingClockUI.Instance == null)
    {
      Debug.LogError("Score.Instance is null. Ensure Score is active in the scene.");
      return;
    }

    currentScoreText.text = "Score: " + GamePlayingClockUI.Instance.GetCurrentScore();
    highScoreText.text = "High Score: " + GamePlayingClockUI.Instance.GetHighScore();
  }

  private void Update()
  {
    UpdateScoreDisplay();
  }
}
