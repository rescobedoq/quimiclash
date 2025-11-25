using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public Text dialogText;
    public Text nameText;
    public GameObject dialogBox;
    public GameObject nameBox;

    public string [] dialogLines;
    public int currentLine;

    public static DialogManager instance;

    private bool justStarted;
    void Start()
    {
        instance = this;
        dialogBox.SetActive(false);
        nameBox.SetActive(false);
        //dialogText.text = dialogLines[currentLine]; 
    }

    // Update is called once per frame
    void Update()
    {
        if(dialogBox.activeInHierarchy)
        {
            if (Input.GetButtonUp("Fire2"))
            {
                if (!justStarted)
                {
                    currentLine++;
                    if (currentLine >= dialogLines.Length)
                    {
                        dialogBox.SetActive(false);
                        nameBox.SetActive(false);
                        currentLine = 0;
                        PlayerController2D.instance.canMove = true; 
                    }
                    else
                    {
                        CheckIfName();
                        dialogText.text = dialogLines[currentLine];
                    }

                }
                else
                {
                    justStarted = false;
                    dialogText.text = dialogLines[currentLine];
                }
            }
        }
    }
    public void showDialog(string[] newLines)
    {
        dialogLines = newLines;
        currentLine = 0;
        
        CheckIfName();
        dialogText.text = dialogLines[currentLine];

        dialogBox.SetActive(true);  
        justStarted = true;

        PlayerController2D.instance.canMove = false;
    }

    public void CheckIfName()
    {
        if (dialogLines[currentLine].StartsWith("n-"))
        {
            nameBox.SetActive(true);
            nameText.text = dialogLines[currentLine].Substring(2); // Quita "n-"
            currentLine++;
        }
        else
        {
            nameBox.SetActive(false);
        }
    }

}
