using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{
    public GameObject UIScreen;
    public GameObject Player;
    public GameObject GameManager;
    void Start()
    {
        // Evita duplicar UIFade
        if (UIFade.instance == null && UIScreen != null)
        {
            var uiClone = Instantiate(UIScreen);
            UIFade.instance = uiClone.GetComponent<UIFade>();
        }

        // Evita duplicar Player
        if (PlayerController2D.instance == null && Player != null)
        {
            var playerClone = Instantiate(Player).GetComponent<PlayerController2D>();
            PlayerController2D.instance = playerClone;
        }
        //if (GameManager.instance == null && GameManager != null)
        //{
        //    var gmClone = Instantiate(GameManager).GetComponent<GameManager>();
        //    GameManager.instance = gmClone;
        //}
    }
}
