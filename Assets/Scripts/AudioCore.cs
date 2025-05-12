using System.Runtime.InteropServices;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioCore : MonoBehaviour
{
    [SerializeField] public float[]    bandAmplitudes;
    [SerializeField] public float[]    flattenedArray;
    [HideInInspector] public int[]      footprintArray;
    [SerializeField] public float      peakAmplitude;

    public bool isAudioCoreReady = false;

    [DllImport("audiocore", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool ac2_initialize();

    [DllImport("audiocore", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool ac2_start();

    [DllImport("audiocore", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ac2_destroy();

    [DllImport("audiocore", CallingConvention = CallingConvention.Cdecl)]
    public static extern float ac2_get_peak_amplitude();

    [DllImport("audiocore", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr ac2_get_band_amplitudes();

    [DllImport("audiocore", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr ac2_get_flat_btw_amplitudes();

    [DllImport("audiocore", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr ac2_get_foot_btw_amplitudes();

    public void FetchAmplitudes()
    {
        int bandLength = 7;
        IntPtr dataPtr = IntPtr.Zero;

        try
        {
            dataPtr = ac2_get_band_amplitudes();
            if (dataPtr == IntPtr.Zero)
            {
                Debug.LogWarning("ac2_get_band_amplitudes returned null.");
                return;
            }

            bandAmplitudes = new float[bandLength];
            Marshal.Copy(dataPtr, bandAmplitudes, 0, bandLength);
        }
        finally
        {
        }
    }

    public void FetchRawAmplitudes()
    {
        IntPtr flatPtr = IntPtr.Zero;
        IntPtr footPtr = IntPtr.Zero;
        int flatLength = 700;
        int rowLength = 7;

        try
        {
            // Call the native function
            flatPtr = ac2_get_flat_btw_amplitudes();
            footPtr = ac2_get_foot_btw_amplitudes();

            // Validate the results
            if (flatPtr == IntPtr.Zero || footPtr == IntPtr.Zero)
            {
                Debug.LogWarning("ac2_get_btw_amplitudes returned invalid data.");
                return;
            }

            // Allocate managed arrays with proper size
            flattenedArray = new float[flatLength];
            footprintArray = new int[rowLength];

            // Copy data from native memory to managed arrays
            Marshal.Copy(flatPtr, flattenedArray, 0, 700);
            Marshal.Copy(footPtr, footprintArray, 0, 7);
        }
        finally
        {
            
        }
    }

    void Start()
    {
        isAudioCoreReady = ac2_initialize() && ac2_start();
        if (!isAudioCoreReady)
        {
            Debug.LogError("AudioCore initialization failed.");
            ac2_destroy();
        }
    }

    void Update()
    {
        if (!isAudioCoreReady)
            return;

        try
        {
            // 1. Fetch data from native lib
            FetchAmplitudes();
            FetchRawAmplitudes();
            peakAmplitude = ac2_get_peak_amplitude();

            // 3. Safely tell native lib you're done
            //ac2_analysis_fetched();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Frame processing error: {ex.Message}");
        }
    }

    void OnDestroy()
    {
        if (isAudioCoreReady)
        {
            ac2_destroy();
        }
    }
}