using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSort : MonoBehaviour
{
    public GameObject barPrefab;
    public int numberOfBars = 100;
    public float swapSpeed = 0.5f;
    private GameObject[] bars;
    private int[] array;

    void Start()
    {

    }

    public void OnRunButtonClick()
    {
        GenerateArray();
        StartCoroutine(RunBubbleSort());
    }

    void GenerateArray()
    {
        array = new int[numberOfBars];
        bars = new GameObject[numberOfBars];

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
            array[i] = Random.Range(1, 100);

            // Instantiate bar prefab
            bars[i] = Instantiate(barPrefab);

            // Calculate bar height
            float barHeight = (array[i] / 100f) * maxHeight;

            // Calculate x position (with spacing)
            float xPos = -cameraWidth / 2f + i * barWidth + (barWidth / 2f);

            // Set y position to 0 (aligned to ground since the pivot is now at the bottom)
            float yPos = 0;

            // Set position with yPos fixed at 0
            bars[i].transform.position = new Vector3(xPos, yPos, 0);

            // Adjust the scale to change only the height
            bars[i].transform.localScale = new Vector3(barWidth * 0.6f, barHeight, 1); // Scaling height while leaving the bottom aligned
        }
    }




    IEnumerator RunBubbleSort()
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            for (int j = 0; j < array.Length - i - 1; j++)
            {
                bars[j].GetComponent<SpriteRenderer>().color = Color.red;
                bars[j + 1].GetComponent<SpriteRenderer>().color = Color.red;

                if (array[j] > array[j + 1])
                {
                    yield return SwapBars(j, j + 1);
                }

                bars[j].GetComponent<SpriteRenderer>().color = Color.white;
                bars[j + 1].GetComponent<SpriteRenderer>().color = Color.white;
            }
            bars[array.Length - i - 1].GetComponent<SpriteRenderer>().color = Color.green;
        }
        bars[0].GetComponent<SpriteRenderer>().color = Color.green;
    }

    IEnumerator SwapBars(int indexA, int indexB)
    {
        Vector3 posA = bars[indexA].transform.position;
        Vector3 posB = bars[indexB].transform.position;

        float elapsedTime = 0f;
        while (elapsedTime < swapSpeed)
        {
            bars[indexA].transform.position = Vector3.Lerp(posA, posB, elapsedTime / swapSpeed);
            bars[indexB].transform.position = Vector3.Lerp(posB, posA, elapsedTime / swapSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bars[indexA].transform.position = posB;
        bars[indexB].transform.position = posA;

        GameObject tempBar = bars[indexA];
        bars[indexA] = bars[indexB];
        bars[indexB] = tempBar;

        int temp = array[indexA];
        array[indexA] = array[indexB];
        array[indexB] = temp;
    }
}

