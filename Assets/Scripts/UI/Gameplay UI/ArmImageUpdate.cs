using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmImageUpdate : MonoBehaviour
{
    public Sprite[] armSprites; // 0: Basic Shooter, 1: Beetle, 2: Silkworm, 3: HoneyBee, 4: Locust
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateArmImage(int armIndex)
    {
        transform.GetComponent<Image>().sprite = armSprites[armIndex]; 
    }
}
