using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] InputField heightInputField;
    [SerializeField] InputField widthInputField;

    [SerializeField] GameObject letterPrefab;
    [SerializeField] Canvas canvas;

    private RectTransform[,] lettersInCanvasArray;
    private RectTransform canvasRectTransform;
    private string letterLine;

    //Для перемешивания букв
    private float nextPushBtnTime;
    private float pushRate = 2f;
    private float lerpTime = 0.0625f;

    //Пропорции сетки для букв
    [Range(.5f, .8f)]
    [SerializeField] float proportionGridX = 0.8f;
    [Range(.5f, .8f)]
    [SerializeField] float proportionGridY = 0.8f;

    private void Start()
    {
        heightInputField = heightInputField.GetComponent<InputField>();
        widthInputField = widthInputField.GetComponent<InputField>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    #region Buttons
    public void GenerateBtn()
    {
        if (!string.IsNullOrEmpty(heightInputField.text) && !string.IsNullOrEmpty(widthInputField.text))
        {
            letterLine = "";
            if (lettersInCanvasArray != null)
                DeleteLettersInScene();

            int height = int.Parse(heightInputField.text);
            int width = int.Parse(widthInputField.text);

            lettersInCanvasArray = new RectTransform[height, width];

            for (int i = 0; i < height * width; i++)
            {
                letterLine += (char)(Random.Range(65, 91));
            }

            FillElementsInCanvas(letterLine, width, height);
        }
    }

    public void MixBtn()
    {
        if (string.IsNullOrEmpty(letterLine))
            return;

        if (Time.time >= nextPushBtnTime)
        {
            StopAllCoroutines();
            Vector2[,] newPlaceLettersArray = GenerateNewRandomVectorArray(lettersInCanvasArray);


            for (int i = 0; i < lettersInCanvasArray.GetLength(0); i++)
            {
                for (int k = 0; k < lettersInCanvasArray.GetLength(1); k++)
                {
                    StartCoroutine(LerpLetter(lettersInCanvasArray[i, k], newPlaceLettersArray[i, k]));
                }
            }
            nextPushBtnTime = Time.time + pushRate;
        }
    }
    #endregion

    #region methods
    private void FillElementsInCanvas(string line, int p_width, int p_height)
    {

        float spaceBetweenLettersX = canvasRectTransform.rect.width * proportionGridX / p_width; //пробелы между буквами
        float spaceBetweenLettersY = canvasRectTransform.rect.height * proportionGridY / p_height; //пробелы между буквами

        for (int i = 0; i < p_height; i++)
        {
            for (int k = 0; k < p_width; k++)
            {
                GameObject letter_ = Instantiate(letterPrefab, canvas.transform);

                letter_.GetComponent<Text>().text += line[i + k];

                RectTransform letterRectTransform = letter_.GetComponent<RectTransform>();
                lettersInCanvasArray[i, k] = letterRectTransform;

                letterRectTransform.anchoredPosition = new Vector2(letterRectTransform.anchoredPosition.x - spaceBetweenLettersX * p_width / 2, letterRectTransform.anchoredPosition.y + spaceBetweenLettersY * p_height / 2); //элемент в левом верхнем углу

                Vector2 offset = new Vector2(letterRectTransform.anchoredPosition.x + spaceBetweenLettersX * (k + 0.5f), letterRectTransform.anchoredPosition.y - spaceBetweenLettersY * (i + 0.5f));

                letterRectTransform.anchoredPosition = offset;
            }
        }
    }

    private Vector2[,] GenerateNewRandomVectorArray(RectTransform[,] array)
    {
        Vector2[,] vectorsArray = new Vector2[array.GetLength(0), array.GetLength(1)];
        Vector2[] tempArray = new Vector2[array.GetLength(0) * array.GetLength(1)];
        int temp = 0;

        for (int i = 0; i < array.GetLength(0); i++) //делает одномерный массив
        {
            for (int k = 0; k < array.GetLength(1); k++)
            {
                tempArray[temp] = array[i, k].anchoredPosition;
                temp++;
            }
        }

        for (int i = tempArray.Length - 1; i >= 1; i--) //перемешивает его
        {
            int j = Random.Range(0, i + 1);
            Vector2 tempV = tempArray[j];
            tempArray[j] = tempArray[i];
            tempArray[i] = tempV;
        }

        temp = 0;
        for (int i = 0; i < array.GetLength(0); ++i) //из одномерного в двумерный
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
        for (int i = 0; i < lettersInCanvasArray.GetLength(0); i++)
        {
            for (int k = 0; k < lettersInCanvasArray.GetLength(1); k++)
            {
                Destroy(lettersInCanvasArray[i, k].gameObject);
            }
        }
    }
    #endregion

    #region IEnumerator
    IEnumerator LerpLetter(RectTransform letter, Vector2 endPosition)
    {
        while (letter.anchoredPosition != endPosition)
        {
            letter.anchoredPosition = Vector2.Lerp(letter.anchoredPosition, endPosition, lerpTime);
            yield return null;
        }
    }
    #endregion
}
