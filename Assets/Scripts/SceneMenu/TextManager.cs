using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    
    public bool writting;

    //récupère tous les textes pour ne pas tous les afficher en meme temps 
    [SerializeField] private TextMeshProUGUI[] textMeshProUGUIs;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        writting = false;
        StartCoroutine(ShowMenu());
    }



    private IEnumerator TextAnimation( TextMeshProUGUI textMeshProUGUI)
    {
        string currentText = textMeshProUGUI.text;
        textMeshProUGUI.text = "";
        textMeshProUGUI.enabled = true;
        writting = true;
        foreach (char c in currentText)
        {
            textMeshProUGUI.text += "" + c;
            for (int i = 0; i < 7; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        writting = false;
        
    }

  
    private IEnumerator ShowMenu()
    {
        for (int i = 0;i < textMeshProUGUIs.Length; i++)
        {
            
            StartCoroutine(TextAnimation(textMeshProUGUIs[i]));
            yield return new WaitUntil(() => !writting);
        }
        
    }

   

    

}
