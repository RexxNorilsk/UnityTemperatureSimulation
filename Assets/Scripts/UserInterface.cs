using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TemperatureSimulator;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class UserInterface : MonoBehaviour
{

    public TMP_Dropdown TypeCalculator;
    public TMP_InputField SizeField;
    public TMP_InputField AlphaField;
    public TMP_InputField TauField;
    public TMP_InputField TimeMaxField;
    public TMP_InputField TimeIntervalField;
    public TMP_InputField HField;
    public TMP_Dropdown[] BorderFields;
    public List<TMP_Dropdown.OptionData> BorderFieldsData;
    public Button SwitchBtn;
    public Slice[] Targets;

    private float _interval;
    private float _currentCooldown;
    private Calculator _CurrentCalculator;
    private bool _run = false;
    private bool _isFinish = true;
    private Vector3[] _defaultPositions = new Vector3[3];

    public Calculator CurrentCalculator { get => _CurrentCalculator;  }

    private void Start()
    {
        SwitchBtn.onClick.AddListener(
            () =>
            {
                SwitchMode();
            });
        for (int i = 0; i < BorderFields.Length; i++)
        {
            BorderFields[i].options = BorderFieldsData;
        }
    }

    public void SwitchMode()
    {
        if (_isFinish)
            Init();
        _run = !_run;
        if (_run)
            SwitchBtn.GetComponentInChildren<TMP_Text>().text = "Ïàóçà";
        else
            SwitchBtn.GetComponentInChildren<TMP_Text>().text = "Çàïóñê";
    }

    public void Init()
    {
        _isFinish = false;
        _currentCooldown = 0;
        Config config = BuildConfig();

        if (TypeCalculator.value == 0)
        {
            _CurrentCalculator = new Consistently();
        }
        else
        {
            _CurrentCalculator = new TemperatureSimulator.Parallel();
        }
        CurrentCalculator.Init(config);
        for (int i = 0; i < Targets.Length; i++)
        {
            int id = i;
            Targets[i].SliceChange += (int t) => { CurrentCalculator.SetSlice(id, t); };
        }
    }

    public Config BuildConfig()
    {
        Config config = new Config();
        config.Alfa = float.Parse(AlphaField.text);
        config.H = float.Parse(HField.text);
        config.Tau = float.Parse(TauField.text);
        config.Size = int.Parse(SizeField.text);
        config.TimeMax = float.Parse(TimeMaxField.text);
        config.MaxTemperature = 200;

        if (config.Alfa * config.Alfa * config.Tau / (config.H * config.H) > 1)
        {
            return null;
        }


        _interval = float.Parse(TimeIntervalField.text);

        for (int i = 0; i < Targets.Length; i++)
        {
            Targets[i].MeshRenderer.material.mainTexture = new Texture2D(config.Size, config.Size);
            Targets[i].MeshRenderer.material.mainTexture.filterMode = FilterMode.Point;

            Targets[i].SetDivisions(config.Size);
            Targets[i].SetPosition(-config.Size / 2 + 1);
        }

        for (int i = 0; i < config.BorderÑonditions.Length; i++)
        {
            config.BorderÑonditions[i] = BorderFields[i].value;
        }

        config.XTarget = Targets[0].MeshRenderer.material.mainTexture as Texture2D;
        config.YTarget = Targets[1].MeshRenderer.material.mainTexture as Texture2D;
        config.ZTarget = Targets[2].MeshRenderer.material.mainTexture as Texture2D;
        return config;
    }

    private void Update()
    {
        if (_run)
        {
            if (_currentCooldown >= _interval)
            {
                _currentCooldown = 0;
                if (CurrentCalculator.CalculateStep())
                {
                    _run = false;
                    _isFinish = true;
                    SwitchBtn.GetComponentInChildren<TMP_Text>().text = "Çàïóñê";
                }
            }
            else
                _currentCooldown += Time.deltaTime;
        }
    }
}
