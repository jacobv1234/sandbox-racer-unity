using System.Linq;
using UnityEngine;
using TMPro;

public class Track : MonoBehaviour
{
    private GameObject[,] tiles;
    private GameObject[,] tracks;

    private GameObject[] checkpoints;
    private GameObject finish = null;
    private int laps;

    private StateTracker state;

    private TMP_Text errorMessageBox; // type is a TextMeshPro text object

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tiles = new GameObject[8,8];
        tracks = new GameObject[8,8];

        GameObject[] tileObjects = GameObject.FindGameObjectsWithTag("EditorTile");
        foreach (GameObject tileObject in tileObjects)
        {
            int x = (int) (tileObject.transform.position.x + 40) / 10;
            int z = (int) (tileObject.transform.position.z + 40) / 10;
            tiles[x,z] = tileObject;
        }

        state = GameObject.FindGameObjectWithTag("State").GetComponent<StateTracker>();

        errorMessageBox = GameObject.Find("ErrorMessage").GetComponent<TMP_Text>();
    }

    // returns "" if successful, returns error message if not
    private string loadTrack()
    {
        checkpoints = new GameObject[0];
        finish = null;

        for (int x = 0; x < 8; x++)
        {
            for (int z = 0; z < 8; z++)
            {
                // load the object
                TrackSpawning spawner = tiles[x, z].GetComponent<TrackSpawning>();
                tracks[x, z] = spawner.getChild();

                if (tracks[x, z] != null) // ignore blank tiles
                {
                    string objectName = tracks[x, z].transform.name;
                    if (objectName.Contains("checkpoint"))
                    {
                        // add the checkpoint to the array
                        checkpoints = checkpoints.Concat(new GameObject[] { tracks[x, z] }).ToArray();
                    }
                    if (objectName.Contains("finish"))
                    {
                        // create reference to finish line if there isn't already one
                        if (finish != null)
                        {
                            return "Error: Track must have only one finish line.";
                        }
                        finish = tracks[x, z];
                    }
                }
            }
        }

        Debug.Log(checkpoints.Length);

        // validity checks
        if (finish == null)
        {
            return "Error: Track must include a finish line.";
        }
        if (checkpoints.Length == 0)
        {
            return "Error: Track must include at least one checkpoint.";
        }


        // load lap count
        string lapText = GameObject.Find("LapEntry").GetComponent<TMP_InputField>().text;

        if (lapText == "")
        {
            lapText = "3";
        }

        // parse text
        if (int.TryParse(lapText, out int num))
        {
            if (num > 0)
            {
                laps = num;
            }
            else
            {
                return "Error: Lap count must be greater than 0.";
            }
        }
        else
        {
            return "Error: Lap count must be a number.";
        }

        return "";
    }

    // called by clear button
    void clearTrack()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int z = 0; z < 8; z++)
            {
                TrackSpawning spawner = tiles[x, z].GetComponent<TrackSpawning>();
                if (spawner != null)
                {
                    spawner.deleteChild();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state.wasChangedThisFrame())
        {
            switch (state.getState())
            {
                case 0:
                    finish = null; checkpoints = new GameObject[0]; break;
                case 1:
                    string error = loadTrack();
                    if (error != "")
                    {
                        // cancel starting the race
                        state.setState(0);
                    }

                    // display error to user (also removes the error if successful)
                    errorMessageBox.text = error;
                    break;
            }
        }
    }

    public int getCheckpointCount()
    {
        return checkpoints.Length;
    }

    public int getLapCount()
    {
        return laps;
    }
}
