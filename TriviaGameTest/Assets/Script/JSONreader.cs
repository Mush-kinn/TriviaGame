using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class JSONreader : MonoBehaviour
{
    [Serializable]
    public class Song{
      public string id; // ID for the song
      public string title;
      public string artist;
      public string picture; // Image web address location
      public string sample; // music web address location
    };

    [Serializable]
    public class Choice{
        public string artist;
        public string title;
    }
    [Serializable]
    public class Question{
      public string id; // Id for question Set
      public int answerIndex; // Zero based
      public Choice[] choices;
      public Song song; // Correct song class object
    };

    [Serializable]
     public class Playlist{
      public string id; // Id for playlist
      public Question[] questions; // Array of question objects
      public string playlist; // Plalist Title
    };

    [Serializable]
    public class List{
      public Playlist[] temporaryObject;
    }
    public Playlist[] allPlaylists;
    public string jsonFileText;
    string path = Application.streamingAssetsPath + "/coding-test-frontend-unity.json";

    // Start is called before the first frame update
    void Start()
    {
        jsonFileText = File.ReadAllText(path);
      // Unity's JsonUtility Doesn't handle JSONs that are arrays, therefore I have to "fake" an object to store
      // the array in, essentially creating a temporary object to hold the JSON array when read, I also
      // created a class that just hold an playlist array to extract the JSON only.
        List tempList;
        tempList = JsonUtility.FromJson<List>("{\"temporaryObject\" : " + jsonFileText + "}");

        // Copying the data to a simpler object to make it easier to use.
        allPlaylists = tempList.temporaryObject;
    }

     public void Update()
    {
        
    }
}
