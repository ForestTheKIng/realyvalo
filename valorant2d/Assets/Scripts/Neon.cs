using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Neon : MonoBehaviour
{
    public GameObject outline;
    public GameObject pp;
    public GameObject steps;
    const float maxEnergy = 100f;
    public float currentEnergy = maxEnergy;
    [SerializeField] Image energyImage;
    public Movement movement;
    public float NeonModeSpeed = 7.0f;
    public GameObject energyBar;


    void Update()
    {
        energyImage.fillAmount = currentEnergy / maxEnergy;
        if (Input.GetKey(KeyCode.LeftShift) && currentEnergy >= 0){
            NeonMode();
        } else {
            if (currentEnergy <= 100){
                currentEnergy += 2 * Time.deltaTime;
            }
            movement.moveSpeed = 5f;
            pp.SetActive(false);
            outline.SetActive(false);
            steps.GetComponent<ParticleSystem>().Pause();
            steps.SetActive(false);
        }
    }


    void NeonMode() {
        energyBar.SetActive(true);
        currentEnergy -= 7 * Time.deltaTime;
        if (energyImage != null){
            movement.moveSpeed = NeonModeSpeed;
            pp.SetActive(true);
            steps.SetActive(true);
            steps.GetComponent<ParticleSystem>().Play();
            outline.SetActive(true);
        }
    }
}
