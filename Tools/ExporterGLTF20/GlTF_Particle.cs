#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlTF_Particle : GlTF_Writer {
	public ParticleSystem particleSys = null;
	public ParticleSystemRenderer renderer = null;
	public float textureIndex = -1;
    private string uvOffset = "";
    private string uvScale = "";
    private int alphaMode = 0;

    public int texWidth;
    public int texHeight;

    public Color startColor = new Color(1, 1, 1);

    public void initParticleSystem (Transform tr){
        particleSys     = tr.GetComponent<ParticleSystem>();
		renderer        = tr.GetComponent<ParticleSystemRenderer> ();
    }

    public void setTextureIndex(int index)
    {
        textureIndex = index;
    }
    public void setTextureScale(string str)
    {
        uvScale = str;
    }
    public void setTextureOffset(string str)
    {
        uvOffset = str;
    }
    public void setAlphaMode(int mode)
    {
        alphaMode = mode;
    }


    public GlTF_Particle () {  }

	public override void Write()
	{
		
		IndentIn();
		CommaNL();
		Indent();	jsonWriter.Write ("{\n");

		IndentIn();
		WriteTexture ();
		WriteMain();
		WriteEmission();
		WriteShape();

		WriteVelocityOverLifetime();
		WriteLimitVelocityOverLifetime();

		WriteForceOverLifetime();
		WriteColorOverLifetime();
		WriteSizeOverLifetime ();
		WriteRotationOverLifetime();

        WriteTextureSheetAnimation(particleSys.textureSheetAnimation);

        WriteRender();


        IndentOut();
		Indent();	jsonWriter.Write ("}");
		IndentOut();
	}

	//面板导出
	private void WriteTexture(){
		CommaNL();
		Indent();	jsonWriter.Write ("\"textureIndex\":{\n" );
		IndentIn();
		CommaNL();
		Indent();	jsonWriter.Write ("\"index\":" + textureIndex + "\n" );
        IndentOut();
		Indent();	jsonWriter.Write ("}");
	}
	private void WriteMain(){
		CommaNL();
		Indent();	jsonWriter.Write ("\"main\":{\n" );
		IndentIn();

        if (uvOffset != "")
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"uvOffset\":" + uvOffset);
        }
        if (uvScale != "")
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"uvScale\":" + uvScale);
        }

        CommaNL();
        Indent(); jsonWriter.Write("\"blend\":" + alphaMode + "");

        CommaNL();
		Indent();	jsonWriter.Write ("\"loop\":"+ (particleSys.main.loop ? "true" : "false") + "");

		CommaNL();
		Indent();	jsonWriter.Write ("\"maxParticles\":"+ particleSys.main.maxParticles );

		CommaNL();
		Indent();	jsonWriter.Write ("\"playOnAwake\":"+ (particleSys.main.playOnAwake == true?"true":"false" ) );

		CommaNL();
		Indent();	jsonWriter.Write ("\"randomizeRotationDirection\":"+ particleSys.main.randomizeRotationDirection );

		CommaNL();
		Indent();	jsonWriter.Write ("\"simulationSpeed\":"+ particleSys.main.simulationSpeed );

		if (particleSys.main.prewarm) {
			CommaNL();
			Indent();	jsonWriter.Write ("\"prewarm\":true");
		} else {
			WriteMinMaxCurve(particleSys.main.startDelay, "startDelay");
		}

		WriteMinMaxCurve(particleSys.main.duration, "duration");

		WriteStartColor(particleSys.main.startColor, "startColor");

		WriteMinMaxCurve(particleSys.main.startSpeed, "startSpeed");

		WriteMinMaxCurve(particleSys.main.startLifetime, "startLifetime");

		if (particleSys.main.startSize3D) {

            CommaNL();
            Indent(); jsonWriter.Write("\"startSize3D\":{\n");
            IndentIn();

            WriteMinMaxCurve (particleSys.main.startSizeX, "startSizeX");
			WriteMinMaxCurve (particleSys.main.startSizeY, "startSizeY");
			WriteMinMaxCurve (particleSys.main.startSizeZ, "startSizeZ");
            jsonWriter.Write("\n");

            IndentOut();
            Indent(); jsonWriter.Write("}");
        } else {
			WriteMinMaxCurve(particleSys.main.startSize, "startSize");
		}

		if (particleSys.main.startRotation3D) {
			//WriteMinMaxCurve (particleSys.main.startRotationX, "startRotationX");
			//WriteMinMaxCurve (particleSys.main.startRotationY, "startRotationY");
			WriteMinMaxCurve (particleSys.main.startRotationZ, "startRotation");
		} else
        {
            WriteMinMaxCurve(particleSys.main.startRotation, "startRotation");
        }

		WriteMinMaxCurve(particleSys.main.gravityModifier, "gravityModifier");
        jsonWriter.Write("\n");

        IndentOut();
		Indent();	jsonWriter.Write ("}");
	}

	private void WriteEmission()
	{
		var dataE = particleSys.emission;
		if (!dataE.enabled) return;
		CommaNL();
		Indent();	jsonWriter.Write ("\"emission\":{\n" );
		IndentIn();
		var bursts = new UnityEngine.ParticleSystem.Burst[dataE.burstCount];
		dataE.GetBursts(bursts);
		CommaNL();
		Indent();	jsonWriter.Write ("\"rateOverDistanceMultiplier\":"+ dataE.rateOverDistanceMultiplier );
		CommaNL();
		Indent();	jsonWriter.Write ("\"rateOverTimeMultiplier\":"+ dataE.rateOverTimeMultiplier );

		WriteMinMaxCurve(dataE.rateOverDistance, "rateOverDistance");
		WriteMinMaxCurve(dataE.rateOverTime, "rateOverTime");

		CommaNL();
		Indent();	jsonWriter.Write ("\"bursts\":[\n" );
		IndentIn();
	
		if(bursts != null){
			foreach (var v in bursts)
				WriteBurst(v);
		}
        jsonWriter.Write("\n");
        IndentOut();
		Indent();	jsonWriter.Write ("]\n");
		/// bursts: [{time: 0.1, min: 3, max: 3}];
		//scope.WriteKeyValue("burstCount", data.burstCount);

		IndentOut();
		Indent();	jsonWriter.Write ("}");
	}

	private void WriteShape()
	{
		var dataS = particleSys.shape;
		if (!dataS.enabled) return;
		CommaNL();
		Indent();	jsonWriter.Write ("\"shape\":{\n" );
		IndentIn();
		CommaNL();
		Indent();	jsonWriter.Write ("\"alignToDirection\":"+  (dataS.alignToDirection == true?"true":"false" ));
		CommaNL();
		Indent();	jsonWriter.Write ("\"angle\":"+ dataS.angle / 180 * Mathf.PI);
		CommaNL();
		Indent();	jsonWriter.Write ("\"arc\":"+ dataS.arc);
		CommaNL();
		Indent();	jsonWriter.Write ("\"length\":"+ dataS.length);
		CommaNL();
		Indent();	jsonWriter.Write ("\"meshScale\":"+ dataS.meshScale);
		CommaNL();
		Indent();	jsonWriter.Write ("\"meshShapeType\":"+(int)dataS.meshShapeType);
		CommaNL();
		Indent();	jsonWriter.Write ("\"normalOffset\":"+ dataS.normalOffset);
		CommaNL();
		Indent();	jsonWriter.Write ("\"radius\":"+ dataS.radius);
		CommaNL();
		Indent();	jsonWriter.Write ("\"randomDirection\":"+ dataS.randomDirectionAmount);
		CommaNL();
		Indent();	jsonWriter.Write ("\"shapeType\":\""+ dataS.shapeType + "\"");
		if (dataS.box != null) {
			CommaNL();
			Indent();	jsonWriter.Write ("\"box\":["+ dataS.box.x + "," +  dataS.box.y + "," +  dataS.box.z + "]");
		}
		CommaNL();
		Indent();	jsonWriter.Write ("\"sphericalDirectionAmount\":"+ dataS.sphericalDirectionAmount);
		CommaNL();
		Indent();	jsonWriter.Write ("\"useMeshColors\":"+ (dataS.useMeshColors == true?"true":"false" ));
		CommaNL();
		Indent();	jsonWriter.Write ("\"useMeshMaterialIndex\":" + (dataS.useMeshMaterialIndex == true?"true":"false" ) + "\n");

		IndentOut();
		Indent();	jsonWriter.Write ("}");
	}

	private void WriteVelocityOverLifetime()
	{
		var dataV = particleSys.velocityOverLifetime;
		if (!dataV.enabled) return;
		CommaNL();
		Indent();	jsonWriter.Write ("\"velocityOverLifetime\":{ \"importPoss\": true,\n" );
		IndentIn();

        var modeX = dataV.x.mode;
        var modeY = dataV.y.mode;
        var modeZ = dataV.z.mode;
        var mode = modeX;


        CommaNL();

        switch (mode)
        {
            case (ParticleSystemCurveMode.Constant):
                {
                    WriteVelocityOverLifetimeConstant(dataV);
                    Indent(); jsonWriter.Write("\"mode\":"+ 0);
                    break;
                }
            case (ParticleSystemCurveMode.Curve):
                {
                    WriteVelocityOverLifetimeCurve(dataV);
                    Indent(); jsonWriter.Write("\"mode\":" + 1);
                    break;
                }
            case (ParticleSystemCurveMode.TwoCurves):
                {
                    WriteVelocityOverLifetimeTwoCurve(dataV);
                    Indent(); jsonWriter.Write("\"mode\":" + 2);
                    break;
                }
            case (ParticleSystemCurveMode.TwoConstants):
                {
                    WriteVelocityOverLifetimeTwoConstant(dataV);
                    Indent(); jsonWriter.Write("\"mode\":" + 3);
                    break;
                }
        }
        //CommaNL();
        //Indent();	jsonWriter.Write ("\"xMultiplier\":"+ dataV.xMultiplier);
        //CommaNL();
        //Indent();	jsonWriter.Write ("\"yMultiplier\":"+ dataV.yMultiplier);
        //CommaNL();
        //Indent();	jsonWriter.Write ("\"zMultiplier\":"+ dataV.zMultiplier);

  //      WriteMinMaxCurve( dataV.x, "x");
		//WriteMinMaxCurve( dataV.y, "y");
		//WriteMinMaxCurve( dataV.z, "z");

        jsonWriter.Write("\n");

        IndentOut();
		Indent();	jsonWriter.Write ("}");
	}

    private void WriteVelocityOverLifetimeConstant(ParticleSystem.VelocityOverLifetimeModule velocity)
    {
        float yDiffMin = 0.0f;
        float yDiffMax = 0.0f;

        if (particleSys.main.startSpeed.mode == ParticleSystemCurveMode.Constant)
        {
            yDiffMin = yDiffMax = -particleSys.main.startSpeed.constant;
        }
        else if (particleSys.main.startSpeed.mode == ParticleSystemCurveMode.TwoConstants)
        {
            yDiffMin = (-particleSys.main.startSpeed.constantMin > -particleSys.main.startSpeed.constantMax) ? -particleSys.main.startSpeed.constantMax : -particleSys.main.startSpeed.constantMin;
            yDiffMax = (-particleSys.main.startSpeed.constantMin < -particleSys.main.startSpeed.constantMax) ? -particleSys.main.startSpeed.constantMax : -particleSys.main.startSpeed.constantMin;
        }

        yDiffMin = 0.0f;
        yDiffMax = 0.0f;
        Indent(); jsonWriter.Write("\"constant\":[" + velocity.x.constant + "," + (velocity.y.constant + yDiffMin) + "," + velocity.z.constant + "]" );
        CommaNL();
    }
    private void WriteVelocityOverLifetimeTwoConstant(ParticleSystem.VelocityOverLifetimeModule velocity)
    {
        float yDiffMin = 0.0f;
        float yDiffMax = 0.0f;

        if (particleSys.main.startSpeed.mode == ParticleSystemCurveMode.Constant)
        {
            yDiffMin = yDiffMax = -particleSys.main.startSpeed.constant;
        }
        else if (particleSys.main.startSpeed.mode == ParticleSystemCurveMode.TwoConstants)
        {
            yDiffMin = (-particleSys.main.startSpeed.constantMin > -particleSys.main.startSpeed.constantMax) ? -particleSys.main.startSpeed.constantMax : -particleSys.main.startSpeed.constantMin;
            yDiffMax = (-particleSys.main.startSpeed.constantMin < -particleSys.main.startSpeed.constantMax) ? -particleSys.main.startSpeed.constantMax : -particleSys.main.startSpeed.constantMin;
        }
        yDiffMin = 0.0f;
        yDiffMax = 0.0f;
        Indent(); jsonWriter.Write("\"constantMin\":[" + velocity.x.constantMin + "," + (velocity.y.constantMin + yDiffMin) + "," + velocity.z.constantMin + "],\n");
        Indent(); jsonWriter.Write("\"constantMax\":[" + velocity.x.constantMax + "," + (velocity.y.constantMax + yDiffMax) + "," + velocity.z.constantMax + "]");
        CommaNL();
    }

    private void WriteVelocityOverLifetimeCurve(ParticleSystem.VelocityOverLifetimeModule velocity)
    {
        List<float> baseValue = new List<float>();
        List<float> curveValue = new List<float>();

        float yDiff = 0.0f;

        if (particleSys.main.startSpeed.constant > 0)
        {
            yDiff = -particleSys.main.startSpeed.constant;
        }
        yDiff = 0.0f;

        getVelocityOverLifetimeCurve(velocity.x.curve, velocity.y.curve, velocity.z.curve, baseValue, curveValue, yDiff);

        int count = baseValue.Count;
        int index = 0;

        if (count == 3)
        {
            Indent(); jsonWriter.Write("\"base\":[" + baseValue[0] + "," + baseValue[1] + "," + baseValue[2] + "]");
            CommaNL();

            count = curveValue.Count;
            Indent(); jsonWriter.Write("\"curve\":[\n");

            IndentIn();
            CommaNL();
            for (index = 0; index < count; index = index + 2)
            {
                Indent(); jsonWriter.Write("[" + curveValue[index] + "," + curveValue[index + 1] + "]");
                if (index == count - 2)
                {
                    Indent(); jsonWriter.Write("\n");
                }
                else
                {
                    CommaNL();
                }
            }

            IndentOut();
            Indent(); jsonWriter.Write("]");
            CommaNL();
        }
    }

    private void getVelocityOverLifetimeCurve(AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, List<float> baseValue, List<float> curveValue, float yDiff)
    {
        yDiff = 0.0f;
        List<float> timesX  = new List<float>();
        List<float> valuesX = new List<float>();
        List<float> timesY  = new List<float>();
        List<float> valuesY = new List<float>();
        List<float> timesZ  = new List<float>();
        List<float> valuesZ = new List<float>();
        float maxX = 0.0f;
        float maxY = 0.0f;
        float maxZ = 0.0f;
        float baseX = 0.0f;
        float baseY = 0.0f;
        float baseZ = 0.0f;

        int count = curveX.keys.Length;
        int index = 0;
        float temp = 0.0f;
        Keyframe[] maxKeyFrames = null;
        float tempMax = 0.0f;
        
        for (index = 0; index < count; index++)
        {
            timesX.Add(curveX.keys[index].time);
            valuesX.Add(curveX.keys[index].value);
            temp = Mathf.Abs(curveX.keys[index].value);
            if (temp > maxX)
            {
                maxX = temp;
                baseX = curveX.keys[index].value;
            }
        }
        maxKeyFrames = curveX.keys;
        tempMax = maxX;

        count = curveY.keys.Length;
        temp = 0.0f;
        for (index = 0; index < count; index++)
        {
            timesY.Add(curveY.keys[index].time);
            valuesY.Add(curveY.keys[index].value + yDiff);
            temp = Mathf.Abs(curveY.keys[index].value + yDiff);
            if (temp > maxY)
            {
                maxY = temp;
                baseY = curveY.keys[index].value + yDiff;
            }
        }
        if (maxKeyFrames.Length < curveY.keys.Length)
        {
            maxKeyFrames = curveY.keys;
            tempMax = maxY;
        }

        count = curveZ.keys.Length;
        temp = 0.0f;
        for (index = 0; index < count; index++)
        {
            timesZ.Add(curveZ.keys[index].time);
            valuesZ.Add(curveZ.keys[index].value);
            temp = Mathf.Abs(curveZ.keys[index].value);
            if (temp > maxZ)
            {
                maxZ = temp;
                baseZ = curveZ.keys[index].value;
            }
        }
        if (maxKeyFrames.Length < curveZ.keys.Length)
        {
            maxKeyFrames = curveZ.keys;
            tempMax = maxZ;
        }

        baseValue.Add(baseX);
        baseValue.Add(baseY);
        baseValue.Add(baseZ);

        count = maxKeyFrames.Length;
        if (count > 0)
        {

            float tempValue;
            for (index = 0; index < count; index++)
            {
                temp = maxKeyFrames[index].time;
                tempValue = maxKeyFrames[index].value;

                tempValue = (maxX == 0 || tempValue == 0 || tempMax == 0) ? 0 : (tempValue / tempMax );

                tempValue = Mathf.Abs(tempValue) < 0.00001 ? 0 : tempValue;

                curveValue.Add(temp);
                curveValue.Add(tempValue);
            }
            
        }
        
    }
    private void WriteVelocityOverLifetimeTwoCurve(ParticleSystem.VelocityOverLifetimeModule velocity)
    {

        List<float> baseValue = new List<float>();
        List<float> curveValue = new List<float>();
        List<float> baseValue2 = new List<float>();
        List<float> curveValue2 = new List<float>();


        float yDiffMin = 0.0f;
        float yDiffMax = 0.0f;

        if (particleSys.main.startSpeed.mode == ParticleSystemCurveMode.Constant)
        {
            yDiffMin = yDiffMax = -particleSys.main.startSpeed.constant;
        }
        else if (particleSys.main.startSpeed.mode == ParticleSystemCurveMode.TwoConstants)
        {
            yDiffMin = (-particleSys.main.startSpeed.constantMin > -particleSys.main.startSpeed.constantMax) ? -particleSys.main.startSpeed.constantMax : -particleSys.main.startSpeed.constantMin;
            yDiffMax = (-particleSys.main.startSpeed.constantMin < -particleSys.main.startSpeed.constantMax) ? -particleSys.main.startSpeed.constantMax : -particleSys.main.startSpeed.constantMin;
        }

        getVelocityOverLifetimeCurve(velocity.x.curveMin, velocity.y.curveMin, velocity.z.curveMin, baseValue, curveValue, yDiffMin);
        getVelocityOverLifetimeCurve(velocity.x.curveMax, velocity.y.curveMax, velocity.z.curveMax, baseValue2, curveValue2, yDiffMax);

        int count = baseValue.Count;
        int index = 0;

        if (count == 3 && curveValue.Count == curveValue2.Count)
        {
            Indent(); jsonWriter.Write("\"baseMin\":[" + baseValue[0] + "," + baseValue[1] + "," + baseValue[2] + "]");
            CommaNL();

            Indent(); jsonWriter.Write("\"baseMax\":[" + baseValue2[0] + "," + baseValue2[1] + "," + baseValue2[2] + "]");
            CommaNL();

            count = curveValue.Count;
            Indent(); jsonWriter.Write("\"curveMultiplier\":[\n");

            IndentIn();
            CommaNL();
            for (index = 0; index < count; index = index + 2)
            {
                Indent(); jsonWriter.Write("[" + curveValue[index] + "," + curveValue[index + 1] + "," + +curveValue2[index + 1] + "]");
                if (index == count - 2)
                {
                    Indent(); jsonWriter.Write("\n");
                }
                else
                {
                    CommaNL();
                }
            }

            IndentOut();

            Indent(); jsonWriter.Write("]");
            CommaNL();
        }
    }


    private void WriteLimitVelocityOverLifetime()
	{
		var dataL = particleSys.limitVelocityOverLifetime;
		if (!dataL.enabled) return;

		CommaNL();
		Indent();	jsonWriter.Write ("\"limitVelocityOverLifetime\":{\n" );
		IndentIn();

		CommaNL();
		Indent();	jsonWriter.Write ("\"dampen\":"+ dataL.dampen);

		if(dataL.separateAxes){
            Debug.LogError("不支持 limit Velocity Over Lifetime separateAxes 导出！");

			//CommaNL();
			//Indent();	jsonWriter.Write ("\"limitMultiplier\":"+ dataL.limitMultiplier);
			//CommaNL();
			//Indent();	jsonWriter.Write ("\"limitXMultiplier\":"+ dataL.limitXMultiplier);
			//CommaNL();
			//Indent();	jsonWriter.Write ("\"limitYMultiplier\":"+ dataL.limitYMultiplier);
			//CommaNL();
			//Indent();	jsonWriter.Write ("\"limitZMultiplier\":"+ dataL.limitZMultiplier);
		}

		CommaNL();
		Indent();	jsonWriter.Write ("\"space\":"+ (int)dataL.space);

		WriteMinMaxCurve( dataL.limit, "limit");
		//WriteMinMaxCurve(dataL.limitX, "limitX");
		//WriteMinMaxCurve(dataL.limitY, "limitY");
		//WriteMinMaxCurve(dataL.limitZ, "limitZ");

		IndentOut();
		Indent();	jsonWriter.Write ("}");
	}

	private void WriteForceOverLifetime()
	{
		var dataF = particleSys.forceOverLifetime;
		if (!dataF.enabled) return;

		CommaNL();
		Indent();	jsonWriter.Write ("\"forceOverLifetime\":{\n" );
		IndentIn();

		//CommaNL();
		//Indent();	jsonWriter.Write ("\"randomized\":"+ dataF.randomized);
		//CommaNL();
		//Indent();	jsonWriter.Write ("\"space\":"+ (int)dataF.space);

		WriteMinMaxCurve( dataF.x, "x");
		WriteMinMaxCurve( dataF.y, "y");
		WriteMinMaxCurve(dataF.z, "z");
		IndentOut();
		Indent();	jsonWriter.Write ("}");

	}

	private void WriteColorOverLifetime()
	{
		var dataC = particleSys.colorOverLifetime;
		if (!dataC.enabled) return;

		//CommaNL();
		//Indent();	jsonWriter.Write ("\"colorOverLifetime\":{\n" );
		//IndentIn();

		WriteMinMaxGradient( dataC.color, "colorOverLifetime");

  //      jsonWriter.Write("\n");
  //      IndentOut();
		//Indent();	jsonWriter.Write ("}");
	}

	private void WriteSizeOverLifetime()
	{
		var dataS = particleSys.sizeOverLifetime;
		if (!dataS.enabled || dataS.separateAxes) return;

		//CommaNL();
		//Indent();	jsonWriter.Write ("\"sizeOverLifetime\":{\n" );
		//IndentIn();

		WriteMinMaxCurve(dataS.size, "sizeOverLifetime");

  //      jsonWriter.Write("\n");

  //      IndentOut();
		//Indent();	jsonWriter.Write ("}");
	}

	private void WriteRotationOverLifetime()
	{
		var dataR = particleSys.rotationOverLifetime;
		if (!dataR.enabled) return;
		CommaNL();
		Indent();	jsonWriter.Write ("\"rotationOverLifetime\":{\n" );
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
		Indent();	jsonWriter.Write ("}");
    }

    private void  writeNoise()
    {
        if (particleSys.noise.enabled)
        {

            CommaNL();
            Indent(); jsonWriter.Write("\"noise\":{\n");
            IndentIn();


            //CommaNL();
            //Indent(); jsonWriter.Write("\"cellWidth\":" + texWidth / data.numTilesX);

            //CommaNL();
            //Indent(); jsonWriter.Write("\"cellWidth\":" + texWidth / data.numTilesX);

            //CommaNL();
            //Indent(); jsonWriter.Write("\"cellWidth\":" + texWidth / data.numTilesX);


            jsonWriter.Write("\n");
            IndentOut();
            Indent(); jsonWriter.Write("}");
        }
    }

    /// <summary>
    /// 纹理信息获取之后,处理帧动画数据
    /// 只支持匀速播放
    /// </summary>
    /// <param name="data"></param>
    private void WriteTextureSheetAnimation(ParticleSystem.TextureSheetAnimationModule data)
    {
        if (data.enabled)
        {

            texWidth = renderer.sharedMaterial.mainTexture.width;
            texHeight = renderer.sharedMaterial.mainTexture.height;

            CommaNL();
            Indent(); jsonWriter.Write("\"texSheeetAnim\":{\n");
            IndentIn();

            CommaNL();
            Indent(); jsonWriter.Write("\"cellWidth\":" + texWidth / data.numTilesX);

            CommaNL();
            Indent(); jsonWriter.Write("\"cellHeight\":" + texHeight / data.numTilesY);

            if (data.startFrame.mode == ParticleSystemCurveMode.TwoConstants)
            {
                CommaNL();
                Indent(); jsonWriter.Write("\"cellStartID\":" + data.startFrame.constantMin);

                CommaNL();
                Indent(); jsonWriter.Write("\"cellEndID\":" + data.startFrame.constantMax);
            }
            else
            {
                CommaNL();
                Indent(); jsonWriter.Write("\"cellStartID\":" + data.startFrame.constant);

                CommaNL();
                Indent(); jsonWriter.Write("\"cellEndID\":" + (data.numTilesX * data.numTilesY - 1));
            };

            CommaNL();
            Indent(); jsonWriter.Write("\"cellChangeSpeed\":" + data.cycleCount);

            jsonWriter.Write("\n");
            IndentOut();
            Indent(); jsonWriter.Write("}");
        }
    }

    private void WriteRender()
	{
		CommaNL();
		Indent();	jsonWriter.Write ("\"renderer\":{\n" );
		IndentIn();

		CommaNL();
		Indent();	jsonWriter.Write ("\"sortingFudge\":"+ renderer.sortingFudge );
        

        CommaNL();
		Indent();	jsonWriter.Write ("\"renderMode\":"+ (int)renderer.renderMode);

        CommaNL();
        Indent(); jsonWriter.Write("\"povit\":[" + renderer.pivot.x + "," + renderer.pivot.y + "," + renderer.pivot.z + "]");

        //int srcBlend = renderer.sharedMaterial.GetInt("_SrcBlend");
        //int dstBlend = renderer.sharedMaterial.GetInt("_DstBlend");

        //switch (dstBlend)
        //{
        //    case (1):
        //        {
        //            if (srcBlend == 1)
        //            {
        //                CommaNL();
        //                Indent(); jsonWriter.Write("\"blendMode\":" + 0);
        //            }
        //            else if (srcBlend == 3)
        //            {
        //                CommaNL();
        //                Indent(); jsonWriter.Write("\"blendMode\":" + 0);
        //            }
        //            else if (srcBlend == 5)
        //            {
        //                CommaNL();
        //                Indent(); jsonWriter.Write("\"blendMode\":" + 1);
        //            }
        //            break;
        //        }
        //    case (8):
        //        {
        //            if (srcBlend == 1)
        //            {
        //                CommaNL();
        //                Indent(); jsonWriter.Write("\"blendMode\":" + 4);
        //            }
        //            else if (srcBlend == 3)
        //            {
        //                CommaNL();
        //                Indent(); jsonWriter.Write("\"blendMode\":" + 3);
        //            }
        //            break;
        //        }
        //}

        switch (renderer.renderMode)
        {
            case (ParticleSystemRenderMode.Mesh):
                {

                    break;
                }
            case (ParticleSystemRenderMode.None):
                {
                    break;
                }
            case (ParticleSystemRenderMode.Billboard):
                {
                    break;
                }
            case (ParticleSystemRenderMode.Stretch):
                {
                    break;
                }
        }

        jsonWriter.Write("\n");
        IndentOut();
		Indent();	jsonWriter.Write ("}\n");
	}


	//通用解析数据
	private void WriteMinMaxCurve( ParticleSystem.MinMaxCurve data, string name)
	{
		CommaNL();
		Indent();	jsonWriter.Write ("\"" + name + "\":{\n" );
		IndentIn ();
		CommaNL();
		Indent();	jsonWriter.Write ("\"mode\":"+ (int)data.mode);
		CommaNL();
		switch (data.mode)
		{
		case ParticleSystemCurveMode.Constant:
			Indent();	jsonWriter.Write ("\"constant\":"+ data.constant + "\n");
			break;
		case ParticleSystemCurveMode.Curve:
			//Indent();	jsonWriter.Write ("\"curveMultiplier\":"+ data.curveMultiplier);
			//WriteAnimationCurve( data.curve, "curve");
            WriteAnimationCurve1(data.curve, "curve", particleSys.main.duration);
            break;
		case ParticleSystemCurveMode.TwoConstants:
			Indent();	jsonWriter.Write ("\"constantMax\":"+ data.constantMax);
			CommaNL();
			Indent();	jsonWriter.Write ("\"constantMin\":"+ data.constantMin + "\n");
			break;
		case ParticleSystemCurveMode.TwoCurves:
			//Indent();	jsonWriter.Write ("\"curveMultiplier\":"+ data.curveMultiplier);
			//WriteAnimationCurve( data.curveMax, "curveMax");
			//WriteAnimationCurve( data.curveMin, "curveMin");

            WriteAnimationCurve2(data.curveMin, data.curveMax, "curveMultiplier", particleSys.main.duration);
            break;
		default:
			break;
		}
		IndentOut ();
		Indent();	jsonWriter.Write ("}");
	}

	private void WriteAnimationCurve( AnimationCurve data, string name)
	{
		if (data == null)
			return;
		CommaNL();
		Indent();	jsonWriter.Write ("\""+name+"\":{\n" );
		IndentIn ();
		CommaNL();
		Indent();	jsonWriter.Write ("\"keys\":[\n");
		IndentIn();
		foreach(var v in data.keys) {
			WriteKeyframe(v);
		}
		IndentOut ();
		Indent();	jsonWriter.Write ("]\n");
		IndentOut ();
		Indent();	jsonWriter.Write ("}" );
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

        Indent(); jsonWriter.Write("\"" + name + "\":{\n");



        int lenMin = dataMin.keys.Length;

        if (lenMin > 0)
        {
            float minBase = dataMin.keys[0].value;

            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("\"startValue\":" + minBase);

            CommaNL();

            Indent(); jsonWriter.Write("\"keys\":[\n");
            IndentIn();

            int index = 0;
            float time = 0.0f;
            lenMin = 11;
            for (index = 0; index < lenMin; index++)
            {
                //WriteKeyframe1(dataMin.keys[index], minBase, duration);
                time = (float)(index * (1.0 / (lenMin - 1)));
                WriteKeyframe1_(dataMin.Evaluate(time), time, minBase, duration);
            }
            jsonWriter.Write("\n");

            IndentOut();
            Indent(); jsonWriter.Write("]\n");
            IndentOut();

        }
        else
        {
            Debug.LogError("关键帧数据错误：" + name + "");
        }


        Indent(); jsonWriter.Write("}\n");
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

        Indent(); jsonWriter.Write("\"" + name + "\":{\n");



        int lenMin = dataMin.keys.Length;
        int lenMax = dataMax.keys.Length;

        if (lenMin > 0 && lenMin == lenMax)
        {
            float minBase = dataMin.keys[0].value;
            float maxBase = dataMax.keys[0].value;

            IndentIn();
            Indent(); jsonWriter.Write("\"startValue\":[" + minBase + "," + maxBase + "],\n");
            
            CommaNL();

            Indent(); jsonWriter.Write("\"keys\":[\n");
            IndentIn();

            int index = 0;
            float time = 0.0f;
            lenMin = 11;
            for (index = 0; index < lenMin; index++)
            {
                //WriteKeyframe2(dataMin.keys[index], dataMax.keys[index], minBase, maxBase, duration);
                time = (float)(index * (1.0 / (lenMin - 1)));
                WriteKeyframe2_(dataMin.Evaluate(time), dataMax.Evaluate(time), time, minBase, maxBase, duration);
            }
            jsonWriter.Write("\n");

            IndentOut();
            Indent(); jsonWriter.Write("]");

            jsonWriter.Write("\n");
            IndentOut();
        }
        else
        {
            Debug.LogError("关键帧数据错误：" + name + "");
        }


        Indent(); jsonWriter.Write("}\n");
    }

    private void WriteKeyframe( Keyframe data)
	{
		
		CommaNL();
		Indent();	jsonWriter.Write ("{\n" );
			IndentIn();
			CommaNL();
			Indent();	jsonWriter.Write ("\"inTangent\":"+ data.inTangent);
			CommaNL();
			Indent();	jsonWriter.Write ("\"outTangent\":"+ data.outTangent);
			CommaNL();
			Indent();	jsonWriter.Write ("\"time\":"+ data.time);
			CommaNL();
			Indent();	jsonWriter.Write ("\"value\":"+ data.value);
			CommaNL();
			Indent();	jsonWriter.Write ("\"tangentMode\":"+ data.tangentMode);
		IndentOut ();
		Indent();	jsonWriter.Write ("}");

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
        //Indent(); jsonWriter.Write("[" + dataMin.time / duration + "," + dataMin.value + "," + dataMax.value + "]");
        Indent(); jsonWriter.Write("[" + dataMin.time + "," + dataMin.value + "," + dataMax.value + "]");
    }

    private void WriteKeyframe2_(float dataMin, float dataMax, float time, float baseMin, float baseMax, float duration)
    {

        CommaNL();
        //Indent(); jsonWriter.Write("[" + dataMin.time / duration + "," + dataMin.value + "," + dataMax.value + "]");
        Indent(); jsonWriter.Write("[" + time + "," + dataMin + "," + dataMax + "]");
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
        //Indent(); jsonWriter.Write("[" + dataMin.time / duration + "," + dataMin.value + "]");
        Indent(); jsonWriter.Write("[" + dataMin.time + "," + dataMin.value + "]");
    }
    private void WriteKeyframe1_(float value, float time, float baseMin, float duration)
    {

        CommaNL();
        //Indent(); jsonWriter.Write("[" + dataMin.time / duration + "," + dataMin.value + "]");
        Indent(); jsonWriter.Write("[" + time + "," + value + "]");
    }
    private void WriteStartColor(ParticleSystem.MinMaxGradient data, string name)
    {
        CommaNL();
        Indent(); jsonWriter.Write("\"" + name + "\":{\n");
        IndentIn();
        CommaNL();
        Indent(); jsonWriter.Write("\"mode\":" + (int)data.mode);
        switch (data.mode)
        {
            case ParticleSystemGradientMode.Color:
                CommaNL();
                Indent(); jsonWriter.Write("\"colors\":[\n");
                IndentIn();
                CommaNL();

                WriteColor(data.color, -1, "color");

                jsonWriter.Write("\n");

                IndentOut();
                Indent(); jsonWriter.Write("]");

                startColor.r = Mathf.RoundToInt(data.color.r * 255) / 255.0f;
                startColor.g = Mathf.RoundToInt(data.color.g * 255) / 255.0f;
                startColor.b = Mathf.RoundToInt(data.color.b * 255) / 255.0f;
                startColor.a = data.color.a;
                break;
            case ParticleSystemGradientMode.Gradient:
                //WriteGradient2( data.gradient, "gradient");
                Debug.LogWarning("Start Color 仅支持常量设置！");
                break;
            case ParticleSystemGradientMode.RandomColor:
                Debug.LogWarning("Start Color 仅支持常量设置！");
                break;
            case ParticleSystemGradientMode.TwoColors:
                //CommaNL();
                //Indent(); jsonWriter.Write("\"colors\":[\n");
                //IndentIn();
                //CommaNL();

                //WriteColor(data.colorMin, -1, "colorMin");
                //CommaNL();
                //WriteColor(data.colorMax, -1, "colorMax");

                //jsonWriter.Write("\n");

                //IndentOut();
                //Indent(); jsonWriter.Write("]");
                Debug.LogWarning("Start Color 仅支持常量设置！");
                break;
            case ParticleSystemGradientMode.TwoGradients:
                //WriteGradient2( data.gradientMax, "gradientMax");
                //WriteGradient2( data.gradientMin, "gradientMin");
                Debug.LogWarning("Start Color 仅支持常量设置！");
                break;
            default:
                WriteColor(startColor, -1, "color");
                break;
        }

        jsonWriter.Write("\n");
        IndentOut();
        Indent(); jsonWriter.Write("}");
    }

    private void WriteMinMaxGradient( ParticleSystem.MinMaxGradient data, string name)
	{
		CommaNL();
		Indent();	jsonWriter.Write ("\"" + name + "\":{\n" );
		IndentIn ();
		CommaNL();
		Indent();	jsonWriter.Write ("\"mode\":"+ (int)data.mode);
		switch(data.mode)
		{
		    case ParticleSystemGradientMode.Color:
                CommaNL();
                Indent(); jsonWriter.Write("\"colors\":[\n");
                IndentIn();
                CommaNL();

                WriteColor(data.color, -1, "color");

                jsonWriter.Write("\n");

                IndentOut();
                Indent(); jsonWriter.Write("]");
                
                break;
		    case ParticleSystemGradientMode.Gradient:
                WriteGradient2(data.gradient, "gradient");
                //Debug.LogWarning("Start Color 仅支持常量设置！");
                break;
		    case ParticleSystemGradientMode.RandomColor:
                //Debug.LogWarning("Start Color 仅支持常量设置！");
                break;
		    case ParticleSystemGradientMode.TwoColors:
                CommaNL();
                Indent(); jsonWriter.Write("\"colors\":[\n");
                IndentIn();
                CommaNL();

                WriteColor(data.colorMin, -1, "colorMin");
                CommaNL();
                WriteColor(data.colorMax, -1, "colorMax");

                jsonWriter.Write("\n");

                IndentOut();
                Indent(); jsonWriter.Write("]");
                break;
		    case ParticleSystemGradientMode.TwoGradients:
                WriteGradient2(data.gradientMax, "gradientMax");
                WriteGradient2(data.gradientMin, "gradientMin");
                //Debug.LogWarning("Start Color 仅支持常量设置！");
			    break;
		    default:
                //WriteColor(startColor, -1, "color");
                break;
		}

        jsonWriter.Write("\n");
        IndentOut ();
		Indent();	jsonWriter.Write ("}");
	}

	private void WriteColor(  Color data, float alpha, string name)
	{
		if (data == null)
			return;
		Indent();	jsonWriter.Write ("[\n" );
		IndentIn ();

		CommaNL();
		Indent();	jsonWriter.Write ( Mathf.RoundToInt(data.r * 255) / 255.0f );

		CommaNL();
		Indent();	jsonWriter.Write( Mathf.RoundToInt(data.g * 255) / 255.0f);

		CommaNL();
		Indent();	jsonWriter.Write( Mathf.RoundToInt(data.b * 255) / 255.0f);

		CommaNL();
		Indent();	jsonWriter.Write ( (alpha != -1 ? alpha : data.a) + "\n");

		IndentOut ();
		Indent();	jsonWriter.Write ("]");
    }

    private void WriteColorMixStartColor(Color data, float alpha, string name)
    {
        if (data == null)
            return;
        Indent(); jsonWriter.Write("[\n");
        IndentIn();

        CommaNL();
        Indent(); jsonWriter.Write(Mathf.RoundToInt(data.r * startColor.r * 255) / 255.0f);

        CommaNL();
        Indent(); jsonWriter.Write(Mathf.RoundToInt(data.g * startColor.g * 255) / 255.0f);

        CommaNL();
        Indent(); jsonWriter.Write(Mathf.RoundToInt(data.b * startColor.b * 255) / 255.0f);

        CommaNL();
        Indent(); jsonWriter.Write((alpha != -1 ? alpha : data.a) * startColor.a + "\n");

        IndentOut();
        Indent(); jsonWriter.Write("]");
    }

    private void WriteGradient2(Gradient data, string name)
    {
        if (data == null)
            return;
        CommaNL();
        Indent(); jsonWriter.Write("\"" + name + "\":[\n");
        IndentIn();

        List<float> alphaTimes = new List<float>();
        List<float> alphaValues = new List<float>();
        List<float> rgbTimes = new List<float>();
        List<float> rValues = new List<float>();
        List<float> gValues = new List<float>();
        List<float> bValues = new List<float>();
        List<float> times = new List<float>();

        foreach (var v in data.alphaKeys)
        {
            //WriteGradientAlphaKey(v);
            alphaValues.Add(v.alpha);
            alphaTimes.Add(v.time);
            times.Add(v.time);
        }

        foreach (var v in data.colorKeys)
        {
            rgbTimes.Add(v.time);
            rValues.Add(v.color.r);
            gValues.Add(v.color.g);
            bValues.Add(v.color.b);

            if (!times.Contains(v.time))
            {
                times.Add(v.time);
            }
        }

        times.Sort();
        foreach (var v in times)
        {
            CommaNL();
            Indent(); jsonWriter.Write("[\n");
            IndentIn();

            CommaNL();
            Indent(); jsonWriter.Write("" + v);
            CommaNL();

            float alpha = getCurveTempValue(alphaTimes, alphaValues, v);
            float r = getCurveTempValue(rgbTimes, rValues, v);
            float g = getCurveTempValue(rgbTimes, gValues, v);
            float b = getCurveTempValue(rgbTimes, bValues, v);
            WriteColor(new Color(r, g, b), alpha, "color");

            jsonWriter.Write("\n");

            IndentOut();
            Indent(); jsonWriter.Write("]");
        }

        jsonWriter.Write("\n");

        IndentOut();
        Indent(); jsonWriter.Write("]");
    }

    private void WriteGradientAlphaKey( GradientAlphaKey data)
	{
		CommaNL();
		Indent();	jsonWriter.Write ("{\n" );
		IndentIn ();
		CommaNL();
		Indent();	jsonWriter.Write ("\"time\":"+data.time);
		CommaNL();
		Indent();	jsonWriter.Write ("\"alpha\":"+data.alpha + "\n");
		IndentOut ();
		Indent();	jsonWriter.Write ("}");
	}

	private void WriteBurst(UnityEngine.ParticleSystem.Burst data)
	{
		CommaNL();
		Indent();	jsonWriter.Write ("{\n" );
		IndentIn ();

		CommaNL();
		Indent();	jsonWriter.Write ("\"minCount\":" + data.minCount);

		CommaNL();
		Indent();	jsonWriter.Write ("\"maxCount\":" + data.maxCount);

		CommaNL();
		Indent();	jsonWriter.Write ("\"time\":" + data.time + "\n");
        
        IndentOut ();
		Indent();	jsonWriter.Write ("}");
	}

    private float getCurveTempValue(List<float> times, List<float> values, float time)
    {
        float value = 0.0f;

        int count = times.Count;
        int index = 0;
        for (index = 0; index < count; index++)
        {
            if (time < times[index])
            {
                if (index == 0)
                {
                    value = values[index];
                } else
                {
                    value = values[index - 1] + (values[index] - values[index - 1]) * (time - times[index - 1]) / (times[index] - times[index - 1]);
                }
                break;
            }
            else if (time == times[index])
            {
                value = values[index];
                break;
            }
            else if (index == count - 1)
            {
                value = values[index];
            }
        }

        return value;
    }
}
#endif