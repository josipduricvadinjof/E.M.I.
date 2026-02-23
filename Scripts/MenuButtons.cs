using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{

    [SerializeField] private Text bestscoreText;


    private void Awake()
    {
        if(Application.isEditor == false)
            Debug.unityLogger.logEnabled = false;


        if (bestscoreText != null)
        {
            bestscoreText.text = PlayerPrefs.GetInt("EMI_Best_Score", 0).ToString();

        }
    }

    public void LoadScene (string name)
    {
        SceneManager.LoadScene(name);
    }
}
