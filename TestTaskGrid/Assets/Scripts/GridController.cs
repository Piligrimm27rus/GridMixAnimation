using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] InputField heightField;
    [SerializeField] InputField widthField;

    [SerializeField] GameObject letterPrefab;
    [SerializeField] Canvas canvas;

    private RectTransform[,] lettersArray;
    private string letterLine;

    //для перемешивания
    private float nextPushBtnTime;
    private float pushRate = 2f;
    private float timeMix = 0.0625f;

    [Range(.5f, .8f)]
    [SerializeField] float proportionX = 0.8f;
    [Range(.5f, .8f)]
    [SerializeField] float proportionY = 0.8f;

    private void Start()
    {
        heightField = heightField.GetComponent<InputField>();
        widthField = widthField.GetComponent<InputField>();
    }


    private void FillElementsInCanvas(string line, int p_width, int p_height)
    {

        float spaceBetweenX = canvas.GetComponent<RectTransform>().rect.width * proportionX / p_width; //пробелы между буквами
        float spaceBetweenY = canvas.GetComponent<RectTransform>().rect.height * proportionY / p_height; //пробелы между буквами

        for (int i = 0; i < p_height; i++)
        {
            for (int k = 0; k < p_width; k++)
            {
                GameObject letter_ = Instantiate(letterPrefab, canvas.transform);

                letter_.GetComponent<Text>().text += line[i + k];

                RectTransform letterRectTransform = letter_.GetComponent<RectTransform>();
                lettersArray[i, k] = letterRectTransform;

                letterRectTransform.anchoredPosition = new Vector2(letterRectTransform.anchoredPosition.x - spaceBetweenX * p_width / 2, letterRectTransform.anchoredPosition.y + spaceBetweenY * p_height / 2); //элемент в левом верхнем углу

                Vector2 offset = new Vector2(letterRectTransform.anchoredPosition.x + spaceBetweenX * (k + 0.5f), letterRectTransform.anchoredPosition.y - spaceBetweenY * (i + 0.5f));

                letterRectTransform.anchoredPosition = offset;
            }
        }
    }

    public void GenerateBtn()
    {
        if (!string.IsNullOrEmpty(heightField.text) && !string.IsNullOrEmpty(widthField.text))
        {
            letterLine = "";
            if (lettersArray != null)
                DeleteLettersInScene();

            int height = int.Parse(heightField.text);
            int width = int.Parse(widthField.text);

            lettersArray = new RectTransform[height, width];

            for (int i = 0; i < height * width; i++)
            {
                letterLine += (char)(Random.Range(65, 91));
            }

            FillElementsInCanvas(letterLine, width, height);
        }
    }

    IEnumerator LerpLetter(RectTransform rectTransform, Vector2 endPosition)
    {
        while (rectTransform.anchoredPosition != endPosition)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, endPosition, timeMix);
            yield return null;
        }
    }

    public void MixBtn()
    {
        if (string.IsNullOrEmpty(letterLine))
            return;

        if (Time.time >= nextPushBtnTime)
        {
            StopAllCoroutines();
            Vector2[,] mixLetterArray = GenerateNewRandomVectorArray(lettersArray);


            for (int i = 0; i < lettersArray.GetLength(0); i++)
            {
                for (int k = 0; k < lettersArray.GetLength(1); k++)
                {

                    StartCoroutine(LerpLetter(lettersArray[i, k], mixLetterArray[i, k]));
                }
            }
            nextPushBtnTime = Time.time + pushRate;
        }
    }

    private Vector2[,] GenerateNewRandomVectorArray(RectTransform[,] array)
    {
        Vector2[,] vectorsArray = new Vector2[array.GetLength(0), array.GetLength(1)];
        Vector2[] tempArray = new Vector2[array.GetLength(0) * array.GetLength(1)];
        int temp = 0;

        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int k = 0; k < array.GetLength(1); k++)
            {
                tempArray[temp] = array[i, k].anchoredPosition;
                temp++;
            }
        }

        for (int i = tempArray.Length - 1; i >= 1; i--)
        {
            int j = Random.Range(0, i + 1);
            Vector2 tempV = tempArray[j];
            tempArray[j] = tempArray[i];
            tempArray[i] = tempV;
        }

        temp = 0;
        for (int i = 0; i < array.GetLength(0); ++i)
            for (int j = 0; j < array.GetLength(1); ++j)
            {
                vectorsArray[i, j] = tempArray[temp];
                temp++;
            }

        return vectorsArray;
    }

    private void DeleteLettersInScene()
    {
        StopAllCoroutines();
        for (int i = 0; i < lettersArray.GetLength(0); i++)
        {
            for (int k = 0; k < lettersArray.GetLength(1); k++)
            {
                Destroy(lettersArray[i, k].gameObject);
            }
        }
    }
}
