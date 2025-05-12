using Unity.VisualScripting;
using UnityEngine;

public class AmplitudeMultiplier : Unit
{
    private ValueInput original;
    private ValueInput multiplier;

    private ValueOutput multiplied;

    protected override void Definition()
    {
        original = ValueInput<float>("original");
        multiplier = ValueInput<float>("multiplier", 1.0f);
        multiplied = ValueOutput<float>("multiplied", (flow) =>
        {
            float org = flow.GetValue<float>(original);
            float mult = flow.GetValue<float>(multiplier);
            return Multiply(org, mult);
        });
    }

    private float Multiply(float org, float mult)
    {
        float multiplier = Mathf.Clamp(mult, 0, 1);
        return Mathf.Clamp(org * multiplier, 0, 1);
    }
}
