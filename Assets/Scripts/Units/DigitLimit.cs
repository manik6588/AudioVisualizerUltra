using Unity.VisualScripting;
using UnityEngine;

[UnitTitle("Digit Limit")]
[UnitCategory("Custom")]
public class DigitLimitUnit : Unit
{
    private ValueInput original;
    private ValueInput digits;
    private ValueOutput limited;
    protected override void Definition()
    {
        original = ValueInput<float>("Original", 0);
        digits = ValueInput<int>("Digits", 0);
        limited = ValueOutput<float>("Limited", (flow) =>
        {
            float originalValue = flow.GetValue<float>(original);
            int digitsValue = flow.GetValue<int>(digits);
            float p = (int)Mathf.Pow(10.0f, digitsValue);
            float r = Mathf.Round(p * originalValue);
            return r / p;
        });
    }
}
