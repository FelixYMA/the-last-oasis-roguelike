using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RandomGlobalLightFlicker : MonoBehaviour
{
    private Light2D globalLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 0.2f;

    private float timer;
    private float targetIntensity;
    private Color m_Color;

    void Start()
    {
        globalLight = GetComponent<Light2D>();
        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }

    void Update()
    {
        if (globalLight != null)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                targetIntensity = Random.Range(minIntensity, maxIntensity);
                timer = flickerSpeed;
            }
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, targetIntensity, Time.deltaTime * 1);
        }
    }
}
