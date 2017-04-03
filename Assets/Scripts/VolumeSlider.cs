using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour {

    

	public void SetVolume()
    {
        AudioListener.volume = GetComponent<Slider>().value;
    }
}
