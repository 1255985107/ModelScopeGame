Shader "Custom/VideoChromaKey"
{
    Properties
    {
        _MainTex ("Video Texture", 2D) = "white" {}
        _KeyColor ("Key Color (抠除的颜色)", Color) = (1, 0, 0, 1) // 默认红色
        _Threshold ("Threshold (阈值)", Range(0, 1)) = 0.4
        _Smoothness ("Smoothness (边缘柔化)", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        // 开启透明混合
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _KeyColor;
            float _Threshold;
            float _Smoothness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. 获取图片原本的颜色（包含原本的 Alpha）
                fixed4 col = tex2D(_MainTex, i.uv);

                // --- 2. 抠像逻辑 (针对视频) ---
                // 计算当前颜色与 KeyColor(红色) 的差距
                float diff = distance(col.rgb, _KeyColor.rgb);
                // 算出抠像后的透明度 (红色部分为0，其他为1)
                float chromaAlpha = smoothstep(_Threshold, _Threshold + _Smoothness, diff);
                
                // --- 3. 核心修改：融合逻辑 ---
                // 最终透明度 = 原图自带Alpha * 抠像Alpha
                // 这样：
                // A. 如果是封面图透明区域：原图Alpha是0 -> 结果就是0 (透明)
                // B. 如果是视频红色背景：抠像Alpha是0 -> 结果就是0 (透明)
                // C. 如果是正常显示部分：两个都是1 -> 结果是1 (不透明)
                col.a = col.a * chromaAlpha;

                return col;
            }
            ENDCG
        }
    }
}