using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public CharStats[] characterStats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // instance = this;
       // DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
