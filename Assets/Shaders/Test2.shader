Shader "Unlit/Test2"
{
    Properties
    {
        _Diffuse("Diffuse",Color)=(1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            fixed4 _Diffuse;

            struct v2f
            {
                fixed3 color:COLOR;
                float4 vertex:SV_POSITION;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);//法线方向
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);//环境光方向
                fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal,worldLight));//漫反射光
                o.color = diffuse;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(i.color,1);
            }
            ENDCG
        }
    }
}
