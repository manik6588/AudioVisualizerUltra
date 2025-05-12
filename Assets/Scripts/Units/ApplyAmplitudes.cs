using Unity.VisualScripting;
using UnityEngine;

[UnitTitle("AnimateBands")]
[UnitCategory("Custom")]
public class ApplyBandAmplitudes : Unit
{
    private ControlInput inputTrigger;
    private ControlOutput outputTrigger;

    private ValueInput cubes;
    private ValueInput mirroredCubes;
    private ValueInput amplitudes;
    private ValueInput multiplier;
    private ValueInput minHeight;
    private ValueInput maxHeight;
    private ValueInput offset;
    private ValueInput upSpeed;
    private ValueInput downSpeed;
    private ValueInput rotationFactor;

    private ValueOutput gameObjects;
    private ValueOutput mirroredGameObjects;

    private GameObject spawned;
    private GameObject mSpawned;

    protected override void Definition()
    {
        inputTrigger = ControlInput("in", (flow) =>
        {
            GameObject cub = flow.GetValue<GameObject>(cubes);
            GameObject mcub = flow.GetValue<GameObject>(mirroredCubes);
            float amps = flow.GetValue<float>(amplitudes);
            float multi = flow.GetValue<float>(multiplier);
            float minH = flow.GetValue<float>(minHeight);
            float maxH = flow.GetValue<float>(maxHeight);
            float offs = flow.GetValue<float>(offset);
            float adjUp = flow.GetValue<float>(upSpeed);
            float adjDown = flow.GetValue<float>(downSpeed);
            float rotFac = flow.GetValue<float>(rotationFactor);

            Apply(cub, mcub, amps, multi, minH, maxH, offs, adjUp, adjDown, rotFac);

            return outputTrigger;
        });

        outputTrigger = ControlOutput("out");

        cubes = ValueInput<GameObject>("cube");
        mirroredCubes = ValueInput<GameObject>("mirroredCube");
        amplitudes = ValueInput<float>("amplitude");
        multiplier = ValueInput<float>("multiplier", 1.0f);
        minHeight = ValueInput<float>("minHeight", 0.0f);
        maxHeight = ValueInput<float>("maxHeight", 1.0f);
        offset = ValueInput<float>("offset", 0.0f);
        upSpeed = ValueInput<float>("upSpeed", 1.0f);
        downSpeed = ValueInput<float>("downSpeed", 1.0f);
        rotationFactor = ValueInput<float>("rotationFactor", 1.0f);

        gameObjects = ValueOutput<GameObject>("objects", (flow) => spawned);
        mirroredGameObjects = ValueOutput<GameObject>("mirrored", (flow) => mSpawned);

        Succession(inputTrigger, outputTrigger);
        Assignment(inputTrigger, gameObjects);
        Assignment(inputTrigger, mirroredGameObjects);
    }

    private void Apply(GameObject cub, GameObject mcub, float amp, float multi, float minH, float maxH, float offs, float adjUp, float adjDown, float rotFac)
    {
        if (cub == null || mcub == null)
        {
            Debug.LogError("One or more input GameObjects are null.");
            return;
        }

        // --- Raw Input Visualization ---
        float input = amp * multi; // No clamp — allows overshooting >1 or below 0

        // --- Velocity-aware Decay ---
        Vector3 pos = cub.transform.localPosition;
        float currentY = pos.y;

        // Instead of clamped Lerp, use direct scaling
        float targetY = (maxH - minH) * input + minH + offs;

        float diff = targetY - currentY;
        float speed;

        float fallMultiplier = 0.2f; // Tweak this for responsiveness


        if (diff > 0f)
        {
            speed = adjUp; // Constant upward speed
        }
        else
        {
            // Non-linear fall: fast at top, slows near target
            //float fallStrength = Mathf.Sqrt(Mathf.Abs(diff)) * fallMultiplier;
            float fallStrength = Mathf.Pow(Mathf.Abs(diff), 0.75f) * fallMultiplier;
            speed = adjDown + fallStrength;
        }


        pos.y = Mathf.MoveTowards(currentY, targetY, speed * Time.deltaTime);
        cub.transform.localPosition = pos;
        mcub.transform.localPosition = new Vector3(pos.x, -pos.y, pos.z);

        // --- Rotation ---
        float rotSpeed = (amp > 0f) ? speed * rotFac : rotFac / 0.6f;
        Quaternion deltaRot = Quaternion.Euler(0f, rotSpeed, 0f);
        cub.transform.localRotation *= deltaRot;
        mcub.transform.localRotation *= deltaRot;
    }
}