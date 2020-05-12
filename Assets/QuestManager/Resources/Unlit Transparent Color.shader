Shader "OculusCustom/Unlit Transparent Color" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
    }

        SubShader{
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            LOD 100
            Fog {Mode Off}

            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
        //Color [_Color]

        Pass {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
               
            struct a2v  //a application to vert
            {
                //用模型顶点填充V变量
                float4 vertex:POSITION;
                //用模型法线填充V变量
                float3 normal:NORMAL;
                //用模型的第一套uv填充texcoord
                float4 texcoord:TEXCOORD0;

            };
            struct v2f //v vert to frag
            {
                //SV_POSITION 告诉Unity :pos为裁剪空间中的位置信息
                float4 pos:SV_POSITION;
                //COLOR0 语义可以存储颜色信息
                fixed3 color : COLOR0;
            };

            fixed4 _Color;
            // POSITION SV_POSITION  语义
            v2f vert(a2v v)
            {
                 v2f o;
                 o.pos = UnityObjectToClipPos(v.vertex);
                 //将 【-1，1】 转变为【0，1】 
                 o.color = lerp(0,1,o.pos) * _Color;
                 return  o;
            }
            fixed4 frag(v2f i) :SV_TARGET
            {
                 return fixed4(i.color,1);
            }
            ENDCG
    }
}
}
