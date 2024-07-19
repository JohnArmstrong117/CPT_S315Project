using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OpeningDatabase : MonoBehaviour
{
    public TextAsset csvFile;
    private List<Opening> openings;
    void Start()
    {
        LoadOpenings();
    }

    void LoadOpenings()
    {
        openings = new List<Opening>();
        StringReader stringReader = new StringReader(csvFile.text);
        bool isFirst = true;

        while (stringReader.Peek() != -1)
        {
            //Skipping the header info
            string line = stringReader.ReadLine();
            if (isFirst)
            {
                isFirst = false;
                continue;
            }

            string[] values = ParseCsvLine(line);
            if (values.Length >= 20)
            {
                //Removing the quotes around moves
                string moves = values[10].Trim();

                // Removing '[' and ']' from the start and end of moves
                if (moves.StartsWith("["))
                {
                    moves = moves.Substring(1);
                }
                if (moves.EndsWith("]"))
                {
                    moves = moves.Substring(0, moves.Length - 1);
                }

                Opening opening = new Opening
                {
                    Name = values[0],
                    Color = values[1],
                    Moves = moves,
                    NextMove = "N/A",
                    WhiteWinPercentage = float.Parse(values[19]),
                    BlackWinPercentage = float.Parse(values[20])
                };
                openings.Add(opening);
            }
        }
    }

    //method for avoiding parsing mistakes.
    //Avoids spliting at commas that are withing quotation blocks
    // necessarry because of the name and move list fields
    string[] ParseCsvLine(string line)
    {
        List<string> values = new List<string>();
        bool inQuotes = false;
        string currentField = string.Empty;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                values.Add(currentField);
                currentField = string.Empty;
            }
            else
            {
                currentField += c;
            }
        }

        values.Add(currentField); // Add the last field
        return values.ToArray();
    }

    public List<Opening> GetOpenings()
    {
        return openings;
    }
}
