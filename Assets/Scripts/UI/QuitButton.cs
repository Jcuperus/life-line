using UnityEngine;

namespace UI
{
    public class QuitButton : MonoBehaviour
    {
        private void OnMouseDown()
        {
            Application.Quit();
        }
    }
}