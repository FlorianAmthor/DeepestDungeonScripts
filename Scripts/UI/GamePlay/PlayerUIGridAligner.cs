using UnityEngine;
using UnityEngine.UI;

public class PlayerUIGridAligner : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        GetComponent<GridLayoutGroup>().cellSize = new Vector2(Screen.width / 3, GetComponent<RectTransform>().sizeDelta.y);
    }
}
