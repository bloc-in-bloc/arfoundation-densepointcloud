using System;
using System.Globalization;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;

namespace BlocInBloc {
    public class MemoryMonitor : MonoBehaviour {
        [Serializable]
        public class OnDebugBuildEvent : UnityEvent { }

        public TMP_Text totalUsedMemory;
        public TMP_Text totalReservedMemory;
        public TMP_Text meshCount;
        public TMP_Text meshMemory;
        public OnDebugBuildEvent onDebugBuild;

        private ProfilerRecorder _totalUsedMemoryRecorder;
        private ProfilerRecorder _totalReservedMemoryRecorder;
        private ProfilerRecorder _meshCountRecorder;
        private ProfilerRecorder _meshMemoryRecorder;

        private bool isDebugBuild => Debug.isDebugBuild || Application.isEditor;
        private NumberFormatInfo _nfi;

        private void Awake () {
            _nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone ();
            _nfi.NumberGroupSeparator = " ";
            if (isDebugBuild) {
                onDebugBuild.Invoke ();
            }
        }

        private void OnEnable () {
            _totalUsedMemoryRecorder = ProfilerRecorder.StartNew (ProfilerCategory.Memory, "Total Used Memory");
            _totalReservedMemoryRecorder = ProfilerRecorder.StartNew (ProfilerCategory.Memory, "Total Reserved Memory");
            if (isDebugBuild) {
                _meshMemoryRecorder = ProfilerRecorder.StartNew (ProfilerCategory.Memory, "Mesh Memory");
                _meshCountRecorder = ProfilerRecorder.StartNew (ProfilerCategory.Memory, "Mesh Count");
            }
        }

        private void OnDisable () {
            _totalUsedMemoryRecorder.Dispose ();
            _totalReservedMemoryRecorder.Dispose ();
            _meshCountRecorder.Dispose ();
            _meshMemoryRecorder.Dispose ();
        }

        private void Update () {
            if (_totalUsedMemoryRecorder.Valid) {
                totalUsedMemory.text = $"{ToMegaByte (_totalUsedMemoryRecorder.LastValue).ToString ("#,0", _nfi)} mb";
            }

            if (_totalReservedMemoryRecorder.Valid) {
                totalReservedMemory.text = $"{ToMegaByte (_totalReservedMemoryRecorder.LastValue).ToString ("#,0", _nfi)} mb";
            }

            if (isDebugBuild) {
                if (_meshMemoryRecorder.Valid) {
                    meshMemory.text = $"{ToMegaByte (_meshMemoryRecorder.LastValue).ToString ("#,0", _nfi)} mb";
                }

                if (_meshCountRecorder.Valid) {
                    meshCount.text = _meshCountRecorder.LastValue.ToString ("#,0", _nfi);
                }
            }
        }

        private float ToMegaByte (long byteValue) {
            return byteValue / 1048576f; // byte => mega byte : 1024 * 1024
        }
    }
}