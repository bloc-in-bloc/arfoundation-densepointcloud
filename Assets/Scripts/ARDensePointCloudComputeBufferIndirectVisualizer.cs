using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Cdm.XR.Extensions
{
    public class ARDensePointCloudComputeBufferIndirectVisualizer : ARDensePointCloudVisualizer
    {
        public Material material;

        private ComputeBuffer _pointsbuffer;
        private ComputeBuffer _colorsbuffer;

        private int nbPoints = 0;
        private ARDensePointCloud _pointCloud;
        private ComputeBuffer mArgBuffer;
        private Bounds _bounds;
        
        protected override void Awake()
        {
            base.Awake ();
            _pointCloud = GetComponent<ARDensePointCloud> ();
            int count = 10000000;
            _pointsbuffer = new ComputeBuffer (count, sizeof (float) * 4, ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
            _colorsbuffer = new ComputeBuffer (count, sizeof (float) * 4, ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
            material.SetBuffer ("_PointsBuffer", _pointsbuffer);
            material.SetBuffer ("_ColorsBuffer", _colorsbuffer);
            
            //Args for indirect draw
            int[] args = new int[]
            {
                (int)count, //vertex count per instance
                (int)1, //instance count
                (int)0, //start vertex location
                (int)0 //start instance location
            };
            mArgBuffer = new ComputeBuffer(args.Length, sizeof(int), ComputeBufferType.IndirectArguments);
            mArgBuffer.SetData(args);
            
            _bounds = new Bounds (new Vector3 (0, 0, 0), new Vector3 (10, 10, 10));
        }
        
        void Update () {
            Graphics.DrawProceduralIndirect(material, _bounds, MeshTopology.Points, mArgBuffer);
        }
        
        protected override void OnPointCloudUpdated(PointCloudUpdatedEventArgs e) {
            base.OnPointCloudUpdated(e);

            if (e.count == 0 || _pointCloud.isFull) {
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