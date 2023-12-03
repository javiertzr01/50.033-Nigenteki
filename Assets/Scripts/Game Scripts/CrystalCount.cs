using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CrystalCount : MonoBehaviour
{
    private float scaleDuration = 0.07f;
    private Vector3 initialScale;
    private Vector3 upScale = new Vector3(1.7f, 1.7f, 1.7f);
    // Start is called before the first frame update
    void Start()
    {
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCrystalCount(int crystalCount)
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"x {crystalCount}";
        StartCoroutine(ScaleUpDown());
    }

    private IEnumerator ScaleUpDown()
    {
        for (float time = 0; time < scaleDuration * 2; time += Time.deltaTime)
        {
            float progress = Mathf.PingPong(time, scaleDuration) / scaleDuration;
            transform.localScale = Vector3.Lerp(initialScale, upScale, progress);
            yield return null;
        }
        transform.localScale = initialScale;
    }
}
