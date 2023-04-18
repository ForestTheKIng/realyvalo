using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Serialization;

public class Neon : MonoBehaviour
{
    public GameObject outline;
    public GameObject pp;
    public GameObject steps;
    const float maxEnergy = 100f;
    public float currentEnergy = maxEnergy;
    [SerializeField] Image energyImage;
    [FormerlySerializedAs("player")] [FormerlySerializedAs("movement")] public LocalPlayer localPlayer;
    public float NeonModeSpeed = 7.0f;
    public GameObject energyBar;
    PhotonView pv;
    public bool inNeonMode;

    void Awake(){
        pv = GetComponent<PhotonView>();    
    }

    void Update()
    {
        if (pv.IsMine){
            energyImage.fillAmount = currentEnergy / maxEnergy;
            if (Input.GetKey(KeyCode.LeftShift) && currentEnergy >= 0){
                NeonMode();
            } else
            {
                inNeonMode = false;
                if (currentEnergy <= 100){
                    currentEnergy += 2 * Time.deltaTime;
                }
                pp.SetActive(false);
                outline.SetActive(false);
                steps.GetComponent<ParticleSystem>().Pause();
                steps.SetActive(false);
            }
        }
    }


    void NeonMode()
    {
        inNeonMode = true;
        energyBar.SetActive(true);
        currentEnergy -= 7 * Time.deltaTime;
        if (energyImage != null){
            localPlayer.MoveSpeed = NeonModeSpeed;
            pp.SetActive(true);
            steps.SetActive(true);
            steps.GetComponent<ParticleSystem>().Play();
            outline.SetActive(true);
        }
    }
}
