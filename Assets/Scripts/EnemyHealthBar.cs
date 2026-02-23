using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image fillImage;
    public Transform target;

    public void SetHealth(int current, int max)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = (float)current / max;
        }
    }

    void LateUpdate()
    {
        //if (target != null)
        //{
        //    transform.position = target.position + new Vector3(0, 1.2f, 0);
        //    transform.rotation = Quaternion.identity;
        //}
    }
}
