using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace BlocInBloc.Stats {
    public class FpsMonitor : MonoBehaviour {
        [SerializeField]
        private int _averageSamples = 200;
        [Tooltip ("Time (in seconds) to reset the minimum and maximum framerates if they don't change in the specified time. Set to 0 if you don't want it to reset.")]
        [SerializeField]
        private int _timeToResetMinMaxFps = 10;

        public float CurrentFPS { get { return _currentFps; } }
        public float AverageFPS { get { return _avgFps; } }
        public float MinFPS { get { return _minFps; } }
        public float MaxFPS { get { return _maxFps; } }

        private float _currentFps = 0f;
        private float _avgFps = 0f;
        private float _minFps = 0f;
        private float _maxFps = 0f;
        private float[] _averageFpsSamples;
        private int _avgFpsSamplesOffset = 0;
        private int _indexMask = 0;
        private int _avgFpsSamplesCapacity = 0;
        private int _avgFpsSamplesCount = 0;
        private float _timeToResetMinFpsPassed = 0f;
        private float _timeToResetMaxFpsPassed = 0f;
        private float _unscaledDeltaTime = 0f;

        void Awake () {
            Init ();
        }

        void Update () {
            _unscaledDeltaTime = Time.unscaledDeltaTime;

            _timeToResetMinFpsPassed += _unscaledDeltaTime;
            _timeToResetMaxFpsPassed += _unscaledDeltaTime;

            // Update fps and ms
            _currentFps = 1 / _unscaledDeltaTime;

            // Update avg fps
            _avgFps = 0;

            _averageFpsSamples[ToBufferIndex (_avgFpsSamplesCount)] = _currentFps;
            _avgFpsSamplesOffset = ToBufferIndex (_avgFpsSamplesOffset + 1);

            if (_avgFpsSamplesCount < _avgFpsSamplesCapacity) {
                _avgFpsSamplesCount++;
            }

            for (int i = 0; i < _avgFpsSamplesCount; i++) {
                _avgFps += _averageFpsSamples[i];
            }

            _avgFps /= _avgFpsSamplesCount;

            // Checks to reset min and max fps
            if (_timeToResetMinMaxFps > 0 &&
                _timeToResetMinFpsPassed > _timeToResetMinMaxFps) {
                _minFps = 0;
                _timeToResetMinFpsPassed = 0;
            }

            if (_timeToResetMinMaxFps > 0 &&
                _timeToResetMaxFpsPassed > _timeToResetMinMaxFps) {
                _maxFps = 0;
                _timeToResetMaxFpsPassed = 0;
            }

            // Update min fps
            if (_currentFps < _minFps || _minFps <= 0) {
                _minFps = _currentFps;
                _timeToResetMinFpsPassed = 0;
            }

            // Update max fps
            if (_currentFps > _maxFps || _maxFps <= 0) {
                _maxFps = _currentFps;
                _timeToResetMaxFpsPassed = 0;
            }
        }

        [ContextMenu ("ResetAverage")]
        public void ResetAverage () {
            _avgFpsSamplesCount = 0;
            _avgFpsSamplesOffset = 0;
            _avgFps = 0f;
        }

        void Init () {
            ResizeSamplesBuffer (_averageSamples);
        }

        void ResizeSamplesBuffer (int size) {
            _avgFpsSamplesCapacity = Mathf.NextPowerOfTwo (size);

            _averageFpsSamples = new float[_avgFpsSamplesCapacity];

            _indexMask = _avgFpsSamplesCapacity - 1;
            _avgFpsSamplesOffset = 0;
        }

#if NET_4_6 || NET_STANDARD_2_0
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
#endif
        int ToBufferIndex (int index) {
            return (index + _avgFpsSamplesOffset) & _indexMask;
        }
    }
}