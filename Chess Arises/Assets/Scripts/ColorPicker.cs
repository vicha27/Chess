using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    public FlexibleColorPicker fcp;
    public Material material;

    public Color externalColor;
    private Color internalColor;
    public static Color temp = new Color(0, 0, 0);
    public static bool colorLoaded = false;

    private void Start()
    {
        internalColor = externalColor;
    }

    private void Update()
    {
        //apply color of this script to the FCP whenever it is changed by the user
        if (internalColor != externalColor)
        {
            fcp.color = externalColor;
            internalColor = externalColor;
        }

        //extract color from the FCP and apply it to the object material
        if (colorLoaded) //if we loaded a color from memory, make that the material color
        {
            fcp.color = temp;
            colorLoaded = false;
        }//otherwise, we just move along as normal
        material.color = fcp.color;
        temp = material.color;
        
    }
    public static Color SaveColorChoice()
    {//return the temporary color we keep in memory
        return temp;
    }

    public static void SetColorChoice(Color c)
    {//set the temp color to the one we have in memory and let the method know
        temp = c;
        colorLoaded = true;
    }
}//end of color picker