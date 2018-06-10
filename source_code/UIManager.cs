using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class UIManager
{

    private Text _FinishedPlayersText = null;
    private Text _Hint_PickUp = null;
    private Text _Hint_NoSpace = null;
    private Outline _Hint_NoSpaceOutline = null;

    private Text _Hint_EndRound = null;

    private DamageAid _Aid_Damaged = null;

    private Stats _MainPlayerStats = null;

    // private RectTransform _Element_SprintBar = null;
    //private RectTransform _Element_FoodBar = null;

    private static UIManager _instance = null;


    public enum Bars { Sprint, Food, Health, Hydration, Warmth, E }

    public struct Bar
    {
        public int address;
        public float totalWidth;
        public float totalHeight;
        public RectTransform rectTransform;
        public float totalValue;
        public float currentValue; // not fully added
        public float uiRatio;
        public RectTransform.Edge edge;
    };

    Bar _SprintBar = new Bar();
    Bar _FoodBar = new Bar();
    Bar _HealthBar = new Bar();
    Bar _HydrationBar = new Bar();
    Bar _WarmthBar = new Bar();
    Bar _EBar = new Bar();

    public enum WarmthState { Neutral, Freezing, Cold, Hot, Boiling };
    WarmthState _WarmthState = WarmthState.Neutral;
    Color _OriginalWarmthColour;

    float _WarmthValue = 50;

    DayNightCycle _DayNightCycle = null;


    protected UIManager() { }


    // Singleton pattern implementation
    public static UIManager Instance
    {
        get
        {
            if (UIManager._instance == null)
            {
                UIManager._instance = new UIManager();
            }
            return UIManager._instance;
        }
    }

    public float GetWarmthValue()
    {
        return _WarmthValue;
    }

    public WarmthState GetWarmthState()
    {
        return _WarmthState;
    }


    public void Initialise()
    {
        _DayNightCycle = GameObject.FindGameObjectWithTag("Sun").GetComponent<DayNightCycle>();
        if (_DayNightCycle == null) Debug.LogError("_DayNightCycle not found!");

        _Hint_EndRound = GameObject.FindGameObjectWithTag("UI_Hint_EndRound").GetComponent<Text>();
        if (_Hint_EndRound == null) Debug.LogError("UI_Hint_EndRound not found!");

        _FinishedPlayersText = GameObject.FindGameObjectWithTag("FinishedPlayersText").GetComponent<Text>();
        if (_FinishedPlayersText == null) Debug.LogError("FinishedPlayersText not found!");

        _Hint_PickUp = GameObject.FindGameObjectWithTag("UI_Hint_Pickup").GetComponent<Text>();
        if (_Hint_PickUp == null) Debug.LogError("UI_Hint_Pickup not found!");

        _Hint_NoSpace = GameObject.FindGameObjectWithTag("UI_Hint_NoSpace").GetComponent<Text>();
        if (_Hint_NoSpace == null) Debug.LogError("UI_Hint_NoSpace not found!");
        _Hint_NoSpaceOutline = _Hint_NoSpace.transform.GetComponent<Outline>();

        _Aid_Damaged = GameObject.FindGameObjectWithTag("UI_Aid_Damaged").GetComponent<DamageAid>();
        if (_Hint_NoSpace == null) Debug.LogError("_Hint_NoSpace not found!");

        _SprintBar = SetBarStruct(100f, 100f, "UI_Element_SprintBar");
        ShowBar(Bars.Sprint, false);
        //_SprintBar.address = &_SprintBar;

        _FoodBar = SetBarStruct(100f, 60F, "UI_Element_FoodBar");

        _HealthBar = SetBarStruct(100f, 100f, "UI_Element_HealthBar");

        _HydrationBar = SetBarStruct(100f, 60f, "UI_Element_HydrationBar");

        _WarmthBar = SetBarStruct(100f, 100f, "UI_Element_WarmthBar");

        _EBar = SetBarStruct(100f, 10f, "UI_Element_E");
        ShowBar(Bars.E, false);

        _OriginalWarmthColour = _WarmthBar.rectTransform.GetComponent<Image>().color;

        _MainPlayerStats = Camera.main.transform.parent.GetComponent<Stats>();
    }

    public void AdjustWarmthValue(float value, bool fireSource, ID.Specification spec)
    {

        // could do for >0 (Remove it), i would have to make fireplace +2 though so it actually increases. (THIS COULD WORK!)
        if (value < 0)
        {
            switch(_WarmthState)
            {
                case WarmthState.Hot:
                    value *= 3f;
                    break;

                case WarmthState.Neutral:
                    value *= 2f;
                    break;

                case WarmthState.Cold:
                    value *= 1f;
                    break;
            }
            if(_DayNightCycle._DayState == DayNightCycle.DayState.Night)
            {
                value *= 1.5f;
            }
           // value = Mathf.RoundToInt(1 + (0.1f * (_WarmthValue - 10f)));
           // Debug.Log("Val: " + value);
        }
        else if(value > 0)
        {
            switch (_WarmthState)
            {
                case WarmthState.Hot:
                    value /= 7f;
                    break;

                case WarmthState.Neutral:
                    value /= 5f;
                    break;

                case WarmthState.Cold:
                    value /= 3f;
                    break;
            }
            if(_DayNightCycle._DayState == DayNightCycle.DayState.Night)
            {
                value *=  1.5f;
            }
        }

        //Debug.Log(value);

        if (fireSource && _WarmthState == WarmthState.Hot)
        {
            if (_WarmthValue + value >= 89)
            {
                _WarmthValue = 87;
                value = 0;
            }
           // return;
        }

        if(spec == ID.Specification.WoodenTorch && _WarmthState == WarmthState.Cold)
        {
            // CANT GET ANY WARMER THAN COLD IF COLD
            if(_WarmthValue + value >= 25)
            {
                _WarmthValue = 25;
                value = 0;
               // return;
            }
        }

        if (_MainPlayerStats.playerWithinShelter)
        {
            if (_WarmthState == WarmthState.Cold || _WarmthState == WarmthState.Freezing)
            {
                // Debug.Log("HUH: " +  _WarmthValue);
                if (_WarmthValue <= 10f || (_WarmthValue + value) <= 10f)
                {
                    //    Debug.Log("HUH2");
                    _WarmthValue = 11;
                    value = 0;
                }
            }
        }

           
      
        if(_DayNightCycle._DayState != DayNightCycle.DayState.Night)
        {
            //Debug.Log("000H");
            if(_WarmthState == WarmthState.Cold || _WarmthState == WarmthState.Freezing)
            {
               // Debug.Log("HUH: " +  _WarmthValue);
                if (_WarmthValue <= 10f || (_WarmthValue + value) <= 10f)
                {
                //    Debug.Log("HUH2");
                    _WarmthValue = 11;
                    value = 0;
                }
            }

        }

       
            if (_WarmthState == WarmthState.Freezing && _DayNightCycle._DayState == DayNightCycle.DayState.Night)
        {
           // value *= 300;
        }

        _WarmthValue += value;

        if (_WarmthValue < 0) _WarmthValue = 0;
        if (_WarmthValue > 100) _WarmthValue = 100;

        if(_WarmthValue <= 10) _WarmthState = WarmthState.Freezing;
        if(_WarmthValue > 10 && _WarmthValue <= 25) _WarmthState = WarmthState.Cold;
        if (_WarmthValue > 25 && _WarmthValue <= 50) _WarmthState = WarmthState.Neutral;
        if (_WarmthValue < 90 && _WarmthValue >= 75) _WarmthState = WarmthState.Hot;
        if (_WarmthValue >= 90) _WarmthState = WarmthState.Boiling;

        /*if(_WarmthValue == 0)
        {
            Debug.Log("DIE!");
            GameManager.Instance.Die();
        }*/

        switch(_WarmthState)
        {
            case WarmthState.Freezing:
                _WarmthBar.rectTransform.GetChild(0).GetComponent<Text>().text = "Freezing";
                Color32 icey = new Color32(182, 243, 255, 255);
                _WarmthBar.rectTransform.GetComponent<Image>().color = icey;
                break;

            case WarmthState.Cold:
                _WarmthBar.rectTransform.GetChild(0).GetComponent<Text>().text = "Cold";
                Color32 bluey = new Color32(124, 233, 255, 255);
                _WarmthBar.rectTransform.GetComponent<Image>().color = bluey;
                break;

            case WarmthState.Neutral:
                _WarmthBar.rectTransform.GetChild(0).GetComponent<Text>().text = "Neutral";
                _WarmthBar.rectTransform.GetComponent<Image>().color = _OriginalWarmthColour;
                break;

            case WarmthState.Hot:
                _WarmthBar.rectTransform.GetChild(0).GetComponent<Text>().text = "Hot";
                _WarmthBar.rectTransform.GetComponent<Image>().color = Color.magenta;
                break;

            case WarmthState.Boiling:
                _WarmthBar.rectTransform.GetChild(0).GetComponent<Text>().text = "Boiling";
                _WarmthBar.rectTransform.GetComponent<Image>().color = Color.red;
                break;
        }
    }

    private Bar SetBarStruct(float totalValue, float currentValue, string tag)
    {
       // Debug.Log("1");
        Bar _Bar = new Bar();
       // Debug.Log("2");
       // Debug.Log(_Bar.rectTransform);

        if (!GameObject.FindGameObjectWithTag(tag)){
            Debug.LogError("GameObject not found! :::: " + tag);
            return new Bar();
        }

        _Bar.rectTransform = GameObject.FindGameObjectWithTag(tag).GetComponent<RectTransform>();
        if (_Bar.rectTransform == null) Debug.LogError(tag + " not found!");
        
        _Bar.totalWidth = _Bar.rectTransform.rect.width;
        _Bar.totalHeight = _Bar.rectTransform.rect.height;

        _Bar.totalValue = totalValue;
        
        _Bar.uiRatio = _Bar.totalWidth / _Bar.totalValue;

        _Bar.currentValue = currentValue;

        _Bar.edge = RectTransform.Edge.Left;

        if(_Bar.totalWidth > _Bar.totalHeight)
        {
            _Bar.edge = RectTransform.Edge.Left;
        }
        else if(_Bar.totalWidth < _Bar.totalHeight)
        {
            _Bar.edge = RectTransform.Edge.Bottom;
        }
        else if(_Bar.totalWidth == _Bar.totalHeight)
        {
            Debug.LogError("Bar width and height should'nt be equal");
        }

        _Bar.rectTransform.SetInsetAndSizeFromParentEdge(_Bar.edge, 0F, _Bar.rectTransform.rect.width - (_Bar.uiRatio * (_Bar.totalValue - _Bar.currentValue)));
                
 

        return _Bar;
    }


    public void ShowHint_EndRound(bool state)
    {
        GetUIHint_EndRound().enabled = state;
    }

    public void ShowBar(Bars _Bar, bool show)
    {
        
        switch (_Bar)
        {
            case Bars.Food:
                _FoodBar.rectTransform.transform.parent.gameObject.SetActive(show);
                break;

            case Bars.Sprint:
                _SprintBar.rectTransform.transform.parent.gameObject.SetActive(show);
                break;

            case Bars.Health:
                _HealthBar.rectTransform.transform.parent.gameObject.SetActive(show);
                break;

            case Bars.Hydration:
                _HydrationBar.rectTransform.transform.parent.gameObject.SetActive(show);
                break;

            case Bars.Warmth:
                _WarmthBar.rectTransform.transform.parent.gameObject.SetActive(show);
                break;

            case Bars.E:
                _EBar.rectTransform.transform.parent.gameObject.SetActive(show);
                if (show) _EBar.rectTransform.transform.parent.gameObject.GetComponent<DecreasePerSecond>().StarDecrease();
                //if(show) SetBar(10f, Bars.E); 
                break;
        }

    }

    public void ShowPickup(string name, bool show)
    {
        _Hint_PickUp.enabled = show;
        _Hint_PickUp.text = "Press 'E' \n (" + name + ")";
    }


    public void ShowNoSpace(bool show)
    {
        Color newColor = _Hint_NoSpace.color;
        newColor = new Color(newColor.r, newColor.g, newColor.b, 1f);
        _Hint_NoSpace.color = newColor;

        Color newColor2 = _Hint_NoSpaceOutline.effectColor;
        newColor2 = new Color(newColor2.r, newColor2.g, newColor2.b, 1f);
        _Hint_NoSpaceOutline.effectColor = newColor2;

        _Hint_NoSpace.enabled = show;
    }

    public void AdjustNoSpaceTransparency(float value)
    {
        Color newColor =  _Hint_NoSpace.color;
        newColor = new Color(newColor.r, newColor.g, newColor.b, newColor.a + (value/100f));
        _Hint_NoSpace.color = newColor;

        Color newColor2 = _Hint_NoSpaceOutline.effectColor;
        newColor2 = new Color(newColor2.r, newColor2.g, newColor2.b, newColor2.a + (value / 100f));
        _Hint_NoSpaceOutline.effectColor = newColor2;

 
    }

    public Text GetUIHint_EndRound()
    {
        return _Hint_EndRound;
    }

    public Bar GetBar(Bars _Bars)
    {
        Bar _Bar = new Bar();

        switch (_Bars)
        {
            case Bars.Food:
                _Bar = _FoodBar;
                break;

            case Bars.Sprint:
                _Bar = _SprintBar;
                break;

            case Bars.Health:
                _Bar = _HealthBar;
                break;

            case Bars.Hydration:
                _Bar = _HydrationBar;
                break;

            case Bars.Warmth:
                _Bar = _WarmthBar;
                break;

            case Bars.E:
                _Bar = _EBar;
                break;
        }

        return _Bar;
    }

    public void SetFinishedPlayers(int number, int total)
    {
        GetFinishedPlayers().text = number + "/" + total;
    }

    public void SetBar(float value, Bars _Bar)
    {
        Bar tempBar = GetBar(_Bar);

  
        tempBar.rectTransform.SetInsetAndSizeFromParentEdge(tempBar.edge, 0F, tempBar.uiRatio * value);
        tempBar.currentValue = value;

        switch (_Bar)
        {
            case Bars.Food:
                _FoodBar = tempBar;
                break;

            case Bars.Health:
                _HealthBar = tempBar;
                break;

            case Bars.Hydration:
                _HydrationBar = tempBar;
                break;

            case Bars.Sprint:
                _SprintBar = tempBar;
                break;

            case Bars.Warmth:
                _WarmthBar = tempBar;
                break;

            case Bars.E:
                _EBar = tempBar;
                break;
        } 
    }

    public void AdjustBar(float manipulationValue, Bars _Bars)
    {


       // float fill = 0f;

        Bar tempBar = GetBar(_Bars);  //NEEDS TO BE A POINTER, this just copies the bar...
        

       // if(tempBar.rectTransform == _SprintBar.rectTransform)
       // Debug.Log("CurrentValue: " + tempBar.currentValue + " ::manip:: " + manipulationValue);


        //if (manipulationValue == tempBar.totalValue)
          //  fill = -tempBar.rectTransform.rect.width;

        if (tempBar.currentValue + manipulationValue < 0f)
        {
            // make UI to zero;
            tempBar.rectTransform.SetInsetAndSizeFromParentEdge(tempBar.edge, 0F,0f);
            tempBar.currentValue = 0f;
        }
        else if (tempBar.currentValue + manipulationValue > tempBar.totalValue)
        {
            //fill = (tempBar.totalValue - tempBar.currentValue);
           // tempBar.rectTransform.SetInsetAndSizeFromParentEdge(tempBar.edge, 0F, tempBar.rectTransform.rect.width + (tempBar.uiRatio * fill));
            tempBar.rectTransform.SetInsetAndSizeFromParentEdge(tempBar.edge, 0F, tempBar.totalValue * tempBar.uiRatio);
            tempBar.currentValue = tempBar.totalValue;
        }
        else
        {
            tempBar.currentValue += manipulationValue;
            tempBar.rectTransform.SetInsetAndSizeFromParentEdge(tempBar.edge, 0F, tempBar.rectTransform.rect.width + (tempBar.uiRatio * manipulationValue));
        }

        /*
        Color32 originalColor = tempBar.rectTransform.parent.GetChild(0).GetComponent<Image>().color;
        Color32 tempColor = tempBar.rectTransform.parent.GetChild(0).GetComponent<Image>().color;
        tempColor = new Color32(tempColor.r, tempColor.g, tempColor.b,20);
        tempBar.rectTransform.parent.GetChild(0).GetComponent<Image>().color = tempColor;
         * 
         * // then start coroutine with _Bars and originalColor as arguements
         * // and this sets back.
 */

        // this would be removed when pointer system implemented
        switch(_Bars)
        {
            case Bars.Food:
                _FoodBar = tempBar;
                if (manipulationValue > 1f || manipulationValue < -1f)
                _FoodBar.rectTransform.transform.GetComponent<Flash>().FlashTheImage();
                break;

            case Bars.Health:
                _HealthBar = tempBar;
                if(manipulationValue < 0f)
                {
                    _Aid_Damaged.Show();
                   
                }
                _HealthBar.rectTransform.transform.GetComponent<Flash>().FlashTheImage();
                break;

            case Bars.Hydration:
                _HydrationBar = tempBar;

                if(manipulationValue > 1f || manipulationValue < -1f)
                _HydrationBar.rectTransform.transform.GetComponent<Flash>().FlashTheImage();
                break;

            case Bars.Sprint:

                ShowBar(Bars.Sprint, !(tempBar.currentValue == tempBar.totalValue));
                _SprintBar = tempBar;
                break;

            case Bars.Warmth:
                _WarmthBar = tempBar;
                break;

            case Bars.E:
                _EBar = tempBar;
                break;
        } 
       
    }

    private Text GetFinishedPlayers()
    {
        return _FinishedPlayersText;
    }
}