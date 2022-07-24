using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuizLogic : MonoBehaviour
{
    [SerializeField] string WelcomeScreenName = "WelcomeScreen";
    [SerializeField] string TriviaScreenName = "TriviaScreen";
    [SerializeField] string ResultScreenName = "ResultsScreen";

    /*
      Welcome Screen: User can select which playlist to play.
      : - Load Playlist Titles,
             - Show Playlist Titles in buttons
             - When selected, change screens and send selected Title to trivia screen
    
    GameObject _welcomeScreen;
    
      Trivia Screen: User plays trivia game by guessing the right song
       : - Load all Data from the incoming playlist selected.
             - Display possible choices, along with a default or "?" Image.
             - Load Song sample and play it [ make sure it can loop as needed]
             - When User answers state if it's correct or not, then update the
             -   Placeholder image with the image attached to the song along with other details.
             - Tally up answers (correct or not)
             - Go to next song, and repeat from second Dash(-)
             Once all questions are answered, change screens and send answer tally results.
    
    GameObject _triviaScreen;
    
      Results Screen: Show Final results for the complated Quiz.
      : - With received results, Display Total Score
             - Display Answer Breakdown per question (right or wrong)
             - Display a Button to return to wecome screen
    
    GameObject _resultScreen;
    */
    public class ResultTally
    {
        public int totalScore;
        public Vector2Int[] answers; // answers[4][2] // answer[question1,2,3,4][choiceIndex, playlist.question[question].answerIndex]
    }

    public enum state { WELCOME, TRIVIA, RESULTS, COUNT };
    //public JSONreader m_playlists;
    [HideInInspector] public state m_currState;
    //JSONreader.Playlist m_selectedPlaylist;
    string m_selectedPlaylistId;
    bool m_changeScene;
    public ResultTally m_ScoreDetails;

    public string SelectedPlaylistID
    {
        get { return m_selectedPlaylistId; }   
    }

    private void Awake()
    {
        Debug.Log(SceneManager.GetSceneByName(TriviaScreenName).isLoaded);

        if(SceneManager.GetSceneByName(TriviaScreenName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(TriviaScreenName);
        }

        if(SceneManager.GetSceneByName(ResultScreenName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(ResultScreenName);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_currState = state.WELCOME;
        m_changeScene = false;
        if (!SceneManager.GetSceneByName(WelcomeScreenName).isLoaded)
        {
            SceneManager.LoadScene(WelcomeScreenName, LoadSceneMode.Additive);
        }

        if (SceneManager.GetSceneByName(TriviaScreenName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(TriviaScreenName);
        }

        if (SceneManager.GetSceneByName(ResultScreenName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(ResultScreenName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Update the Current screen
        if(m_changeScene)
        {
            // Change scene logic.
            Debug.Log(m_selectedPlaylistId);
            switch(m_currState)
            {
                case state.WELCOME:
                    // Are we changing from results screen?
                    //Switch to trivia Scene
                    SceneManager.UnloadSceneAsync(WelcomeScreenName);
                    SceneManager.LoadScene(TriviaScreenName, LoadSceneMode.Additive);
                    m_currState = state.TRIVIA;
                    break;

                case state.TRIVIA:
                    SceneManager.UnloadSceneAsync(TriviaScreenName);
                    SceneManager.LoadScene(ResultScreenName, LoadSceneMode.Additive);
                    m_currState = state.RESULTS;
                    break;
                case state.RESULTS:
                    SceneManager.UnloadSceneAsync(ResultScreenName);
                    SceneManager.LoadScene(WelcomeScreenName, LoadSceneMode.Additive);
                    m_selectedPlaylistId = null;
                    m_ScoreDetails = null;
                    m_currState = state.WELCOME;
                    break;
            }
            m_changeScene = false;
        }
    }

    public void PlaylistSelected(string id)
    {
        m_selectedPlaylistId = id;
        m_changeScene = true;
        Debug.Log("selected id: " + id);
    }

    public void TriviaDone(ResultTally _tally)
    {
        m_ScoreDetails = _tally;
        m_changeScene = true;
        Debug.Log(m_ScoreDetails);
    }

    public void NewGame()
    {
        m_changeScene = true;
    }
}
