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
            _pointsbuffer = new ComputeBuffer (count, sizeof (float) * 3, ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
            _colorsbuffer = new ComputeBuffer (count, sizeof (uint), ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
            
            if (material == null) {
                material = new Material (shader);
                material.SetFloat ("_PointSize", 10);
            }
            
            material.SetBuffer ("_PointsBuffer", _pointsbuffer);
            material.SetBuffer ("_ColorsBuffer", _colorsbuffer);
        }
        
        static uint EncodeColor(Color c)
        {
            const float kMaxBrightness = 16;

            var y = Mathf.Max(Mathf.Max(c.r, c.g), c.b);
            y = Mathf.Clamp(Mathf.Ceil(y * 255 / kMaxBrightness), 1, 255);

            var rgb = new Vector3(c.r, c.g, c.b);
            rgb *= 255 * 255 / (y * kMaxBrightness);

            return ((uint)rgb.x      ) |
                   ((uint)rgb.y <<  8) |
                   ((uint)rgb.z << 16) |
                   ((uint)y     << 24);
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

            var vertices = new NativeArray<float3>(e.count, Allocator.Temp);
            var colors = new NativeArray<uint>(e.count, Allocator.Temp);
            for (var i = 0; i < vertices.Length; i++) {
                Vector3 pos = e.pointCloud.points[e.startIndex + i];
                vertices[i] = new float3 (pos.x, pos.y, pos.z);
                Color32 color = e.pointCloud.colors[e.startIndex + i];
                colors[i] = EncodeColor (color);
            }
            
            nbPoints += e.count;

            NativeArray<float3> tmpPoints = _pointsbuffer.BeginWrite<float3> (e.startIndex, e.count);
            tmpPoints.CopyFrom (vertices);
            _pointsbuffer.EndWrite<float3> (e.count);
            
            NativeArray<uint> tmpColors = _colorsbuffer.BeginWrite<uint> (e.startIndex, e.count);
            tmpColors.CopyFrom (colors);
            _colorsbuffer.EndWrite<uint> (e.count);

            vertices.Dispose ();
            colors.Dispose ();
        }
    }
}