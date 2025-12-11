using UnityEngine;
using System.Collections;

public class AreaEntrance : MonoBehaviour
{
    public string transitionName;

    IEnumerator Start()
    {
        // Espera un frame a que PlayerController2D despierte
        yield return null;

        if (PlayerController2D.instance != null)
        {
            if (transitionName == PlayerController2D.instance.areaTransitionName)
            {
                PlayerController2D.instance.transform.position = transform.position;
            }
        }
        else
        {
            Debug.LogWarning("⚠️ PlayerController2D.instance es nulo en AreaEntrance");
        }

        // También protege UIFade por si acaso
        if (UIFade.instance != null)
        {
            UIFade.instance.FadeFromBlack();
        }
    }
}
