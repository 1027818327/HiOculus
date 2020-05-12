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
                //��ģ�Ͷ������V����
                float4 vertex:POSITION;
                //��ģ�ͷ������V����
                float3 normal:NORMAL;
                //��ģ�͵ĵ�һ��uv���texcoord
                float4 texcoord:TEXCOORD0;

            };
            struct v2f //v vert to frag
            {
                //SV_POSITION ����Unity :posΪ�ü��ռ��е�λ����Ϣ
                float4 pos:SV_POSITION;
                //COLOR0 ������Դ洢��ɫ��Ϣ
                fixed3 color : COLOR0;
            };

            fixed4 _Color;
            // POSITION SV_POSITION  ����
            v2f vert(a2v v)
            {
                 v2f o;
                 o.pos = UnityObjectToClipPos(v.vertex);
                 //�� ��-1��1�� ת��Ϊ��0��1�� 
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
