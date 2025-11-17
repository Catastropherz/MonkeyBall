using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PGSDialogHandler : MonoBehaviour
{
    [SerializeField] private GameObject m_DialogPrefab;

    [SerializeField] private bool m_DebugTest;

    private Queue m_QueuedDialogs;
    private PGSDialog m_CurrentDialog = null;

    public void LogAchievement(string _bodyText)
    {
        PGSDialog.PGSDialogData data;
        data.m_Type = PGSDialog.PGSDialogData.PGSDialogType.ACHIEVEMENT;
        data.m_BodyText = _bodyText;
        m_QueuedDialogs.Enqueue(data);
    }

    public void LogLeaderboard(string _bodyText)
    {
        PGSDialog.PGSDialogData data;
        data.m_Type = PGSDialog.PGSDialogData.PGSDialogType.LEADERBOARD;
        data.m_BodyText = _bodyText;
        m_QueuedDialogs.Enqueue(data);
    }

    private void Awake()
    {
        m_QueuedDialogs = new Queue();
    }
    
    private void Update()
    {
        if (m_DebugTest)
        {
            LogAchievement("Test Achievement Text!");
            LogLeaderboard("Test Leaderboard Text!");
            m_DebugTest = false;
        }

        if (m_CurrentDialog)
        {
            if (m_CurrentDialog.Completed())
            {
                Destroy(m_CurrentDialog.gameObject);
                m_CurrentDialog = null;
            }
        }

        if (m_QueuedDialogs.Count > 0 && m_CurrentDialog == null)
        {
            PGSDialog.PGSDialogData data = (PGSDialog.PGSDialogData)m_QueuedDialogs.Dequeue();
            GameObject go = Instantiate(m_DialogPrefab, transform);
            m_CurrentDialog = go.GetComponent<PGSDialog>();
            m_CurrentDialog.Initialize(data);
        }
    }
}
