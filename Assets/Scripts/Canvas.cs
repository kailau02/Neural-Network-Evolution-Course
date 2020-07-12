using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FileInterpreter;

public class Canvas : MonoBehaviour
{
    public bool firstCanvas;
    public int[] stats;
    public Text genTxt;
    public Text fitTxt;

    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("Canvas").Length > 1 && !firstCanvas) Destroy(gameObject);
        else
        {
            firstCanvas = true;
        }
        stats = FileCtrl.GetStatistics();
        genTxt.text = "Generation:\n" + stats[0].ToString();
        fitTxt.text = "Top Fitness:\n" + stats[1].ToString();
    }
    public void ResetPressed()
    {
        FileCtrl.DeleteData();
        Destroy(GameObject.Find("Canvas"));
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
