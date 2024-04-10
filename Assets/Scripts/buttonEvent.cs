using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonEvent : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject gridGenerate;
    public GameObject terrainGenerate;
    public void Generate_button()
    {
        
        gridGenerate.SendMessage("builtGrid");
        //terrainGenerate.SendMessage("deleteTerrain");
        //terrainGenerate.SendMessage("builtTerrain");
    }
}
