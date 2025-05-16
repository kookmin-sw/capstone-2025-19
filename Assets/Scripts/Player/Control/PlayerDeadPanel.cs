using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
//using static System.Net.Mime.MediaTypeNames;

public class PlayerDeadPanel : MonoBehaviour
{

    [SerializeField] private float readyTime = 3.0f;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private Image fadeOutPanel;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        SetPanelClean();
    }

    public void SetDeath()
    {
        ReadyDeadPanel(readyTime);
    }

    private IEnumerator ReadyDeadPanel(float time)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        while (!Mathf.Approximately(fadeOutPanel.color.a, 1))
        {
            float alpha = Mathf.MoveTowards(fadeOutPanel.color.a, 1, fadeSpeed * Time.deltaTime);
            fadeOutPanel.color = new Color(fadeOutPanel.color.r, fadeOutPanel.color.g, fadeOutPanel.color.b, alpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(3f);
        SceneController.Instance.LoadScene("Village");
        PlayerState.Instance.ChageStateHard(PlayerState.State.Idle);
    }

    public void SetPanelClean()
    {
        fadeOutPanel.color = new Color(fadeOutPanel.color.r, fadeOutPanel.color.g, fadeOutPanel.color.b, 0);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }

}
