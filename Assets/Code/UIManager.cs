using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public BoardManager boardManager;
    public GameObject openingTemplate;
    public Transform contentTransform;
    public GameObject pleaseWait;

    void Start()
    {
        Invoke("LateStart", 3.5f);
    }

    void LateStart()
    {
        UpdateOpeningList(boardManager.GetMatchingOpenings());
        DisablePleaseWait();
    }

    public void UpdateOpeningList(List<Opening> openings)
    {
        // Clear existing UI elements
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        // Populate with new openings
        foreach (var opening in openings)
        {
            GameObject openingEntry = Instantiate(openingTemplate, contentTransform);
            openingEntry.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = opening.Name;
            openingEntry.transform.Find("NextMove").GetComponent<TextMeshProUGUI>().text = "Next Move:" + opening.NextMove;
            openingEntry.transform.Find("WhiteWinText").GetComponent<TextMeshProUGUI>().text = "White Win: " + opening.WhiteWinPercentage + "%";
            openingEntry.transform.Find("BlackWinText").GetComponent<TextMeshProUGUI>().text = "Black Win: " + opening.BlackWinPercentage + "%";
        }
    }

    void DisablePleaseWait()
    {
        pleaseWait.SetActive(false);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
