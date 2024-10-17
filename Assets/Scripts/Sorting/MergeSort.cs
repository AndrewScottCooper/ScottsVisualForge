using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MergeSort : MonoBehaviour
{

    public GameObject barPrefab;
    public int numberOfBars = 100;
    public float swapSpeed = 0.5f;
    private List<GameObject> bars = new List<GameObject>();
    private int[] array;

    private void Start()
    {
        GenerateArray();
        RunMergeSort();
    }
    void GenerateArray()
    {
        array = new int[numberOfBars];

        // Calculate camera's world units for width and height
        float cameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float cameraHeight = Camera.main.orthographicSize;

        // Padding and bar width setup
        float totalPadding = cameraWidth * 0.1f; // 10% padding of total width
        float totalAvailableWidth = cameraWidth - totalPadding; // Available width for bars
        float barWidth = totalAvailableWidth / numberOfBars;

        // Set maximum height of bars to 90% of camera height
        float maxHeight = cameraHeight * 0.6f;

        for (int i = 0; i < numberOfBars; i++)
        {
            // Generate random values for bar height
            array[i] = UnityEngine.Random.Range(1, 100);


            bars.Add(Instantiate(barPrefab));
            // Calculate bar height
            float barHeight = (array[i] / 100f) * maxHeight;

            // Calculate x position (with spacing)
            float xPos = -cameraWidth / 2f + i * barWidth + (barWidth / 2f);
            float yPos = 0;
            bars[i].transform.position = new Vector3(xPos, yPos, 0);

            // Adjust the scale to change only the height
            bars[i].transform.localScale = new Vector3(barWidth * 0.6f, barHeight, 1); // Scaling height while leaving the bottom aligned

        }
    }
    public void RunMergeSort()
    {
        StartCoroutine(Split(bars));
    }
    public IEnumerator Split(List<GameObject> barList)
    {
        if (barList.Count > 1)
        {
            // Split into two halves
            List<GameObject> firstHalf = barList.GetRange(0, barList.Count / 2);
            List<GameObject> secondHalf = barList.GetRange(barList.Count / 2, barList.Count - barList.Count / 2);

            // Recursively split both halves
            yield return StartCoroutine(Split(firstHalf));
            yield return StartCoroutine(Split(secondHalf));

            // Merge the two halves back together
            yield return StartCoroutine(Merge(firstHalf, secondHalf, barList));
        }
    }

    public IEnumerator Merge(List<GameObject> first, List<GameObject> second, List<GameObject> original)
    {
        int i = 0, j = 0, k = 0;

        // Preserve original x-positions
        List<Vector3> originalPositions = original.Select(bar => bar.transform.position).ToList();

        // Start merging the two halves while animating the comparisons
        while (i < first.Count && j < second.Count)
        {
            // Color the bars being compared in yellow
            first[i].GetComponent<Renderer>().material.color = Color.yellow;
            second[j].GetComponent<Renderer>().material.color = Color.yellow;

            yield return new WaitForSeconds(0.05f); // Wait to visualize comparison

            // Compare the height (y scale) of the bars
            if (first[i].transform.localScale.y < second[j].transform.localScale.y)
            {
                // Move the bar from first list to its correct position in the original list
                StartCoroutine(SwapPositions(first[i], originalPositions[k]));
                i++;
            }
            else
            {
                // Move the bar from second list to its correct position in the original list
                StartCoroutine(SwapPositions(second[j], originalPositions[k]));
                j++;
            }

            k++;  // Move to the next position in the merged array
            yield return new WaitForSeconds(0.1f); // Pause to visualize movement
        }

        // Add any remaining bars in `first`
        while (i < first.Count)
        {
            StartCoroutine(SwapPositions(first[i], originalPositions[k]));
            i++;
            k++;
        }

        // Add any remaining bars in `second`
        while (j < second.Count)
        {
            StartCoroutine(SwapPositions(second[j], originalPositions[k]));
            j++;
            k++;
        }

        // Once the merge is done, turn all the bars in `original` green (indicating they are sorted)
        for (int m = 0; m < original.Count; m++)
        {
            original[m].GetComponent<Renderer>().material.color = Color.green;
        }

        yield return null;
    }

    IEnumerator SwapPositions(GameObject bar, Vector3 targetPosition)
    {
        Vector3 startPosition = bar.transform.position;

        // Color the moving bar red
        bar.GetComponent<Renderer>().material.color = Color.red;

        float elapsedTime = 0f;
        float duration = 0.1f; // Adjust for faster/slower animation

        // Animate the bar's movement to its new position
        while (elapsedTime < duration)
        {
            bar.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap the bar's position to the target position
        bar.transform.position = targetPosition;

        yield return null;
    }

}
