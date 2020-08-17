#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 已弃用
/// </summary>
namespace Assets.ExporterGLTF20
{
    class GLTF_ParticleSystem : GlTF_Writer
    {
        public ParticleSystem particleSys = null;
        public ParticleSystemRenderer renderer = null;
        public float textureIndex = -1;

        public void Populate(Transform tr, float index)
        {
            textureIndex = index;
            particleSys = tr.GetComponent<ParticleSystem>();
            renderer = tr.GetComponent<ParticleSystemRenderer>();
        }


        public GLTF_ParticleSystem() { }

        public override void Write()
        {

            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("{\n");

            IndentIn();
            WriteTexture();
            WriteMain();
            WriteEmission();
            WriteShape();
            WriteVelocityOverLifetime();
            WriteLimitVelocityOverLifetime();
            WriteForceOverLifetime();
            WriteColorOverLifetime();
            WriteSizeOverLifetime();
            WriteRotationOverLifetime();

            WriteRender();

            IndentOut();
            Indent(); jsonWriter.Write("}");
            IndentOut();
        }

        /// 面板导出
        private void WriteTexture()
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"textureIndex\":{\n");
            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("\"index\":" + textureIndex);
            IndentOut();
            Indent(); jsonWriter.Write("}");
        }
        private void WriteMain()
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"main\":{\n");
            IndentIn();

            CommaNL();
            Indent(); jsonWriter.Write("\"targetStopDuration\":" + particleSys.main.duration);

            CommaNL();
            Indent(); jsonWriter.Write("\"beginAnimationLoop\":" + (particleSys.main.loop ? "true" : "false") + "");

            CommaNL();
            Indent(); jsonWriter.Write("\"manualEmitCount\":" + particleSys.main.maxParticles);

            CommaNL();
            Indent(); jsonWriter.Write("\"playOnAwake\":" + (particleSys.main.playOnAwake == true ? "true" : "false"));

            CommaNL();
            Indent(); jsonWriter.Write("\"randomizeRotationDirection\":" + particleSys.main.randomizeRotationDirection);

            /// 默认local
            //CommaNL();
            //Indent(); jsonWriter.Write("\"simulationSpeed\":" + data.main.simulationSpeed);

            if (particleSys.main.prewarm)
            {
                CommaNL();
                Indent(); jsonWriter.Write("\"prewarm\":true");
            }
            else
            {
                WriteMinMaxCurve(particleSys.main.startDelay, "startDelay");
            }

            WriteMinMaxCurve(particleSys.main.duration, "duration");
            WriteMinMaxGradient(particleSys.main.startColor, "startColor");
            WriteMinMaxCurve(particleSys.main.startSpeed, "startSpeed");
            WriteMinMaxCurve(particleSys.main.startLifetime, "startLifetime");

            if (particleSys.main.startSize3D)
            {
                WriteMinMaxCurve(particleSys.main.startSizeX, "startSizeX");
                WriteMinMaxCurve(particleSys.main.startSizeY, "startSizeY");
                //WriteMinMaxCurve(particleSys.main.startSizeZ, "startSizeZ");
            }
            else
            {
                WriteMinMaxCurve(particleSys.main.startSize, "startSize");
            }

            WriteMinMaxCurve(particleSys.main.startRotation, "startRotation");

            if (particleSys.main.startRotation3D)
            {
                WriteMinMaxCurve(particleSys.main.startRotationX, "startRotationX");
                WriteMinMaxCurve(particleSys.main.startRotationY, "startRotationY");
                WriteMinMaxCurve(particleSys.main.startRotationZ, "startRotationZ");
            }

            WriteMinMaxCurve(particleSys.main.gravityModifier, "gravityModifier");


            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        private void WriteEmission()
        {
            var dataE = particleSys.emission;
            if (!dataE.enabled) return;
            CommaNL();
            Indent(); jsonWriter.Write("\"emission\":{\n");
            IndentIn();
            var bursts = new UnityEngine.ParticleSystem.Burst[dataE.burstCount];
            dataE.GetBursts(bursts);
            CommaNL();
            Indent(); jsonWriter.Write("\"rateOverDistanceMultiplier\":" + dataE.rateOverDistanceMultiplier);
            CommaNL();
            Indent(); jsonWriter.Write("\"rateOverTimeMultiplier\":" + dataE.rateOverTimeMultiplier);

            WriteMinMaxCurve(dataE.rateOverDistance, "rateOverDistance");
            WriteMinMaxCurve(dataE.rateOverTime, "rateOverTime");

            CommaNL();
            Indent(); jsonWriter.Write("\"bursts\":[\n");
            IndentIn();

            if (bursts != null)
            {
                foreach (var v in bursts)
                    WriteBurst(v);
            }
            IndentOut();
            Indent(); jsonWriter.Write("]\n");
            /// bursts: [{time: 0.1, min: 3, max: 3}];
            //scope.WriteKeyValue("burstCount", data.burstCount);

            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        private void WriteShape()
        {
            var dataS = particleSys.shape;
            if (!dataS.enabled) return;
            CommaNL();
            Indent(); jsonWriter.Write("\"shape\":{\n");
            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("\"alignToDirection\":" + (dataS.alignToDirection == true ? "true" : "false"));
            CommaNL();
            Indent(); jsonWriter.Write("\"angle\":" + dataS.angle / 90 * Mathf.PI);
            CommaNL();
            Indent(); jsonWriter.Write("\"arc\":" + dataS.arc);
            CommaNL();
            Indent(); jsonWriter.Write("\"length\":" + dataS.length);
            CommaNL();
            Indent(); jsonWriter.Write("\"meshScale\":" + dataS.meshScale);
            CommaNL();
            Indent(); jsonWriter.Write("\"meshShapeType\":" + (int)dataS.meshShapeType);
            CommaNL();
            Indent(); jsonWriter.Write("\"normalOffset\":" + dataS.normalOffset);
            CommaNL();
            Indent(); jsonWriter.Write("\"radius\":" + dataS.radius);
            CommaNL();
            Indent(); jsonWriter.Write("\"randomDirectionAmount\":" + dataS.randomDirectionAmount);
            CommaNL();
            Indent(); jsonWriter.Write("\"shapeType\":\"" + dataS.shapeType + "\"");
            if (dataS.box != null)
            {
                CommaNL();
                Indent(); jsonWriter.Write("\"box\":[" + dataS.box.x + "," + dataS.box.y + "," + dataS.box.z + "]");
            }
            CommaNL();
            Indent(); jsonWriter.Write("\"sphericalDirectionAmount\":" + dataS.sphericalDirectionAmount);
            CommaNL();
            Indent(); jsonWriter.Write("\"useMeshColors\":" + (dataS.useMeshColors == true ? "true" : "false"));
            CommaNL();
            Indent(); jsonWriter.Write("\"useMeshMaterialIndex\":" + (dataS.useMeshMaterialIndex == true ? "true" : "false"));

            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        private void WriteVelocityOverLifetime()
        {
            var dataV = particleSys.velocityOverLifetime;
            if (!dataV.enabled) return;
            CommaNL();
            Indent(); jsonWriter.Write("\"velocityOverLifetime\":{\n");
            IndentIn();
            //CommaNL();
            //Indent();	jsonWriter.Write ("\"xMultiplier\":"+ dataV.xMultiplier);
            //CommaNL();
            //Indent();	jsonWriter.Write ("\"yMultiplier\":"+ dataV.yMultiplier);
            //CommaNL();
            //Indent();	jsonWriter.Write ("\"zMultiplier\":"+ dataV.zMultiplier);

            WriteMinMaxCurve(dataV.x, "x");
            WriteMinMaxCurve(dataV.y, "y");
            WriteMinMaxCurve(dataV.z, "z");

            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        private void WriteLimitVelocityOverLifetime()
        {
            var dataL = particleSys.limitVelocityOverLifetime;
            if (!dataL.enabled) return;

            CommaNL();
            Indent(); jsonWriter.Write("\"limitVelocityOverLifetime\":{\n");
            IndentIn();

            CommaNL();
            Indent(); jsonWriter.Write("\"dampen\":" + dataL.dampen);

            if (dataL.separateAxes)
            {
                CommaNL();
                Indent(); jsonWriter.Write("\"limitMultiplier\":" + dataL.limitMultiplier);
                CommaNL();
                Indent(); jsonWriter.Write("\"limitXMultiplier\":" + dataL.limitXMultiplier);
                CommaNL();
                Indent(); jsonWriter.Write("\"limitYMultiplier\":" + dataL.limitYMultiplier);
                CommaNL();
                Indent(); jsonWriter.Write("\"limitZMultiplier\":" + dataL.limitZMultiplier);
            }
            CommaNL();
            Indent(); jsonWriter.Write("\"space\":" + (int)dataL.space);
            WriteMinMaxCurve(dataL.limit, "limit");
            WriteMinMaxCurve(dataL.limitX, "limitX");
            WriteMinMaxCurve(dataL.limitY, "limitY");
            WriteMinMaxCurve(dataL.limitZ, "limitZ");

            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        private void WriteForceOverLifetime()
        {
            var dataF = particleSys.forceOverLifetime;
            if (!dataF.enabled) return;

            CommaNL();
            Indent(); jsonWriter.Write("\"forceOverLifetime\":{\n");
            IndentIn();

            //CommaNL();
            //Indent();	jsonWriter.Write ("\"randomized\":"+ dataF.randomized);
            //CommaNL();
            //Indent();	jsonWriter.Write ("\"space\":"+ (int)dataF.space);

            WriteMinMaxCurve(dataF.x, "x");
            WriteMinMaxCurve(dataF.y, "y");
            WriteMinMaxCurve(dataF.z, "z");
            IndentOut();
            Indent(); jsonWriter.Write("}");

        }

        private void WriteColorOverLifetime()
        {
            var dataC = particleSys.colorOverLifetime;
            if (!dataC.enabled) return;

            CommaNL();
            Indent(); jsonWriter.Write("\"colorOverLifetime\":{\n");
            IndentIn();

            WriteMinMaxGradient(dataC.color, "color");

            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        private void WriteSizeOverLifetime()
        {
            var dataS = particleSys.sizeOverLifetime;
            if (!dataS.enabled || dataS.separateAxes) return;

            CommaNL();
            Indent(); jsonWriter.Write("\"sizeOverLifetime\":{\n");
            IndentIn();

            WriteMinMaxCurve(dataS.size, "size");

            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        private void WriteRotationOverLifetime()
        {
            var dataR = particleSys.rotationOverLifetime;
            if (!dataR.enabled) return;
            CommaNL();
            Indent(); jsonWriter.Write("\"rotationOverLifetime\":{\n");
            IndentIn();
            //if(dataR.separateAxes){
            /*if(false){
                CommaNL();
                Indent();	jsonWriter.Write ("\"xMultiplier\":"+ dataR.xMultiplier);
                CommaNL();
                Indent();	jsonWriter.Write ("\"yMultiplier\":"+ dataR.yMultiplier);
                CommaNL();
                Indent();	jsonWriter.Write ("\"zMultiplier\":"+ dataR.zMultiplier);
            }*/

            WriteMinMaxCurve(dataR.x, "x");
            WriteMinMaxCurve(dataR.y, "y");
            WriteMinMaxCurve(dataR.z, "z");

            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        private void WriteRender()
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"renderer\":{");
            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("\"sortingFudge\":" + renderer.sortingFudge);
            CommaNL();
            Indent(); jsonWriter.Write("\"renderMode\":" + (int)renderer.renderMode);

            IndentOut();
            Indent(); jsonWriter.Write("}\n");
        }


        //通用解析数据
        private void WriteMinMaxCurve(ParticleSystem.MinMaxCurve data, string name)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"" + name + "\":{\n");
            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("\"mode\":" + (int)data.mode);
            CommaNL();

            switch (data.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    Indent(); jsonWriter.Write("\"constant\":" + data.constant + "\n");
                    break;
                case ParticleSystemCurveMode.Curve:
                    //Indent(); jsonWriter.Write("\"curveMultiplier\":" + data.curveMultiplier);
                    //WriteAnimationCurve(data.curve, "curve");
                    WriteAnimationCurve1(data.curve, "curve", particleSys.main.duration);
                    break;
                case ParticleSystemCurveMode.TwoConstants:
                    Indent(); jsonWriter.Write("\"constantMax\":" + data.constantMax);
                    CommaNL();
                    Indent(); jsonWriter.Write("\"constantMin\":" + data.constantMin + "\n");
                    break;
                case ParticleSystemCurveMode.TwoCurves:
                    //Indent(); jsonWriter.Write("\"curveMultiplier\":" + data.curveMultiplier);
                    WriteAnimationCurve2(data.curveMin, data.curveMax, "curveMultiplier", particleSys.main.duration);
                    //WriteAnimationCurve(data.curveMax, "curveMax");
                    //WriteAnimationCurve(data.curveMin, "curveMin");
                    break;
                default:
                    break;
            }

            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        private void WriteAnimationCurve(AnimationCurve data, string name)
        {
            if (data == null)
                return;
            CommaNL();
            Indent(); jsonWriter.Write("\"" + name + "\":{\n");
            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("\"keys\":[\n");
            IndentIn();
            foreach (var v in data.keys)
            {
                WriteKeyframe(v);
            }
            IndentOut();
            Indent(); jsonWriter.Write("]\n");
            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        /// <summary>
        /// 时间线 限制 - 在一条曲线上取值
        /// </summary>
        /// <param name="dataMin"></param>
        /// <param name="dataMax"></param>
        /// <param name="name"></param>
        /// <param name="duration"></param>
        private void WriteAnimationCurve1(AnimationCurve dataMin, string name, float duration)
        {
            if (particleSys == null)
                return;
            CommaNL();
            Indent(); jsonWriter.Write("\"" + name + "\":{\n");



            int lenMin = dataMin.keys.Length;

            if (lenMin > 0)
            {
                float minBase = dataMin.keys[0].value;

                CommaNL();
                Indent(); jsonWriter.Write("\"startValue\":" + minBase);

                IndentIn();
                CommaNL();
                Indent(); jsonWriter.Write("\"keys\":[\n");
                IndentIn();

                int index = 0;
                for (index = 0; index <= lenMin; index++)
                {
                    WriteKeyframe1(dataMin.keys[index], minBase, duration);
                }

                IndentOut();
                Indent(); jsonWriter.Write("]\n");
                IndentOut();

            }
            else
            {
                Debug.LogError("关键帧数据错误：" + name + "");
            }


            Indent(); jsonWriter.Write("}");
        }

        /// <summary>
        /// 上下限 限制
        /// </summary>
        /// <param name="dataMin"></param>
        /// <param name="dataMax"></param>
        /// <param name="name"></param>
        /// <param name="duration"></param>
        private void WriteAnimationCurve2(AnimationCurve dataMin, AnimationCurve dataMax, string name, float duration)
        {
            if (particleSys == null)
                return;
            CommaNL();
            Indent(); jsonWriter.Write("\"" + name + "\":{\n");



            int lenMin = dataMin.keys.Length;
            int lenMax = dataMax.keys.Length;

            if (lenMin > 0 && lenMin == lenMax)
            {
                float minBase = dataMin.keys[0].value;
                float maxBase = dataMax.keys[0].value;

                CommaNL();
                Indent(); jsonWriter.Write("\"startValue\":[" + minBase + "," + maxBase + "]");

                IndentIn();
                CommaNL();
                Indent(); jsonWriter.Write("\"keys\":[\n");
                IndentIn();

                int index = 0;
                for (index = 0; index <= lenMin; index++)
                {
                    WriteKeyframe2(dataMin.keys[index], dataMax.keys[index], minBase, maxBase, duration);
                }

                IndentOut();
                Indent(); jsonWriter.Write("]\n");
                IndentOut();

            } else
            {
                Debug.LogError("关键帧数据错误：" + name + "");
            }


            Indent(); jsonWriter.Write("}");
        }

        private void WriteKeyframe(Keyframe data)
        {

            CommaNL();
            Indent(); jsonWriter.Write("{\n");
            IndentIn();

            //CommaNL();
            //Indent(); jsonWriter.Write("\"inTangent\":" + data.inTangent);
            //CommaNL();
            //Indent(); jsonWriter.Write("\"outTangent\":" + data.outTangent);

            CommaNL();
            Indent(); jsonWriter.Write("\"time\":" + data.time);
            CommaNL();
            Indent(); jsonWriter.Write("\"value\":" + data.value);
            CommaNL();
            Indent(); jsonWriter.Write("\"tangentMode\":" + data.tangentMode);
            IndentOut();
            Indent(); jsonWriter.Write("}");

        }

        private void WriteKeyframe2(Keyframe dataMin, Keyframe dataMax, float baseMin, float baseMax, float duration)
        {

            //CommaNL();
            //Indent(); jsonWriter.Write("{\n");
            //IndentIn();

            ////CommaNL();
            ////Indent(); jsonWriter.Write("\"inTangent\":" + data.inTangent);
            ////CommaNL();
            ////Indent(); jsonWriter.Write("\"outTangent\":" + data.outTangent);

            //CommaNL();
            //Indent(); jsonWriter.Write("\"time\":" + dataMin.time / duration);

            //CommaNL();
            //Indent(); jsonWriter.Write("\"factor\":" + dataMin.value / baseMin);

            //CommaNL();
            //Indent(); jsonWriter.Write("\"factor2\":" + dataMax.value / baseMax);

            ////CommaNL();
            ////Indent(); jsonWriter.Write("\"tangentMode\":" + data.tangentMode);

            //IndentOut();
            //Indent(); jsonWriter.Write("}");


            /////

            CommaNL();
            Indent(); jsonWriter.Write("[" + dataMin.time / duration + "," + dataMin.value / baseMin + "," + dataMax.value / baseMax + "]");

        }

        private void WriteKeyframe1(Keyframe dataMin, float baseMin, float duration)
        {

            //CommaNL();
            //Indent(); jsonWriter.Write("{\n");
            //IndentIn();

            ////CommaNL();
            ////Indent(); jsonWriter.Write("\"inTangent\":" + data.inTangent);
            ////CommaNL();
            ////Indent(); jsonWriter.Write("\"outTangent\":" + data.outTangent);

            //CommaNL();
            //Indent(); jsonWriter.Write("\"time\":" + dataMin.time / duration);

            //CommaNL();
            //Indent(); jsonWriter.Write("\"factor\":" + dataMin.value / baseMin);

            //CommaNL();
            //Indent(); jsonWriter.Write("\"factor2\":" + dataMax.value / baseMax);

            ////CommaNL();
            ////Indent(); jsonWriter.Write("\"tangentMode\":" + data.tangentMode);

            //IndentOut();
            //Indent(); jsonWriter.Write("}");


            /////

            CommaNL();
            Indent(); jsonWriter.Write("[" + dataMin.time / duration + "," + dataMin.value / baseMin + "]");

        }

        private void WriteMinMaxGradient(ParticleSystem.MinMaxGradient data, string name)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"" + name + "\":{");
            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("\"mode\":" + (int)data.mode);
            switch (data.mode)
            {
                case ParticleSystemGradientMode.Color:
                    WriteColor(data.color, "color");
                    break;
                case ParticleSystemGradientMode.Gradient:
                    WriteGradient(data.gradient, "gradient");
                    break;
                case ParticleSystemGradientMode.RandomColor:
                    break;
                case ParticleSystemGradientMode.TwoColors:
                    WriteColor(data.colorMax, "colorMax");
                    WriteColor(data.colorMin, "colorMin");
                    break;
                case ParticleSystemGradientMode.TwoGradients:
                    WriteGradient(data.gradientMax, "gradientMax");
                    WriteGradient(data.gradientMin, "gradientMin");
                    break;
                default:
                    break;
            }

            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        private void WriteColor(Color data, string name)
        {
            if (data == null)
                return;
            CommaNL();
            Indent(); jsonWriter.Write("\"" + name + "\":[\n");
            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write(data.r);
            CommaNL();
            Indent(); jsonWriter.Write(data.g);
            CommaNL();
            Indent(); jsonWriter.Write(data.b);
            CommaNL();
            Indent(); jsonWriter.Write(data.a + "\n");
            IndentOut();
            Indent(); jsonWriter.Write("]");
        }

        private void WriteGradient(Gradient data, string name)
        {
            if (data == null)
                return;
            CommaNL();
            Indent(); jsonWriter.Write("\"" + name + "\":{\n");
            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("\"alphaKeys\":[\n");
            IndentIn();
            foreach (var v in data.alphaKeys)
            {
                WriteGradientAlphaKey(v);
            }
            IndentOut();
            Indent(); jsonWriter.Write("]");
            CommaNL();
            Indent(); jsonWriter.Write("\"colorKeys\":[\n");
            IndentIn();

            foreach (var v in data.colorKeys)
            {
                WriteGradientColorKey(v);
            }

            IndentOut();
            Indent(); jsonWriter.Write("]");
            CommaNL();
            Indent(); jsonWriter.Write("\"mode\":" + (int)data.mode);
            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        private void WriteGradientAlphaKey(GradientAlphaKey data)
        {
            CommaNL();
            Indent(); jsonWriter.Write("{\n");
            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("\"time\":" + data.time);
            CommaNL();
            Indent(); jsonWriter.Write("\"alpha\":" + data.alpha + "\n");
            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        private void WriteGradientColorKey(GradientColorKey data)
        {
            CommaNL();
            Indent(); jsonWriter.Write("{\n");
            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("\"time\":" + data.time);
            WriteColor(data.color, "color");
            IndentOut();
            Indent(); jsonWriter.Write("}");
        }

        private void WriteBurst(UnityEngine.ParticleSystem.Burst data)
        {
            CommaNL();
            Indent(); jsonWriter.Write("{\n");
            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("\"minCount\":" + data.minCount);
            CommaNL();
            Indent(); jsonWriter.Write("\"maxCount\":" + data.maxCount);
            CommaNL();
            Indent(); jsonWriter.Write("\"time\":" + data.time);
            IndentOut();
            Indent(); jsonWriter.Write("}\n");
        }

    }
}
#endif