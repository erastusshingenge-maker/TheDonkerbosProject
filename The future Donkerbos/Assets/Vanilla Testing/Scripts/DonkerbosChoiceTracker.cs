using UnityEngine;
using System;
using System.IO;

public class DonkerbosChoiceTracker : MonoBehaviour
{
    public static DonkerbosChoiceTracker Instance;

    private string filePath;
    private int playerID;
    private string startTime;

    private string scene1Choice = "";
    private string scene2Choice = "";
    private string scene3Choice = "";

    private void Awake()
    {
        // Prevent duplicate trackers
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Record when the participant starts
        startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // CSV file location
        filePath = Path.Combine(
            Application.persistentDataPath,
            "DonkerbosPlayerChoices.csv"
        );

        // Create CSV if it doesn't exist
        if (!File.Exists(filePath))
        {
            File.WriteAllText(
                filePath,
                "PlayerID,StartTime,Scene1,Scene2,Scene3"
                + Environment.NewLine
            );
        }

        // Find the next participant number
        string[] lines = File.ReadAllLines(filePath);

        int highestID = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');

            if (columns.Length > 0 &&
                int.TryParse(columns[0], out int existingID))
            {
                if (existingID > highestID)
                {
                    highestID = existingID;
                }
            }
        }

        playerID = highestID + 1;

        // Create participant row
        SaveChoices();

        Debug.Log("Donkerbos participant started: Player " + playerID);
        Debug.Log("Start time: " + startTime);
        Debug.Log("Results file: " + filePath);
    }

    public void RecordChoice(int sceneNumber, string choice)
    {
        // Validate choice
        if (choice != "Build" &&
            choice != "Find" &&
            choice != "Heal")
        {
            Debug.LogWarning("Invalid player choice: " + choice);
            return;
        }

        switch (sceneNumber)
        {
            case 1:
                if (scene1Choice != "") return;
                scene1Choice = choice;
                break;

            case 2:
                if (scene2Choice != "") return;
                scene2Choice = choice;
                break;

            case 3:
                if (scene3Choice != "") return;
                scene3Choice = choice;
                break;

            default:
                Debug.LogWarning("Invalid scene number!");
                return;
        }

        Debug.Log(
            "Player " + playerID +
            " chose " + choice +
            " in Scene " + sceneNumber
        );

        SaveChoices();
    }

    private void SaveChoices()
    {
        string[] lines = File.ReadAllLines(filePath);

        string entry =
            playerID + "," +
            startTime + "," +
            scene1Choice + "," +
            scene2Choice + "," +
            scene3Choice;

        bool playerFound = false;

        // Update existing participant row
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].StartsWith(playerID + ","))
            {
                lines[i] = entry;
                playerFound = true;
                break;
            }
        }

        if (playerFound)
        {
            File.WriteAllLines(filePath, lines);
        }
        else
        {
            File.AppendAllText(
                filePath,
                entry + Environment.NewLine
            );
        }

        Debug.Log("Donkerbos choices saved successfully!");
    }
}