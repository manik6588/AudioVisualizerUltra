using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("Custom")]
[UnitTitle("Draw Bezier Line")]
public class BezierLineRendererUnit : Unit
{
    // Control ports
    [DoNotSerialize] public ControlInput inputTrigger;
    [DoNotSerialize] public ControlOutput outputTrigger;

    // Value inputs
    [DoNotSerialize] public ValueInput lineRenderer;
    [DoNotSerialize] public ValueInput controlPoints;
    [DoNotSerialize] public ValueInput spectrumPoints;
    [DoNotSerialize] public ValueInput index;
    [DoNotSerialize] public ValueInput smoothSegments;
    [DoNotSerialize] public ValueInput pointOffset;

    protected override void Definition()
    {
        inputTrigger = ControlInput("in", (flow) =>
        {
            LineRenderer    lrs         = flow.GetValue<LineRenderer>(lineRenderer);
            GameObject[]    cps         = flow.GetValue<GameObject[]>(controlPoints);
            List<float>     sps         = flow.GetValue<List<float>>(spectrumPoints);
            int             indx        = flow.GetValue<int>(index);
            int             segments    = flow.GetValue<int>(smoothSegments);
            float           offset      = flow.GetValue<float>(pointOffset);

            DrawBezierLines(lrs, cps, sps, indx, segments, offset);
            return outputTrigger;
        });

        outputTrigger = ControlOutput("out");

        lineRenderer = ValueInput<LineRenderer[]>("lineRenderer");
        controlPoints = ValueInput<GameObject[]>("controlPoints");
        spectrumPoints = ValueInput<List<float>>("spectrumPoints");
        index = ValueInput<int>("index");
        smoothSegments = ValueInput<int>("smoothSegments", 10);
        pointOffset = ValueInput<float>("pointOffset", 0.2f);

        Succession(inputTrigger, outputTrigger);
    }

    private void DrawBezierLines(LineRenderer lineRenderer, GameObject[] controlPoints, List<float> spectrumPoints, int index, int smoothSegments, float pointOffset)
    {
        if (controlPoints == null || controlPoints.Length < 2 || lineRenderer == null)
            return;

        if (spectrumPoints == null || spectrumPoints.Count < 2)
            return;

        if (index < 0 || index >= controlPoints.Length - 1)
            return;

        // Use world positions to ensure correct rendering
        Vector3 worldStart = controlPoints[index].transform.position;
        Vector3 worldEnd = controlPoints[index + 1].transform.position;

        Vector3 direction = (worldEnd - worldStart).normalized;
        float totalWidth = Vector3.Distance(worldStart, worldEnd);
        float xSpacing = totalWidth / (spectrumPoints.Count - 1);

        List<Vector3> bezierPoints = new List<Vector3>();

        for (int i = 0; i < spectrumPoints.Count - 1; i++)
        {
            Vector3 p0 = worldStart + direction * (i * xSpacing);
            Vector3 p3 = worldStart + direction * ((i + 1) * xSpacing);

            p0.y += spectrumPoints[i];
            p3.y += spectrumPoints[i + 1];

            Vector3 tangent = Vector3.right * pointOffset;

            Vector3 p1 = p0 + tangent;
            Vector3 p2 = p3 - tangent;

            for (int j = 0; j <= smoothSegments; j++)
            {
                float t = j / (float)smoothSegments;
                Vector3 point = CalculateCubicBezierPoint(t, p0, p1, p2, p3);
                bezierPoints.Add(point);
            }
        }

        lineRenderer.positionCount = bezierPoints.Count;
        lineRenderer.SetPositions(bezierPoints.ToArray());
    }

    private Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        return uuu * p0 +
               3 * uu * t * p1 +
               3 * u * tt * p2 +
               ttt * p3;
    }
}