    using System.Globalization;
using TMPro;
using Unity.Profiling;
using UnityEngine;

namespace BlocInBloc {
    public class RenderingMonitor : MonoBehaviour {
        public TMP_Text setPassCallsCount;
        public TMP_Text drawCallsCount;
        public TMP_Text trianglesCount;
        public TMP_Text verticesCount;

        private ProfilerRecorder _setPassCallsCountRecorder;
        private ProfilerRecorder _drawCallsCountRecorder;
        private ProfilerRecorder _trianglesCountRecorder;
        private ProfilerRecorder _verticesCountRecorder;
        
        private NumberFormatInfo _nfi;

        private void Awake () {
            _nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone ();
            _nfi.NumberGroupSeparator = " ";
        }

        private void OnEnable () {
            _setPassCallsCountRecorder = ProfilerRecorder.StartNew (ProfilerCategory.Render, "SetPass Calls Count");
            _drawCallsCountRecorder = ProfilerRecorder.StartNew (ProfilerCategory.Render, "Draw Calls Count");
            _trianglesCountRecorder = ProfilerRecorder.StartNew (ProfilerCategory.Render, "Triangles Count");
            _verticesCountRecorder = ProfilerRecorder.StartNew (ProfilerCategory.Render, "Vertices Count");
        }

        private void OnDisable () {
            _setPassCallsCountRecorder.Dispose ();
            _drawCallsCountRecorder.Dispose ();
            _trianglesCountRecorder.Dispose ();
            _verticesCountRecorder.Dispose ();
        }

        private void Update () {
            if (_setPassCallsCountRecorder.Valid) {
                setPassCallsCount.text = _setPassCallsCountRecorder.LastValue.ToString ("#,0", _nfi);
            }

            if (_drawCallsCountRecorder.Valid) {
                drawCallsCount.text = _drawCallsCountRecorder.LastValue.ToString ("#,0", _nfi);
            }
            
            if (_trianglesCountRecorder.Valid) {
                trianglesCount.text = _trianglesCountRecorder.LastValue.ToString ("#,0", _nfi);
            }
            
            if (_verticesCountRecorder.Valid) {
                verticesCount.text = _verticesCountRecorder.LastValue.ToString ("#,0", _nfi);
            }
        }
    }
}