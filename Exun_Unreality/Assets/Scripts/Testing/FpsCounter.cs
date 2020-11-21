using System.Collections;
using UnityEngine;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text = null;

    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(.33f);
            int fps = (int) (1f / Time.unscaledDeltaTime);
            text.SetText(fps.ToString());
        }
    }
}
