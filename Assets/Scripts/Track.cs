using System.Linq;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Track : MonoBehaviour
{
    private GameObject[,] tiles;
    private GameObject[,] tracks;

    private GameObject[] checkpoints;
    private GameObject finish = null;
    private int finishXIndex = -1;
    private int finishYIndex = -1;
    private int laps;

    [SerializeField]
    private int gridSize = 8;

    private List<List<int>> path = null;

    private StateTracker state;

    private TMP_Text errorMessageBox; // type is a TextMeshPro text object

    private MusicPlayer music;

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

    // returns "" if successful, error message otherwise
    // path array contains the indexes of the path
    // notes: bottom corner is (0,0), right corner is (7,0), left corner is (0,7), top corner is (7,7)
    private string findPath()
    {
        // setup and 1st move from finish line
        path = new List<List<int>>
        {
            new List<int>{
                finishXIndex, finishYIndex
            }
        };

        int currentX = finishXIndex;
        int currentY = finishYIndex;

        int currentDir = (int)tracks[currentX, currentY].transform.eulerAngles.y / 90;

        switch (currentDir) {
            case 0:
                currentY += 1;
                break;
            case 1:
                currentX += 1;
                break;
            case 2:
                currentY -= 1;
                break;
            default:
                currentX -= 1;
                break;
        }

        path.Add(new List<int> { currentX, currentY });

        // iterate through each track
        while (true)
        {
            // ensure current position is on the grid
            if (currentX < 0 || currentY < 0 || currentX >= gridSize || currentY >= gridSize)
            {
                return "Error: Track is incomplete";
            }

            GameObject nextTrack = tracks[currentX, currentY];
            if (nextTrack == null) {
                return "Error: Track is incomplete";
            }

            string nextTrackType = nextTrack.name;
            int trackRotation = (int)nextTrack.transform.eulerAngles.y / 90;

            if (nextTrackType.Contains("blank"))
            {
                return "Error: Track is incomplete";
            }

            if (nextTrackType.Contains("straight"))
            {
                // straight is aligned correctly if rotation = currentDir or (currentDir+2)%4
                if (trackRotation == currentDir || trackRotation == (currentDir+2)%4)
                {
                    // no need to change direction
                    Debug.Log("Straight track is aligned");
                }
                else
                {
                    return "Error: Track is incomplete";
                }
            }

            if (nextTrackType.Contains("corner"))
            {
                // right-turning corner if rotation = currentDir
                if (trackRotation == currentDir)
                {
                    currentDir = (currentDir + 1) % 4;
                    Debug.Log("Right corner");
                }
                // left-turning corner if rotation-1 = currentDir
                else if (trackRotation == (currentDir + 1) % 4)
                {
                    currentDir = (currentDir + 3) % 4;
                    Debug.Log("Left corner");
                }
                else
                {
                    return "Error: Track is incomplete";
                }
            }

            if (nextTrackType.Contains("checkpoint"))
            {
                // checkpoint is only aligned if rotation = currentDir
                if (trackRotation == currentDir)
                {
                    Debug.Log("Checkpoint");
                }
                else
                {
                    return "Error: Track is incomplete";
                }
            }

            if (nextTrackType.Contains("finish"))
            {
                // finish is only aligned if rotation = currentDir
                if (trackRotation == currentDir)
                {
                    Debug.Log("Finish - Lap complete");
                    return "";
                }
                else
                {
                    return "Error: Track is incomplete";
                }
            }

            // move to the next tile
            switch (currentDir)
            {
                case 0:
                    currentY += 1;
                    break;
                case 1:
                    currentX += 1;
                    break;
                case 2:
                    currentY -= 1;
                    break;
                default:
                    currentX -= 1;
                    break;
            }

            // add the next tile to the path
            path.Add(new List<int> { currentX, currentY });
        }
    }


    // returns "" if successful, returns error message if not
    private string loadTrack()
    {
        checkpoints = new GameObject[0];
        finish = null;
        finishXIndex = -1;
        finishYIndex = -1;

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
                        finishXIndex = x; finishYIndex = z;
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

        string success = findPath();
        if (success.Equals(""))
        {
            music = GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>();
            music.SendMessage("StartRaceMusic", SendMessageOptions.DontRequireReceiver);
        }
        return success;
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

    public List<List<int>> getPath()
    {
        return path;
    }

    public string getNameOfTileAt(int x, int y)
    {
        return tracks[x, y].name;
    }
}
