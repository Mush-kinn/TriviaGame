using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WelcomeSceneLogic : MonoBehaviour
{
    [SerializeField] Button[] m_options;
    [SerializeField] JSONreader m_JSON;
    [SerializeField] QuizLogic m_mainLogic;

    void Start()
    {
        this.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        m_JSON = FindObjectOfType<JSONreader>();
        m_mainLogic = FindObjectOfType<QuizLogic>();

        //Note to self; the lambda or whatever technique to add the listener saves everything's refference,
        // even temp variables, so using loops such as for(int i....) wont't work cause the last value of i
        // will still be used for all calls. RIP me. 

        m_options[0].GetComponentInChildren<TextMeshProUGUI>().text = m_JSON.allPlaylists[0].playlist;
        m_options[0].onClick.AddListener(() => m_mainLogic.PlaylistSelected(m_JSON.allPlaylists[0].id));
        //m_options[0].onClick.AddListener(() => Debug.Log(0 + "option"));
        Debug.Log("added " + 0 + " of " + m_options.Length);

        m_options[1].GetComponentInChildren<TextMeshProUGUI>().text = m_JSON.allPlaylists[1].playlist;
        m_options[1].onClick.AddListener(() => m_mainLogic.PlaylistSelected(m_JSON.allPlaylists[1].id));
        //m_options[1].onClick.AddListener(() => Debug.Log(1 + "option"));
        Debug.Log("added " + 1 + " of " + m_options.Length);

        m_options[2].GetComponentInChildren<TextMeshProUGUI>().text = m_JSON.allPlaylists[2].playlist;
        m_options[2].onClick.AddListener(() => m_mainLogic.PlaylistSelected(m_JSON.allPlaylists[2].id));
        //m_options[2].onClick.AddListener(() => Debug.Log(2 + "option"));
        Debug.Log("added " + 2 + " of " + m_options.Length);
    }

}
