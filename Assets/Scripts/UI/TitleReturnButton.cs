using UnityEngine;

public class TitleReturnButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameManager.Instance.ReturnToMenu();
    }
}
