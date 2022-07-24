using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class TriviaSceneLogic : MonoBehaviour
{
    [SerializeField, Header("Buttons")] Button[] m_options;
    [SerializeField] Button m_NextButton;

    [SerializeField, Header("TextFields")] TextMeshProUGUI m_resultText;
    [SerializeField] TextMeshProUGUI m_correctSongDetail;

    [SerializeField, Header("Reasources")] RawImage m_image;
    [SerializeField] AudioSource m_audioSource;

    JSONreader m_JSON;
    JSONreader.Question[] selectedQuestions;
    QuizLogic m_mainLogic;
    QuizLogic.ResultTally m_tally;
    int m_currQuestion;

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

        if (m_image == null || m_audioSource == null)
        {
            Debug.Log("You darn need the audio and raw image dummmy");
            m_audioSource = GetComponent<AudioSource>();
            m_image = GetComponentInChildren<RawImage>();
        }

        string id = m_mainLogic.SelectedPlaylistID;

        for (int i = 0; i < m_JSON.allPlaylists.Length; ++i)
        {
            if (id == m_JSON.allPlaylists[i].id)
            {
                selectedQuestions = m_JSON.allPlaylists[i].questions;
                break;
            }
        }
        if (selectedQuestions == null)
        {
            Debug.Log("you failed to pick the questions dummy");
        }

        m_tally = new QuizLogic.ResultTally
        {
            totalScore = 0,
            answers = new Vector2Int[selectedQuestions.Length]
        };

        StartCoroutine(LoadImage(selectedQuestions[m_currQuestion].song.picture));
        StartCoroutine(LoadSong(selectedQuestions[m_currQuestion].song.sample));

        if (m_options[0])
        {
            m_options[0].GetComponentInChildren<TextMeshProUGUI>().text = GetSongSelection(0, 0);
            m_options[0].onClick.AddListener(() => OptionAction(0));
        }

        if (m_options[1])
        {
            m_options[1].GetComponentInChildren<TextMeshProUGUI>().text = GetSongSelection(0, 1);
            m_options[1].onClick.AddListener(() => OptionAction(1));
        }

        if (m_options[2])
        {
            m_options[2].GetComponentInChildren<TextMeshProUGUI>().text = GetSongSelection(0, 2);
            m_options[2].onClick.AddListener(() => OptionAction(2));
        }

        if (m_options[3])
        {
            m_options[3].GetComponentInChildren<TextMeshProUGUI>().text = GetSongSelection(0, 3);
            m_options[3].onClick.AddListener(() => OptionAction(3));
        }

        m_NextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
        m_NextButton.onClick.AddListener(() => NextQuestion());
        m_NextButton.interactable = false;

        m_image.color = Color.clear;
        m_resultText.text = "";
        m_correctSongDetail.text = "";
        m_audioSource.loop = true;
    }

    void OptionAction(int _index)
    {
        m_image.color = Color.white;

        // Reveal Answer
        if(_index == selectedQuestions[m_currQuestion].answerIndex)
        {
            // Correct Answer
            m_resultText.text = "Congratulations! You have guessed correctly";
            m_tally.totalScore += 1;
            m_options[_index].image.color = Color.green;
        }
        else
        {
            m_resultText.text = "Woops, sorry. That wasn't the right answer.";
            m_options[_index].image.color = Color.red;
            m_options[selectedQuestions[m_currQuestion].answerIndex].image.color = Color.green;
        }
        
        m_correctSongDetail.text = "Correct Answer: " + GetSongSelection(m_currQuestion, selectedQuestions[m_currQuestion].answerIndex);

        m_tally.answers[m_currQuestion].x = _index;
        m_tally.answers[m_currQuestion].y = selectedQuestions[m_currQuestion].answerIndex;

        DisableButtons();
        m_NextButton.interactable = true;

        m_audioSource.Stop();
        m_audioSource.clip = null;
    }

    private void Update()
    {
        if (m_audioSource.clip)
        {
            if (!m_audioSource.isPlaying)
            {
                m_audioSource.Play();
                m_audioSource.loop = true;
            }
        }
    }

    void DisableButtons()
    {
        for(int i = 0; i < m_options.Length; ++i)
        {
            m_options[i].interactable = false;
        }
    }

    // WHen the "Next" Button is pressed, Reset items and 
    void NextQuestion()
    { 
        if(m_currQuestion == 4)
        {
            m_mainLogic.TriviaDone(m_tally);
            return;
        }
        m_NextButton.interactable = false;

        m_audioSource.clip = null;
        m_image.texture = null;

        m_currQuestion++;
        m_image.color = Color.clear; 

        StartCoroutine(LoadImage(selectedQuestions[m_currQuestion].song.picture));
        StartCoroutine(LoadSong(selectedQuestions[m_currQuestion].song.sample));

        for (int i = 0; i < m_options.Length; ++i)
        {
            m_options[i].interactable = true;
            m_options[i].GetComponentInChildren<TextMeshProUGUI>().text = GetSongSelection(m_currQuestion, i);
        }

        m_resultText.text = "";
        m_correctSongDetail.text = "";

        for (int i = 0; i < m_options.Length; ++i)
        {
            m_options[i].image.color = Color.white;
        }
    }

    public string GetSongSelection(int questionIndex, int choiceIndex)
    { 
        return new string(selectedQuestions[questionIndex].choices[choiceIndex].artist + ": \"" +
          selectedQuestions[questionIndex].choices[choiceIndex].title + "\"");
    }

    // Coroutine for audio
    public IEnumerator LoadSong(string _url)
    {
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(_url, AudioType.WAV); ;
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("You done goofed once more");
        }
        else
        {
            m_audioSource.clip = ((DownloadHandlerAudioClip)request.downloadHandler).audioClip;
            Debug.Log("AUdio was Loaded<?>");
        }
    }

    // coroutine for image
    public IEnumerator LoadImage(string _url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(_url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("you broke it cause you dont' know what you doing");
        }
        else
        {
            m_image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log("Image was loaded<?>");
        }
    }

}
