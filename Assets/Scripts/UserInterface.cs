using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TemperatureSimulator;
using UnityEngine.UI;
using System.Threading.Tasks;

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
    private void Start()
    {
        SwitchBtn.onClick.AddListener(
            () => {
                if (_isFinish)
                    Init();
                _run = !_run;
                if (_run)
                    SwitchBtn.GetComponentInChildren<TMP_Text>().text = "Ïàóçà";
                else
                    SwitchBtn.GetComponentInChildren<TMP_Text>().text = "Çàïóñê";
            });
        for (int i = 0; i < BorderFields.Length; i++)
        {
            BorderFields[i].options = BorderFieldsData;
        }
    }

    private void Init()
    {
        _isFinish = false;
        _currentCooldown = 0;
        Config config = new Config();
        config.Alfa = float.Parse(AlphaField.text);
        config.H = float.Parse(HField.text);
        config.Tau = float.Parse(TauField.text);
        config.Size = int.Parse(SizeField.text);
        config.TimeMax = float.Parse(TimeMaxField.text);
        config.MaxTemperature = 200;

        _interval = float.Parse(TimeIntervalField.text);

        for (int i = 0; i < Targets.Length; i++)
        {
            Targets[i].MeshRenderer.material.mainTexture = new Texture2D(config.Size, config.Size);
            Targets[i].MeshRenderer.material.mainTexture.filterMode = FilterMode.Point;
            Targets[i].SetDivisions(config.Size);
            Targets[i].SetPosition(-config.Size/2+1);
        }

        for (int i = 0; i < config.BorderÑonditions.Length; i++)
        {
            config.BorderÑonditions[i] = BorderFields[i].value;
        }

        config.XTarget = Targets[0].MeshRenderer.material.mainTexture as Texture2D;
        config.YTarget = Targets[1].MeshRenderer.material.mainTexture as Texture2D;
        config.ZTarget = Targets[2].MeshRenderer.material.mainTexture as Texture2D;

        if (TypeCalculator.value == 0)
        {
            _CurrentCalculator = new Consistently();
            _CurrentCalculator.Init(config);
            for (int i = 0; i < Targets.Length; i++)
            {
                int id = i;
                Targets[i].SliceChange += (int t) => { _CurrentCalculator.SetSlice(id, t); };
            }
        }
        else
        {

        }
    }

    private void Update()
    {
        if (_run)
        {
            if (_currentCooldown >= _interval)
            {
                _currentCooldown = 0;
                if (_CurrentCalculator.CalculateStep())
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
