Shader "Custom/SumofSines"
{
    Properties
    {
        //Amplitude of the waves.
        Amplitude ("Amplitude", Range(0,1)) = 0.5
        //Speed of the waves motion.
        Speed ("Speed", Range(0, 10)) = 2
        //Length of each wave.
        Wavelength ("Wavelength", Range(0, 2)) = 1
        //The directions that waves are being sent.
        Direction ("Direction", Vector) = (0, 0, 0, 0)
        //The Color of the wave.
        WaveColor ("Wave Color", Color) = (0, 0, 1, 1)
        //The clarity of the water, this is later added to a temp color to interpolate with the WaveColor.
        WaveAlpha ("Wave Alpha", Range(0, 1)) = 0.5
    }
    SubShader
    {
        //Set up the shader for transparency so we can see the mannequins below the water.
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        //Don't draw the bottom of the water, effeciency.
        Cull back
        //Level of detail.
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                //Model data.
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                //Vertex data in floats.
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float sumofsines : TEXCOORD2;
            };

            float Amplitude;
            float Speed;
            float Wavelength;
            float2 Direction;
            float4 WaveColor;
            float WaveAlpha;

            v2f vert(appdata v)
            {
                v2f o;
                //Frequency of waves determined.
                float frequency = (2 * 3.14) / Wavelength;
                float timeFactor = _Time.y * (Speed * frequency);
                float2 direction = dot(Direction, float2(v.vertex.x, v.vertex.z));
                
                //Sum of signs to be added together for the wave effect.
                float displacement1 = Amplitude * sin(timeFactor + frequency * .3 * direction * frac(sin(dot(1,float2(12.9898,78.233)))*43758.5453123));
                float displacement2 = Amplitude * sin(timeFactor + frequency * .5 * direction * frac(sin(dot(1,float2(12.9898,78.233)))*43758.5453123));
                float displacement3 = Amplitude * sin(timeFactor + frequency * 1 * direction * frac(sin(dot(1,float2(12.9898,78.233)))*43758.5453123));

                float sumofsines = displacement1 + displacement2 + displacement3;

                o.vertex = UnityObjectToClipPos(v.vertex + float4(0, sumofsines, 0, 0));
                o.uv = v.uv;
                o.normal = mul((float3x3)unity_WorldToObject, v.normal);
                o.sumofsines = sumofsines;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float edge = smoothstep(0.0, 1.0, length(i.normal.xy));
                edge = 1.0 - edge;

                float4 tempAlpha = (1, 1, 1, WaveAlpha);

                //Calculate the final color.
                float4 finalColor = lerp(fixed4(WaveColor.rgb, WaveAlpha), tempAlpha, edge);
                return finalColor;
            }
            ENDCG
        }
    }
}
