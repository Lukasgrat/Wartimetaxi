using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI : MonoBehaviour
{
    //Runs the AI for one move
    public abstract bool makeMove(Player currentPlayer, Player opposingPlayer,
        UnitGenerator unitGenerator, GameObject cards, int maxCardCount);



    //Swaps two elements in a list
    void swap<T>(List<T> x, int index1, int index2)
    {
        T item = x[index1];
        x[index1] = x[index2];
        x[index2] = item;
    }
    // Returns the index where the pivot element ultimately ends up in the sorted source
    // EFFECT: Modifies the source list in the range [loIdx, hiIdx) such that
    //         all values to the left of the pivot are less than (or equal to) the pivot
    //         and all values to the right of the pivot are greater than it
    int partition(List<Action> source, int loIdx, int hiIdx, int pivot, List<Tile> prioritizedTiles)
    {
        int curLo = loIdx;
        int curHi = hiIdx - 1;
        while (curLo < curHi)
        {
            // Advance curLo until we find a too-big value (or overshoot the end of the list)
            while (curLo < hiIdx && source[curLo].value(prioritizedTiles) >= pivot)
            {
                curLo = curLo + 1;
            }
            // Advance curHi until we find a too-small value (or undershoot the start of the list)
            while (curHi >= loIdx && source[curHi].value(prioritizedTiles) < pivot)
            {
                curHi = curHi - 1;
            }
            if (curLo < curHi)
            {
                swap(source, curLo, curHi);
            }
        }
        swap(source, loIdx, curHi); // place the pivot in the remaining spot
        return curHi;
    }


    // EFFECT: Sorts the given ArrayList according to the given comparator
    internal void quicksort(List<Action> arr, List<Tile> prioritizedTiles)
    {
        quicksortHelp(arr, 0, arr.Count, prioritizedTiles);
    }

    // EFFECT: sorts the source array according to comp, in the range of indices [loIdx, hiIdx)
    void quicksortHelp(List<Action> source, int loIdx, int hiIdx, List<Tile> prioritizedTiles)
    {
        if (loIdx >= hiIdx)
        {
            return;
        }
        int pivot = source[loIdx].value(prioritizedTiles);
        int pivotIdx = partition(source, loIdx, hiIdx, pivot, prioritizedTiles);
        // Step 3: sort both halves of the list
        quicksortHelp(source, loIdx, pivotIdx, prioritizedTiles);
        quicksortHelp(source, pivotIdx + 1, hiIdx, prioritizedTiles);
    }


}
