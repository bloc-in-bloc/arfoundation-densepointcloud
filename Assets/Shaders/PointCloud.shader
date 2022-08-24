    Shader "Custom/Point Cloud"
    {
        Properties
        {
            _PointSize("Point Size", Float) = 0.05
        }
        SubShader
        {
            Pass
            {
                CGPROGRAM        
                #pragma multi_compile_fog
                #pragma vertex VSMain
                #pragma fragment PSMain
                #pragma target 5.0
     
                StructuredBuffer<float4> _PointsBuffer;
                StructuredBuffer<float4> _ColorsBuffer;
                float _PointSize;
                float4x4 _Transform;
                
                #include "UnityCG.cginc"
               
                struct shaderdata
                {
                    float4 vertex : SV_POSITION;
                    float4 color : TEXCOORD1;
                    float psize : PSIZE;
                    UNITY_FOG_COORDS(0)
                };
     
                shaderdata VSMain(uint id : SV_VertexID)
                {
                    shaderdata vs;
                    float4 pt = _PointsBuffer[id];
                    vs.vertex = UnityObjectToClipPos(pt);
                    vs.color = _ColorsBuffer[id];
                    vs.psize = _PointSize;
                    UNITY_TRANSFER_FOG(vs, vs.vertex);
                    return vs;
                }
     
                float4 PSMain(shaderdata ps) : SV_TARGET
                {
                    UNITY_APPLY_FOG(ps.fogCoord, ps.color);
                    return ps.color;
                }
               
                ENDCG
            }
        }
    }
