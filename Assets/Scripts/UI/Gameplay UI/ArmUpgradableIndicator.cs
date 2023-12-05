using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArmUpgradableIndicator : MonoBehaviour
{
    float amplitude = 0.5f;

    Vector3 scale;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ScaleUpDown();
    }

    public void ActivateIndicator(bool isUpgradable)
    {
        if (isUpgradable)
            transform.GetComponent<TextMeshProUGUI>().enabled = true;
        else
            transform.GetComponent<TextMeshProUGUI>().enabled = false;
    }

    public void ScaleUpDown()
    {
        transform.localScale = new Vector3(Mathf.PingPong(Time.time, amplitude) + 1, Mathf.PingPong(Time.time, amplitude) + 1, transform.localScale.z);
    }
}
