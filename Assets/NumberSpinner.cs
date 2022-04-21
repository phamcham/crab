using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NumberSpinner : MonoBehaviour
{
    [SerializeField] int minValue;
    [SerializeField] int maxValue;
    [SerializeField] InputField inputField;
    [SerializeField] Button incrementButton;
    [SerializeField] Button decrementButton;

    private int _value;
    public int Value {
        get => _value;
        set => SetValue(value);
    }

    private void Awake() {
        inputField.contentType = InputField.ContentType.IntegerNumber;
        inputField.onValueChanged.AddListener(s => {
            int.TryParse(s, out int num);
            OnValueChanged?.Invoke(_value);
            if (num != _value){
                Debug.Log("aaaa");
                SetValue(num);
            }
        });
        if (!int.TryParse(inputField.text, out _value)){
            inputField.text = _value.ToString();
        }
    }

    private void Start() {
        incrementButton.onClick.AddListener(() => {
            Value++;
        });
        decrementButton.onClick.AddListener(() => {
            Value--;
        });
    }

    private void SetValue(int value){
        _value = Mathf.Clamp(value, minValue, maxValue);
        incrementButton.enabled = decrementButton.enabled = true;

        if (_value <= minValue){
            decrementButton.enabled = false;
        }
        if (_value >= maxValue){
            incrementButton.enabled = false;
        }

        inputField.text = _value.ToString();
    }

    public UnityEvent<int> OnValueChanged;
}
