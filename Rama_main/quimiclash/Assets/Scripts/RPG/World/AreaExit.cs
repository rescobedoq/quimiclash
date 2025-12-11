using UnityEngine;

public class AreaExit : MonoBehaviour
{           
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string areaToLoad;
    public string areaTransitionName;
    public AreaEntrance entrance;   
    public float waitToLoad = 1f;
    private bool shouldLoadAfterFade;

    void Start()
    {
        entrance.transitionName = areaTransitionName;

    }

    // Update is called once per frame
    void Update()
    {
        if(shouldLoadAfterFade)
        {
            waitToLoad -= Time.deltaTime;
            if(waitToLoad <= 0)
            {
                shouldLoadAfterFade = false; 
                UnityEngine.SceneManagement.SceneManager.LoadScene(areaToLoad);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            shouldLoadAfterFade = true; 
            UIFade.instance.FadeToBlack();  

            PlayerController2D.instance.areaTransitionName = areaTransitionName;
        }
    }
}
