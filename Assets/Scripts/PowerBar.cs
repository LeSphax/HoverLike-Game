using System;
using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{

    public float powerValue;
    public const float FILLING_INCREMENT = 1.5f;
    public GameObject sliderPrefab;

    private Slider slider;

    private enum State
    {
        HIDDEN,
        FILLING,
    }

    private State _state;
    private State state
    {
        get { return _state; }
        set
        {
            switch (value)
            {

                case State.HIDDEN:
                    powerValue = 0;
                    DestroySlider();
                    break;
                case State.FILLING:
                    powerValue = 0f;
                    slider = Instantiate(sliderPrefab).GetComponent<Slider>();
                    slider.transform.SetParent(MyComponents.UI().transform, false);
                    break;
            }
            _state = value;
        }
    }

    private void DestroySlider()
    {
        if (slider != null)
            Destroy(slider.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        state = State.HIDDEN;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.HIDDEN:
                break;
            case State.FILLING:
                powerValue = Mathf.Min(1.0f, powerValue + FILLING_INCREMENT * Time.deltaTime);
                slider.value = powerValue;
                break;
        }

    }

    public void StartFilling()
    {
        state = State.FILLING;
    }

    public void Hide()
    {
        state = State.HIDDEN;
    }

    public bool IsFilling()
    {
        return state == State.FILLING;
    }

    void OnDestroy()
    {
        DestroySlider();
    }
}
