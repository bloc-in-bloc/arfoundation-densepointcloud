using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Cdm.XR.Extensions
{
    public class ARDensePointCloudComputeBufferVisualizer : ARDensePointCloudVisualizer {
        
        public Shader shader;
        private Material material;

        private ComputeBuffer _pointsbuffer;
        private ComputeBuffer _colorsbuffer;

        private int nbPoints = 0;
        
        protected override void Awake()
        {
            base.Awake ();
            int count = 110000;
            _pointsbuffer = new ComputeBuffer (count, sizeof (float) * 4, ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
            _colorsbuffer = new ComputeBuffer (count, sizeof (float) * 4, ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
            
            if (material == null) {
                material = new Material (shader);
                material.SetFloat ("_PointSize", 10);
            }
            
            material.SetBuffer ("_PointsBuffer", _pointsbuffer);
            material.SetBuffer ("_ColorsBuffer", _colorsbuffer);
        }
        
        void OnRenderObject () {
            if (_pointsbuffer == null || _colorsbuffer == null) {
                return;
            }
            material.SetPass (0);
            Graphics.DrawProceduralNow (MeshTopology.Points, nbPoints);
        }

        protected override void OnPointCloudUpdated(PointCloudUpdatedEventArgs e) {
            base.OnPointCloudUpdated(e);

            if (e.count == 0) {
                return;
            }

            var vertices = new NativeArray<float4>(e.count, Allocator.Temp);
            var colors = new NativeArray<float4>(e.count, Allocator.Temp);
            for (var i = 0; i < vertices.Length; i++) {
                Vector3 pos = e.pointCloud.points[e.startIndex + i];
                vertices[i] = new float4 (pos.x, pos.y, pos.z, 1);
                Color32 color = e.pointCloud.colors[e.startIndex + i];
                colors[i] = new float4 (color.r / (float)byte.MaxValue,color.g / (float)byte.MaxValue,color.b / (float)byte.MaxValue, color.a / (float)byte.MaxValue);
            }
            
            nbPoints += e.count;

            NativeArray<float4> tmpPoints = _pointsbuffer.BeginWrite<float4> (e.startIndex, e.count);
            tmpPoints.CopyFrom (vertices);
            _pointsbuffer.EndWrite<float4> (e.count);
            
            NativeArray<float4> tmpColors = _colorsbuffer.BeginWrite<float4> (e.startIndex, e.count);
            tmpColors.CopyFrom (colors);
            _colorsbuffer.EndWrite<float4> (e.count);

            vertices.Dispose ();
            colors.Dispose ();
        }
    }
}