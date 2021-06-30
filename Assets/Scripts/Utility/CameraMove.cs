using UnityEngine;
/// <summary>
/// Behaviour class for moving the camera along with a target
/// </summary>
public class CameraMove : MonoBehaviour
{
    /**************** VARIABLES *******************/
    [SerializeField] private Transform followTransform;
    /**********************************************/
    
    /******************* INIT *********************/
    private void Awake()
    {
        EventBroker.LevelReadyEvent += FindPlayer;
    }
    
    private void FindPlayer()
    {
        followTransform = FindObjectOfType<PlayerMovement>().transform;
    }
    /**********************************************/
    
    /******************* LOOP *********************/
    private void LateUpdate()
    {
        transform.position = new Vector3(followTransform.position.x, followTransform.position.y, transform.position.z);
    }
    /**********************************************/
}