using UnityEngine;
using System.Collections;

public class FirePlaceManager : MonoBehaviour {

    float prevIntensity = 3.0f;
    float prevRange = 5.0f;

    private void Add()
    {
        
        prevRange *= 3;
    }

    public Light Initialise(Light light)
    {
        Light _Light = light;

        _Light.intensity = prevIntensity;
        _Light.range = prevRange;

        Add();
        return _Light;
    }
}
