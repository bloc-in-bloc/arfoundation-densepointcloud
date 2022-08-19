using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Cdm.XR.Extensions
{
    public class ARDensePointCloudComputeBufferVisualizer : ARDensePointCloudVisualizer
    {
        public Material material;

        private ComputeBuffer _pointsbuffer;
        private ComputeBuffer _colorsbuffer;
        
        void OnRenderObject () {
            if (_pointsbuffer == null || _colorsbuffer == null) {
                return;
            }
            material.SetPass (0);
            material.SetBuffer ("_PointsBuffer", _pointsbuffer);
            material.SetBuffer ("_ColorsBuffer", _colorsbuffer);
            Graphics.DrawProceduralNow (MeshTopology.Points, _pointsbuffer.count, 1);
        }

        protected override void OnPointCloudUpdated(PointCloudUpdatedEventArgs e) {
            base.OnPointCloudUpdated(e);

            if (e.count == 0) {
                return;
            }
            if (_pointsbuffer != null) {
                _pointsbuffer.Release ();
            }
            _pointsbuffer = new ComputeBuffer (e.count, sizeof (float) * 3, ComputeBufferType.Default);
            if (_colorsbuffer != null) {
                _colorsbuffer.Release ();
            }
            _colorsbuffer = new ComputeBuffer (e.count, sizeof (float) * 4, ComputeBufferType.Default);
            
            var vertices = new NativeArray<float3>(e.count, Allocator.Temp);
            var colors = new NativeArray<float4>(e.count, Allocator.Temp);
            for (var i = 0; i < vertices.Length; i++) {
                vertices[i] = e.pointCloud.points[e.startIndex + i];
                Color32 color = e.pointCloud.colors[e.startIndex + i];
                colors[i] = new float4 (color.r / (float)byte.MaxValue,color.g / (float)byte.MaxValue,color.b / (float)byte.MaxValue, color.a / (float)byte.MaxValue);
            }

            _pointsbuffer.SetData(vertices);
            _colorsbuffer.SetData(colors);
        }
    }
}