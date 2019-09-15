Shader "Unlit/Test1"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color",Color)=(1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			
			#pragma vertex vert
			#pragma fragment frag

            fixed4 _Color;

            struct v2f
            {
                float4 pos:SV_POSITION;
                fixed3 color:COLOR0;
            };

            //POSITION、SV_POSITION语义定义输入、输出的信息
			v2f vert(appdata_full i)
			{
			    v2f o;
			    o.pos = UnityObjectToClipPos(i.vertex);
			    //法线
			    o.color = i.normal * 0.5 + fixed3(0.5,0.5,0.5);
			    //切线
			    //o.color = i.tangent.xyz * 0.5 + fixed3(0.5,0.5,0.5)
			    return o;
			}
			
			fixed4 frag(v2f i):SV_TARGET
			{
			    fixed3 c = i.color;
			    //fixed4的前三个分量:.rgb,所有分量:.xyzw，每个分量:.x,.y,.z,.w
			    c *= _Color.rgb;
			    return fixed4(c,1);
			}

			ENDCG
		}
	}
}
