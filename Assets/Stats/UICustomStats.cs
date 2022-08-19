using Cdm.XR.Extensions;
using TMPro;
using UnityEngine;

namespace BlocInBloc.Stats {
    public class UICustomStats : MonoBehaviour {
        [Header ("Infos")]
        public TMP_Text nbPointsValue;

        [Header ("Specs")]
        public TMP_Text resValue;
        public TMP_Text apiValue;
        public TMP_Text osValue;
        public TMP_Text cpuValue;
        public TMP_Text ramValue;
        public TMP_Text gpuValue;

        private ARDensePointCloudManager _arDensePointCloudManager;

        void Start () {
            InitSpec ();
            _arDensePointCloudManager = FindObjectOfType<ARDensePointCloudManager> ();
        }

        void Update () {
            if (_arDensePointCloudManager != null) {
                nbPointsValue.text = _arDensePointCloudManager.totalPoints.ToString ();
            }
        }

        void InitSpec () {
            Resolution res = Screen.currentResolution;
            resValue.text = $"{res.width}x{res.height}@{res.refreshRate}Hz";
            apiValue.text = SystemInfo.graphicsDeviceVersion;
            osValue.text = $"{SystemInfo.operatingSystem}";
            cpuValue.text = $"{SystemInfo.processorType} ({SystemInfo.processorCount} cores)";
            ramValue.text = $"{SystemInfo.systemMemorySize} Mo";
            gpuValue.text = SystemInfo.graphicsDeviceName;
        }
    }
}