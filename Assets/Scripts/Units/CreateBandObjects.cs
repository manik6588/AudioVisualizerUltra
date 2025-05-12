using Unity.VisualScripting;
using UnityEngine;

[UnitTitle("CreateBands")]
[UnitCategory("Custom")]
public class CreateBandObjects : Unit
{
    private ControlInput inputTrigger;
    private ControlOutput outputTrigger;

    private ValueInput numOf;
    private ValueInput preFab;
    private ValueInput spacing;
    private ValueInput gap;
    private ValueInput parent;
    private ValueInput material;

    private ValueOutput gameObjects;
    private ValueOutput mirroredGameObjects;
    private ValueOutput lines;
    private ValueOutput mirroredLines;

    private Transform[] spawned;
    private Transform[] mSpawned;

    private Transform[] spawnedLines;
    private Transform[] mSpawnedLines;

    protected override void Definition()
    {
        inputTrigger = ControlInput("in", (flow) =>
        {
            int num = flow.GetValue<int>(numOf);
            GameObject fab = flow.GetValue<GameObject>(preFab);
            GameObject par = flow.GetValue<GameObject>(parent);
            float space = flow.GetValue<float>(spacing);
            float vSpace = flow.GetValue<float>(gap);
            Material mat = flow.GetValue<Material>(material);

            // Allocate arrays
            spawned = new Transform[num];
            mSpawned = new Transform[num];
            spawnedLines = new Transform[num];
            mSpawnedLines = new Transform[num];

            CreateBands(par, fab, space, vSpace, num, mat);

            return outputTrigger;
        });

        outputTrigger = ControlOutput("out");

        numOf = ValueInput<int>("pairs", 1);
        spacing = ValueInput<float>("spacing", 2.0f);
        gap = ValueInput<float>("gaps", 1.0f);
        parent = ValueInput<GameObject>("parent");
        preFab = ValueInput<GameObject>("orginal");
        material = ValueInput<Material>("material");

        gameObjects = ValueOutput<Transform[]>("objects", (flow) => spawned);
        mirroredGameObjects = ValueOutput<Transform[]>("mirrored", (flow) => mSpawned);
        lines = ValueOutput<Transform[]>("lines", (flow) => spawnedLines);
        mirroredLines = ValueOutput<Transform[]>("mirroredLines", (flow) => mSpawnedLines);

        Succession(inputTrigger, outputTrigger);
        Assignment(inputTrigger, gameObjects);
        Assignment(inputTrigger, mirroredGameObjects);
        Assignment(inputTrigger, lines);
        Assignment(inputTrigger, mirroredLines);
    }

    /// <summary>
    /// Creates a series of band objects and their mirrored counterparts, along with associated line renderers.
    /// </summary>
    /// <param name="par">The parent GameObject under which the bands and lines will be instantiated.</param>
    /// <param name="fab">The prefab GameObject to be instantiated for each band.</param>
    /// <param name="space">The horizontal spacing between each band object.</param>
    /// <param name="vSpace">The vertical offset for the band and its mirrored counterpart.</param>
    /// <param name="num">The number of band objects to create.</param>
    private void CreateBands(GameObject par, GameObject fab, float space, float vSpace, int num, Material mat)
    {
        if (par == null || fab == null)
        {
            Debug.LogError("Parent or Prefab is null.");
            return;
        }

        for (int i = 0; i < num; i++)
        {
            // Instantiate the main band object and its mirrored counterpart
            Transform cube = GameObject.Instantiate(fab, par.transform).transform;
            Transform mirrored = GameObject.Instantiate(fab, par.transform).transform;

            if (i < num - 1)
            {
                // Create a new GameObject for the line renderer and attach it to the parent
                GameObject lineObj = new GameObject("Line");
                LineRenderer line = lineObj.AddComponent<LineRenderer>();
                line.startWidth = 0.03f;
                line.endWidth = 0.03f;
                line.transform.SetParent(par.transform);
                line.material = mat;
                line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                // Create a new GameObject for the mirrored line renderer and attach it to the parent
                GameObject mirroredLineObj = new GameObject("MirroredLine");
                LineRenderer mirroredLine = mirroredLineObj.AddComponent<LineRenderer>();
                mirroredLine.startWidth = 0.03f;
                mirroredLine.endWidth = 0.03f;
                mirroredLine.transform.SetParent(par.transform);
                mirroredLine.material = mat;
                mirroredLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                // Store the line transforms in the respective arrays
                spawnedLines[i] = line.transform;
                mSpawnedLines[i] = mirroredLine.transform;
            }

            // Calculate the position for the band object and its mirrored counterpart
            float x = i * space;
            cube.localPosition = new Vector3(x, vSpace, 0f);
            mirrored.localPosition = new Vector3(x, -vSpace, 0f);

            // Store the band object transforms in the respective arrays
            spawned[i] = cube;
            mSpawned[i] = mirrored;
        }
    }
}
