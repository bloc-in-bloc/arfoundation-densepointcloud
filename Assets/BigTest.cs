using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class BigTest : MonoBehaviour {
    public Material material;

    private ComputeBuffer _pointsbuffer;
    private ComputeBuffer _colorsbuffer;
    private ComputeBuffer mArgBuffer;

    private int size = 5;
    private int count = 1000000;
    private Bounds _bounds;
    private int nbPoints = 0;
    
    void Awake () {
        _pointsbuffer = new ComputeBuffer (count, sizeof (float) * 4, ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
        _colorsbuffer = new ComputeBuffer (count, sizeof (float) * 4, ComputeBufferType.Default, ComputeBufferMode.SubUpdates);
        material.SetBuffer ("_PointsBuffer", _pointsbuffer);
        material.SetBuffer ("_ColorsBuffer", _colorsbuffer);
        
        _bounds = new Bounds (new Vector3 (0, 0, 0), new Vector3 (10, 10, 10));
        
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
        
        InvokeRepeating ("AddPoints", 0f, 1f);
    }

    void AddPoints () {
        int count = 10000;
        
        var vertices = new NativeArray<float4>(count, Allocator.Temp);
        var colors = new NativeArray<float4>(count, Allocator.Temp);
        for (var i = 0; i < vertices.Length; i++) {
            vertices[i] = new float4 (Random.value * size, Random.value * size, Random.value * size, 1);
            Color32 color = Color.green;
            colors[i] = new float4 (color.r / (float)byte.MaxValue,color.g / (float)byte.MaxValue,color.b / (float)byte.MaxValue, color.a / (float)byte.MaxValue);
        }

        NativeArray<float4> tmpPoints = _pointsbuffer.BeginWrite<float4> (nbPoints, count);
        tmpPoints.CopyFrom (vertices);
        _pointsbuffer.EndWrite<float4> (count);
            
        NativeArray<float4> tmpColors = _colorsbuffer.BeginWrite<float4> (nbPoints, count);
        tmpColors.CopyFrom (colors);
        _colorsbuffer.EndWrite<float4> (count);
        
        nbPoints += count;
        
        Debug.Log (nbPoints);
        
        vertices.Dispose ();
        colors.Dispose ();
    }
    
    /*
    void OnRenderObject () {
        material.SetPass (0);
        Graphics.DrawProceduralNow (MeshTopology.Points,nbPoints,1);
    }
    */

    /*
    void Update () {
        material.SetPass (0);
        Graphics.DrawProcedural (material, new Bounds (new Vector3 (0,0,0),new Vector3 (10,10,10)),MeshTopology.Points, count, 1, null, null, ShadowCastingMode.Off);
    }
    */

    void Update () {
        Graphics.DrawProceduralIndirect(material, _bounds, MeshTopology.Points, mArgBuffer);
    }
}
