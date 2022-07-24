using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ResultSceneLogic : MonoBehaviour
{
    [SerializeField, Header("Guesses Array")] TextMeshProUGUI[] m_guess;
    [SerializeField, Header("Answers Array")] TextMeshProUGUI[] m_answer;

    [SerializeField] TextMeshProUGUI m_FinalScore;

    [SerializeField] Button m_next;

    [SerializeField] JSONreader m_JSON;
    [SerializeField] QuizLogic m_mainLogic;
    QuizLogic.ResultTally m_tally;
    JSONreader.Question[] selectedQuestions = null;

    private void Start()
    {
        // Delay to make sure if scenes are loaded They are unloaded before any error
        // are triggered
        StartCoroutine(LateStart(.5f));
    }

    IEnumerator LateStart(float _delay)
    {
        yield return new WaitForSecondsRealtime(_delay);
        Setup();
    }

    void Setup()
    {
        this.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        m_JSON = FindObjectOfType<JSONreader>();
        m_mainLogic = FindObjectOfType<QuizLogic>();
        string id = m_mainLogic.SelectedPlaylistID;

        for (int i = 0; i < m_JSON.allPlaylists.Length; ++i)
        {
            if (id == m_JSON.allPlaylists[i].id)
            {
                selectedQuestions = m_JSON.allPlaylists[i].questions;
                break;
            }
        }

        m_tally = m_mainLogic.m_ScoreDetails;

        for(int i = 0; i < selectedQuestions.Length; ++i)
        {
            m_guess[i].text = GetSongSelection(i, m_tally.answers[i].x);
            m_answer[i].text = GetSongSelection(i, m_tally.answers[i].y);
            if(m_tally.answers[i].x == m_tally.answers[i].y)
            {
                m_guess[i].color = Color.green;
            }
            else
            {
                m_guess[i].color = Color.red;
            }
        }
        m_FinalScore.text = "Your Final Score was: " + m_tally.totalScore + "/5";

        m_next.onClick.AddListener(() => m_mainLogic.NewGame());
    }

    public string GetSongSelection(int questionIndex, int choiceIndex)
    {
        return new string(selectedQuestions[questionIndex].choices[choiceIndex].artist + ": \"" +
          selectedQuestions[questionIndex].choices[choiceIndex].title + "\"");
    }
}
