using System.ComponentModel;
using UnityEngine;



public class PGSDialog : MonoBehaviour
{
    public struct PGSDialogData
    {
        public enum PGSDialogType
        {
            ACHIEVEMENT,
            LEADERBOARD
        }

        public PGSDialogType m_Type;
        public string m_BodyText;
    }

    private PGSDialogData m_Data;

    [Header("Attached Components")]
    [SerializeField] private TextMesh m_TitleTextMesh;
    [SerializeField] private TextMesh m_BodyTextMesh;
    [SerializeField] private SpriteRenderer m_BackDisplay;

    [Header("Defaults")]
    [SerializeField] private string m_AchievementTitle = "Achievement Acquired";
    [SerializeField] private string m_LeaderboardTitle = "High Score Posted";
    [SerializeField] private Sprite m_AchievementBack;
    [SerializeField] private Sprite m_LeaderboardBack;

    [Header("Animation")]
    [SerializeField] private AnimationCurve m_FadeCurve;
    [SerializeField] private AnimationCurve m_SlideCurve;
    [SerializeField] private float m_SlideDistance;
    [SerializeField] private AnimationCurve m_ScaleCurve;
    [SerializeField] private float m_FadeSpeed = 0.2f;
    private float m_FadeTimer = 0.0f;

    private float m_SlideOrigin = 0.0f;
    private Vector3 m_ScaleOrigin = Vector3.one;

    public void Initialize(PGSDialogData _data)
    {
        m_Data = _data;
        m_TitleTextMesh.text = m_Data.m_Type == PGSDialogData.PGSDialogType.ACHIEVEMENT ? m_AchievementTitle : m_LeaderboardTitle;
        m_BodyTextMesh.text = m_Data.m_BodyText;
        m_BackDisplay.sprite = m_Data.m_Type == PGSDialogData.PGSDialogType.ACHIEVEMENT ? m_AchievementBack : m_LeaderboardBack;
        m_SlideOrigin = transform.localPosition.y;
        m_ScaleOrigin = transform.localScale;
        UpdateAnimation();
    }

    public bool Completed()
    {
        return m_FadeTimer >= 1.0f;
    }

    private void UpdateAnimation()
    {
        float p = Mathf.Min(1.0f, m_FadeTimer);

        // Fade
        float a = m_FadeCurve.Evaluate(p);
        Color c;
        c = m_TitleTextMesh.color;
        c.a = a;
        m_TitleTextMesh.color = c;
        c = m_BodyTextMesh.color;
        c.a = a;
        m_BodyTextMesh.color = c;
        c = m_BackDisplay.color;
        c.a = a;
        m_BackDisplay.color = c;

        // Slight slide
        Vector3 pos = transform.localPosition;
        pos.y = m_SlideOrigin + m_SlideCurve.Evaluate(p) * m_SlideDistance;
        transform.localPosition = pos;

        // Slight scale
        float s = m_ScaleCurve.Evaluate(p);
        transform.localScale = s * m_ScaleOrigin;
    }

    private void Update()
    {
        m_FadeTimer += m_FadeSpeed * Time.deltaTime;

        // Update alphas
        UpdateAnimation();
    }
}
