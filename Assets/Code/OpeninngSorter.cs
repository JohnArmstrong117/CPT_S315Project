using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeninngSorter : MonoBehaviour
{
    public OpeningDatabase od;
    List<Opening> sortedOpenings;

    void Start()
    {
        Invoke("LateStart", 2);
    }

    void LateStart()
    {
        od = gameObject.GetComponent<OpeningDatabase>();
        sortedOpenings = SortOpening(od.GetOpenings());
    }

    private List<Opening> SortOpening(List<Opening> openings)
    {
        //sorting by win percentage for white, and in case of tie by move length
        openings.Sort((a, b) =>
        {
            float winDifference = (a.WhiteWinPercentage) - (b.WhiteWinPercentage);
            if (winDifference != 0) return winDifference > 0 ? -1 : 1;

            return a.Moves.Length.CompareTo(b.Moves.Length);
        });

        return openings;
    }

    public List<Opening> GetSortedOpenings()
    {
        return sortedOpenings;
    }
}
