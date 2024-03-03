using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "ScriptableObjects/CameraSettings", order = 1)]
public class CameraSettingsSO : ScriptableObject {
    public bool LerpFov = true;
    public float FovMin = 82f;
    public float FovMax = 72f;

    public bool LerpShoulderOffset = true;
    public Vector3 ShoulderOffsetMin = new Vector3(0, 0.12f, 0.44f);
    public Vector3 ShoulderOffsetMax = new Vector3(0, 0.12f, 0.12f);

    public bool LerpDamping = true;
    public Vector3 DampingMin = Vector3.zero;
    public Vector3 DampingMax = new Vector3(0.05f, 0.05f, 0.05f);

    public bool LerpLookaheadTime = true;
    public float LookaheadTimeMin = 0f;
    public float LookaheadTimeMax = 0.5f;

    public bool LerpLookaheadSmoothing = true;
    public float LookaheadSmoothingMin = 0f;
    public float LookaheadSmoothingMax = 50;

    public bool LerpAimScreen = true;
    public float AimScreenYMin = 0;
    public float AimScreenYMax = 0.5f;

    public bool LerpAimSoftZoneSize = true;
    public float AimSoftZoneSizeMin = 0.0f;
    public float AimSoftZoneSizeMax = 0.8f;
}
