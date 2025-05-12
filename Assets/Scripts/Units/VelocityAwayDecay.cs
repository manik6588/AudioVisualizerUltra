using Unity.VisualScripting;
using UnityEngine;

[UnitTitle("Velocity Aware Decay")]
public class VelocityAwareDecay : Unit
{
    private ControlInput input;
    private ControlOutput output;

    private ValueInput amp;
    private ValueInput current;
    private ValueInput fallMultiplier;

    private ValueOutput decay;

    private float currentValue = 0.0f;

    protected override void Definition()
    {
        input = ControlInput("in", (flow) =>
        {
            float inp = flow.GetValue<float>(amp);
            float cur = flow.GetValue<float>(current);
            float mul = flow.GetValue<float>(fallMultiplier);

            currentValue = cur;
            float diff = inp - cur;
            if (diff < 0f)
            {
                currentValue += diff * Time.deltaTime * mul; // Drop faster
            }
            else
            {
                currentValue = inp; // Instant rise
            }

            return output;
        });

        output = ControlOutput("out");

        amp = ValueInput<float>("original");
        current = ValueInput<float>("current", 1.0f);
        fallMultiplier = ValueInput<float>("fallMultiplier", 1.0f);

        decay = ValueOutput<float>("multiplied", (flow) => currentValue);

        Succession(input, output);
        Assignment(input, decay);
    }
}
